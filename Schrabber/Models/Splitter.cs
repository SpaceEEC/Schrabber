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
				if (this._folderPath != null) return this._folderPath;
				String folderPath = Path.Combine(
					AppDomain.CurrentDomain.BaseDirectory,
					DateTimeOffset.Now.ToUnixTimeSeconds().ToString()
				);
				Directory.CreateDirectory(folderPath);

				return this._folderPath = folderPath;
			}
			set
			{
				if (this._started) throw new InvalidOperationException($"Can not set \"FolderPath\" after starting.");
				this._folderPath = value;
			}
		}

		public Splitter(IInputMedia[] media, IProgressWindow window)
		{
			this._media = media;
			this._window = window;

			this._window.Splitter = this;
			this._window.TotalMediaCount = this._media.Length;
			this._window.Progress = 0;
			this._window.Step = "Initialized";
		}

		public async Task Run(CancellationToken token = default(CancellationToken))
		{
			this._started = true;

			foreach (IInputMedia media in this._media)
			{
				if (token.IsCancellationRequested)
				{
					this._cancelled();
					return;
				}

				this._window.CurrentMedia = media;
				this._window.Step = "Fetching Video";
				MemoryStream ms = await media.GetMemoryStreamAsync(this, token).ConfigureAwait(false);
				if (media.Parts.Length == 1)
				{
					this._window.NextPart();
					this._window.Step = "Writing Audio";
					this._writeTags(ms, media.Parts[0], media);
					await this._writeFile(
						ms,
						Path.Combine(
							this.FolderPath,
							this._getFileName(media.Parts[0])
						),
						token
					).ConfigureAwait(false);

					continue;
				}


				foreach (IPart part in media.Parts)
				{
					this._window.NextPart();
					this._window.Step = "Splitting";
					using (MemoryStream partMs = await Ffmpeg.SplitMp3Stream(ms, part.Start, part.Stop, this, token).ConfigureAwait(false))
					{
						this._writeTags(partMs, part, media);
						this._window.Step = "Writing Audio";
						await this._writeFile(
							partMs,
							Path.Combine(
								this.FolderPath,
								this._getFileName(part)
							),
							token
						).ConfigureAwait(false);
					}
				}
			}

			this._window.Step = "Done";
			this._window.Progress = 1;

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
			this._window.Step = "Cancelled";
			this._window.Progress = 1;
		}

		public void Report(double value) =>
			this._window.Progress = value;
	}
}
