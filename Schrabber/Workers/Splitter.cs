using Schrabber.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Schrabber.Workers
{
	internal class Splitter
	{
		private static readonly Int32 _maxRunning =  Environment.ProcessorCount;

		internal static Dictionary<Job<Media>, Job<Part>[]> GetJobs(IEnumerable<Media> medias)
		{
			return medias.ToDictionary(
				media => new Job<Media>(media),
				media => {
					Job<Part>[] parts = new Job<Part>[media.Parts.Length];
					Int32 i = 0;

					Part prev = null;
					foreach (Part cur in media.Parts)
					{
						if (prev != null && cur.Start == null)
							cur.Start = prev.Stop;

						prev = cur;
						parts[i++] = new Job<Part>(cur);
					}

					return parts;
				}
			);
		}

		private readonly String _path;
		private readonly IEnumerator _jobs;
		private Int32 running = 0;

		private readonly TaskCompletionSource<Object> _tsc = new TaskCompletionSource<Object>();

		internal Splitter(String path, Dictionary<Job<Media>, Job<Part>[]> jobs)
		{
			if (jobs == null)
				throw new ArgumentNullException(nameof(jobs));

			this._path = path ?? throw new ArgumentNullException(nameof(path));
			this._jobs = this._enumerateJobs(jobs).GetEnumerator();
		}

		private IEnumerable _enumerateJobs(Dictionary<Job<Media>, Job<Part>[]> jobs)
		{
			foreach (KeyValuePair<Job<Media>, Job<Part>[]> kv in jobs)
			{
				yield return kv.Key;
				foreach (Job<Part> job in kv.Value)
					yield return job;
			}
		}

		private readonly Object _lock = new Object();

		internal Task Start()
		{
			Debug.WriteLine($"{nameof(this.Start)} called");
			lock (this._lock)
			{
				while (this.running < Splitter._maxRunning)
				{
					// Are we at the end?
					if (!this._jobs.MoveNext())
					{
						Debug.WriteLine($"Reached end of jobs; Still running: {this.running}.");

						if (this.running == 0)
							this._tsc.TrySetResult(null);

						break;
					}

					++this.running;
					Debug.WriteLine($"Starting job; Now running {this.running}.");

					ThreadPool.QueueUserWorkItem(this._run, this._jobs.Current);
				}
			}

			return this._tsc.Task;
		}

		private void _jobFinished()
		{
			lock (this._lock)
			{
				--this.running;
				Debug.WriteLine($"Finished job; Still running {this.running}.");
			}

			this.Start();
		}

		private async void _run(Object obj)
		{
			if (obj is Job<Media> mediaJob)
			{
				// Fetch if necessary
				if (mediaJob.Target.MustFetch)
				{
					mediaJob.Progress = 0D;
					mediaJob.Caption = "Fetching";

					await mediaJob.Target.FetchAsync(mediaJob);
				}

				// Done
				mediaJob.Caption = "Done";
				mediaJob.Progress = 1D;

				this._jobFinished();
			}
			else if (obj is Job<Part> partJob)
			{
				Part part = partJob.Target;

				// Wait for parent to finish processing if necessary
				if (!part.Parent.FetchTask.IsCompleted)
				{
					partJob.Caption = "Waiting for parent";
					await part.Parent.FetchTask;
				}

				// Do the actual splitting part
				partJob.Caption = "Splitting";
				String dest = Path.Combine(this._path, this._getFileName(part));
				FFmpeg.Split(part.Parent.Location, dest, part.Start, part.Stop, partJob);

				// Write tags
				partJob.Caption = "Writing tags";
				partJob.Progress = 0D;
				this._writeTags(dest, part);

				// Done
				partJob.Caption = "Done";
				partJob.Progress = 1D;

				this._jobFinished();
			}
			else
			{
				this._jobFinished();

				// TODO: This can not be caught. It should not happen in the first place however.
				throw new ArgumentException($"Expect type of {nameof(Job<Media>)} or {nameof(Job<Part>)}, but got {obj?.GetType()?.ToString() ?? "null"}.");
			}
		}

		private void _writeTags(String path, Part part)
		{
			TagLib.File file = TagLib.File.Create(path);
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
					file.Tag.Pictures = new TagLib.IPicture[]
					{
						new TagLib.Picture(new TagLib.ByteVector(ms.ToArray()))
					};
				}
			}

			file.Save();
		}

		private String _getFileName(Part part)
		{
			String fileName = String.IsNullOrWhiteSpace(part.Author) ? part.Title : $"{part.Author} - {part.Title}";

			return String.Join(
				"_",
				fileName.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries)
			// TODO: Configurable extension
			).TrimEnd('.') + ".mp3";
		}
	}
}
