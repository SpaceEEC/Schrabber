﻿using Schrabber.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using TagLib;

namespace Schrabber.Workers
{
	public class Splitter
	{
		private static readonly Int32 _maxRunning = Environment.ProcessorCount;

		public static IEnumerable<Job> GetJobs(IEnumerable<Media> medias)
		{
			return medias.SelectMany(media =>
			{
				Job[] jobs = new Job[media.Parts.Length + 1];
				jobs[0] = new Job(media);
				Int32 i = 1;

				Part prev = null;
				foreach (Part cur in media.Parts)
				{
					if (prev != null && prev.Stop == null)
						prev.Stop = cur.Start;

					prev = cur;
					jobs[i++] = new Job(cur);
				}

				return jobs;
			});
		}

		private readonly String _path;
		private readonly IEnumerator _jobs;
		private Int32 running = 0;

		private readonly TaskCompletionSource<Object> _tsc = new TaskCompletionSource<Object>();

		public Splitter(String path, IEnumerable<Job> jobs)
		{
			if (jobs == null)
				throw new ArgumentNullException(nameof(jobs));

			this._path = path ?? throw new ArgumentNullException(nameof(path));
			this._jobs = jobs.GetEnumerator();
		}

		private readonly Object _lock = new Object();

		public Task Start(Boolean ended = false)
		{
			Debug.WriteLine($"{nameof(this.Start)} called");

			lock (this._lock)
			{
				if (ended)
				{
					Debug.Assert(this.running > 0, "this.running is not greater than 0 ");

					--this.running;
					Debug.WriteLine($"Finished job; Still running {this.running}.");
				}

				while (this.running < Splitter._maxRunning)
				{
					// Are we at the end?
					if (!this._jobs.MoveNext())
					{
						Debug.WriteLine($"Reached end of jobs; Still running: {this.running}.");

						if (this.running <= 0)
							this._tsc.TrySetResult(null);

						break;
					}

					++this.running;
					Debug.WriteLine($"Starting job; Now running {this.running}.");

					ThreadPool.QueueUserWorkItem(this.Run, this._jobs.Current);
				}
			}

			return this._tsc.Task;
		}

		private async void Run(Object state)
		{
			try
			{
				await this._run(state).ConfigureAwait(false);
			}
			catch
			{
				Debug.Fail("_run threw an error");
				/* TODO: Do something here */
			}
			finally
			{
#pragma warning disable 4014
				this.Start(true);
#pragma warning restore 4014
			}
		}

		private async Task _run(Object state)
		{
			if (!(state is Job job))
			{
				Debug.Fail($"Got state of type {state?.GetType()?.ToString() ?? "null"}");

				return;
			}

			// Start
			job.Started = true;

			if (job.Target is Media media)
			{
				// Fetch if necessary
				if (media.MustFetch)
				{
					job.Progress = 0D;
					job.Caption = "Fetching";

					await media.FetchAsync(job).ConfigureAwait(false);
				}
			}
			else if (job.Target is Part part)
			{
				// Wait for parent to finish processing if necessary
				if (!part.Parent.FetchTask.IsCompleted)
				{
					job.Caption = "Waiting for parent";
					await part.Parent.FetchTask.ConfigureAwait(false);
				}

				// Do the actual splitting part
				job.Caption = "Splitting";
				String dest = Path.Combine(this._path, this._getFileName(part));
				FFmpeg.Split(part.Parent.GetLocation(), dest, part.Start, part.Stop, job);

				// Write tags
				job.Caption = "Writing tags";
				this._writeTags(dest, part);
			}

			// Done
			job.Finished = true;
		}

		private void _writeTags(String path, Part part)
		{
			using (TagLib.File file = TagLib.File.Create(path))
			{
				file.Tag.Title = part.Title;
				file.Tag.Performers = new[] { part.Author };
				file.Tag.Album = part.Album;
				if (part.CoverImage != null)
				{
					JpegBitmapEncoder encoder = new JpegBitmapEncoder();
					encoder.Frames.Add(BitmapFrame.Create(part.CoverImage));
					using (MemoryStream ms = new MemoryStream())
					{
						encoder.Save(ms);
						file.Tag.Pictures = new IPicture[] { new Picture(new ByteVector(ms.ToArray())) };
					}
				}

				file.Save();
			}
		}

		private String _getFileName(Part part)
		{
			String fileName = part.ToString();

			return String.Join(
				"_",
				fileName.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries)
			// TODO: Configurable extension
			).TrimEnd('.') + ".mp3";
		}
	}
}
