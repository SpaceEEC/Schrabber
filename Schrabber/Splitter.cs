using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Models;
using System.Linq;

namespace Schrabber
{
	public class Splitter : IProgress<Double>
	{
		public Action<String> UpdateCurrentAction;
		public Action<Double> UpdateCurrentProgress;

		private readonly Video _video;
		private readonly List<Part> _parts;
		public Splitter(Video video, List<Part> parts)
		{
			_video = video;
			_parts = parts;
		}

		public async Task Run()
		{
			UpdateCurrentAction?.Invoke("Fetching Video");
			using (MemoryStream ms = await YouTubeClient.DownloadMp3VideoMemoryStreamAsync(_video, _parts[0].Timestamp, this))
			{
				String folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _video.Id);
				Directory.CreateDirectory(folderPath);

				if (_parts.Count == 1)
				{
					UpdateCurrentAction?.Invoke("Writing Audio");
					_writeTags(ms, _parts[0]);
					await _writeFile(ms, Path.Combine(folderPath, _getFileName(_parts[0])));
					UpdateCurrentAction?.Invoke("Done");
					UpdateCurrentProgress?.Invoke(1);

					return;
				}

				for (int i = 0; i < _parts.Count; ++i)
				{
					TimeSpan end = i + 1 == _parts.Count ? _video.Duration : _parts[i + 1].Timestamp;

					UpdateCurrentAction?.Invoke($"[{i + 1}/{_parts.Count}] Splitting \"{_getFileName(_parts[i])}\"");
					using (MemoryStream partMemoryStream = await Ffmpeg.SplitMp3Stream(ms, _parts[i].Timestamp, end, this))
					{
						_writeTags(partMemoryStream, _parts[i]);
						await _writeFile(partMemoryStream, Path.Combine(folderPath, _getFileName(_parts[i])));
					}
				}
			}

			UpdateCurrentAction?.Invoke($"[{_parts.Count}/{_parts.Count}] Done");
			UpdateCurrentProgress?.Invoke(1);
		}

		private async Task _writeFile(Stream ms, String filePath)
		{ 
			using (StreamWriter sw = new StreamWriter(filePath))await ms.CopyToAsync(sw.BaseStream);
			ms.Position = 0;
		}

		private void _writeTags(MemoryStream ms, Part part)
		{
			TagLib.File file = TagLib.File.Create(new FileStreamAbstraction("file.mp3", ms));
			file.Tag.Title = part.Title;
			file.Tag.Performers = new[] { part.Author };
			file.Tag.Album = part.Album;
			file.Save();

			ms.Position = 0;
		}

		private String _getFileName(Part part)
		{
			String fileName = String.IsNullOrEmpty(part.Author) ? part.Title : $"{part.Author} - {part.Title}";

			return String.Join("_", fileName.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.') + ".mp3";
		}

		public void Report(Double progress)
		{
			UpdateCurrentProgress?.Invoke(progress);
		}
	}
}
