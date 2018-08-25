using Schrabber.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using YoutubeExplode.Models;
using Schrabber.Extensions;

namespace Schrabber.Models
{
	public class Splitter : IProgress<Double>
	{
		private readonly IProgressWindow _window;
		private readonly IInputMedia[] _media;
		public Splitter(IInputMedia[] media, IProgressWindow window)
		{
			_media = media;
			_window = window;

			_window.TotalMediaCount = _media.Length;
			_window.Progress = 0;
			_window.Step = "Initialized";
		}

		public async Task Run(CancellationToken token = default(CancellationToken))
		{
			String folderPath = Path.Combine(
				AppDomain.CurrentDomain.BaseDirectory,
				// TODO: Folder name
				DateTimeOffset.Now.ToUnixTimeSeconds().ToString()
			);
			Directory.CreateDirectory(folderPath);

			foreach (IInputMedia media in _media)
			{
				if (token.IsCancellationRequested)
				{
					_cancelled();
					return;
				}

				_window.CurrentMedia = media;
				_window.Step = "Fetching Video";
				using (MemoryStream ms = await media.GetMemoryStreamAsync(this, token).ConfigureAwait(false))
				{
					if (media.Parts.Length == 1)
					{
						_window.NextPart();
						_window.Step = "Writing Audio";
						_writeTags(ms, media.Parts[0]);
						await _writeFile(
							ms,
							Path.Combine(
								folderPath,
								_getFileName(media.Parts[0])
							),
							token
						).ConfigureAwait(false);

						continue;
					}


					foreach (IPart part in media.Parts)
					{
						_window.NextPart();
						_window.Step = "Splitting";
						using (MemoryStream partMs = await Ffmpeg.SplitMp3Stream(ms, part.Start, part.Stop, this, token).ConfigureAwait(false))
						{
							_writeTags(partMs, part);
							_window.Step = "Writing Audio";
							await _writeFile(
								partMs,
								Path.Combine(
									folderPath,
									_getFileName(part)
								),
								token
							).ConfigureAwait(false);
						}
					}
				}
			}

			_window.Step = "Done";
			_window.Progress = 1;

		}

		private void _writeTags(MemoryStream ms, IPart part)
		{
			TagLib.File file = TagLib.File.Create(new FileStreamAbstraction("file.mp3", ms));
			file.Tag.Title = part.Title;
			file.Tag.Performers = new[] { part.Author };
			file.Tag.Album = part.Album;
			file.Save();

			ms.Position = 0;
		}

		private async Task _writeFile(Stream ms, String filePath, CancellationToken token)
		{
			using (StreamWriter sw = new StreamWriter(filePath)) await ms.CopyToAsync(sw.BaseStream, token: token);
			ms.Position = 0;
		}
		private String _getFileName(IPart part)
		{
			String fileName = String.IsNullOrEmpty(part.Author) ? part.Title : $"{part.Author} - {part.Title}";

			return String.Join("_", fileName.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.') + ".mp3";
		}


		private void _cancelled()
		{
			_window.Step = "Cancelled";
			_window.Progress = 1;
		}

		public void Report(double value) =>
			_window.Progress = value;
	}
}
