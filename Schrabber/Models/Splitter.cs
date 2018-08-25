using Schrabber.Extensions;
using Schrabber.Helpers;
using Schrabber.Interfaces;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Schrabber.Models
{
	public class Splitter : IProgress<Double>
	{
		private readonly IProgressWindow _window;
		private readonly IInputMedia[] _media;
		private Boolean _started = false;
		private String _folderPath;
		public String FolderPath
		{
			get
			{
				if (_folderPath != null) return _folderPath;
				String folderPath = Path.Combine(
					AppDomain.CurrentDomain.BaseDirectory,
					DateTimeOffset.Now.ToUnixTimeSeconds().ToString()
				);
				Directory.CreateDirectory(folderPath);

				return _folderPath = folderPath;
			}
			set
			{
				if (_started) throw new InvalidOperationException($"Can not set \"FolderPath\" after starting.");
				_folderPath = value;
			}
		}

		public Splitter(IInputMedia[] media, IProgressWindow window)
		{
			_media = media;
			_window = window;

			_window.Splitter = this;
			_window.TotalMediaCount = _media.Length;
			_window.Progress = 0;
			_window.Step = "Initialized";
		}

		public async Task Run(CancellationToken token = default(CancellationToken))
		{
			_started = true;

			foreach (IInputMedia media in _media)
			{
				if (token.IsCancellationRequested)
				{
					_cancelled();
					return;
				}

				_window.CurrentMedia = media;
				_window.Step = "Fetching Video";
				MemoryStream ms = await media.GetMemoryStreamAsync(this, token).ConfigureAwait(false);
				if (media.Parts.Length == 1)
				{
					_window.NextPart();
					_window.Step = "Writing Audio";
					_writeTags(ms, media.Parts[0], media);
					await _writeFile(
						ms,
						Path.Combine(
							FolderPath,
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
						_writeTags(partMs, part, media);
						_window.Step = "Writing Audio";
						await _writeFile(
							partMs,
							Path.Combine(
								FolderPath,
								_getFileName(part)
							),
							token
						).ConfigureAwait(false);
					}
				}
			}

			_window.Step = "Done";
			_window.Progress = 1;

		}

		private void _writeTags(MemoryStream ms, IPart part, IInputMedia media)
		{
			TagLib.File file = TagLib.File.Create(new FileStreamAbstraction("file.mp3", ms));
			file.Tag.Title = part.Title;
			file.Tag.Performers = new[] { part.Author };
			file.Tag.Album = part.Album;
			if (media.CoverImage != null)
			{
				JpegBitmapEncoder encoder = new JpegBitmapEncoder();
				encoder.Frames.Add(BitmapFrame.Create(media.CoverImage));
				using (MemoryStream imageMs = new MemoryStream())
				{
					encoder.Save(imageMs);
					file.Tag.Pictures = new TagLib.IPicture[1]
					{
						new TagLib.Picture(
							new TagLib.ByteVector(imageMs.ToArray())
						)
					};
				}

			}
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
