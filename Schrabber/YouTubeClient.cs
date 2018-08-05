using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Schrabber.Extensions;
using YoutubeExplode;
using YoutubeExplode.Models;
using YoutubeExplode.Models.MediaStreams;
using System.Linq;

namespace Schrabber
{
	internal class YouTubeClient
	{
		private static readonly YoutubeClient _youTubeClient = new YoutubeClient();

		public static Task<Video> GetVideoAsync(String videoUrl)
		{
			return _youTubeClient.GetVideoAsync(videoUrl);
		}

		public static async Task<MemoryStream> DownloadMp3VideoMemoryStreamAsync(Video video, TimeSpan? start = null, IProgress<Double> progress = null)
		{
			MediaStreamInfoSet info = await _youTubeClient.GetVideoMediaStreamInfosAsync(video.Id);
			AudioStreamInfo audioStreamInfo = info.Audio.WithHighestBitrate() 
				?? throw new Exception($"No audio stream found for ({video.Id}) - {video.Title}");

			if (audioStreamInfo.Container.GetFileExtension() == "mp3" && start == TimeSpan.FromSeconds(0))
			{
				MemoryStream ms = new MemoryStream();

				await _youTubeClient.DownloadMediaStreamAsync(audioStreamInfo, ms, progress);

				ms.Position = 0;
				return ms;
			}

			String arguments = "-i - -f mp3 -";

			if (start != TimeSpan.FromSeconds(0)) arguments = $"-ss {start.ToString()} {arguments}";

			return await Ffmpeg.RunDuplex(arguments, await _youTubeClient.GetMediaStreamAsync(audioStreamInfo), progress).ConfigureAwait(false);
		}
	}
}
