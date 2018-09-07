using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Models;
using YoutubeExplode.Models.MediaStreams;

namespace Schrabber.Helpers
{
	internal class YouTubeClient
	{
		private static readonly YoutubeClient _youTubeClient = new YoutubeClient();

		public static Task<Video> GetVideoAsync(String videoId) => _youTubeClient.GetVideoAsync(videoId);
		public static Task<Playlist> GetPlaylistAsync(String playlistId) => _youTubeClient.GetPlaylistAsync(playlistId);

		public static async Task<MemoryStream> DownloadYouTubeVideoMp3MemoryStreamAsync(
			String videoId,
			TimeSpan? start = null,
			IProgress<Double> progress = null,
			CancellationToken token = default(CancellationToken)
		)
		{
			MediaStreamInfoSet info = await _youTubeClient.GetVideoMediaStreamInfosAsync(videoId).ConfigureAwait(false);
			AudioStreamInfo audioStreamInfo = info.Audio.WithHighestBitrate()
				?? throw new Exception($"No audio stream found for {videoId}");

			if (audioStreamInfo.Container.GetFileExtension() == "mp3" && start == TimeSpan.FromSeconds(0))
			{
				MemoryStream ms = new MemoryStream();

				await _youTubeClient.DownloadMediaStreamAsync(audioStreamInfo, ms, progress, token).ConfigureAwait(false);

				ms.Position = 0;
				return ms;
			}

			String arguments = "-i - -f mp3 -";

			if (start != TimeSpan.FromSeconds(0)) arguments = $"-ss {start.ToString()} {arguments}";

			return await FFmpeg.RunDuplex(
				arguments,
				await _youTubeClient.GetMediaStreamAsync(audioStreamInfo).ConfigureAwait(false),
				progress,
				token
			).ConfigureAwait(false);
		}
	}
}
