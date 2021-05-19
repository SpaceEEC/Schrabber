using System.Net.Http;
using YoutubeExplode;

namespace Schrabber.Workers
{
	// https://github.com/Tyrrrz/YoutubeExplode/issues/530#issuecomment-825998438
	internal static class Youtube
	{
		private static YoutubeClient _client = null;

		public static YoutubeClient Client => _client ?? (_client = Youtube.CreateClient());

		private static YoutubeClient CreateClient()
		{
			var handler = new HttpClientHandler();
			var httpClient = new HttpClient(handler, true);
			handler.UseCookies = false;

			return new YoutubeClient(httpClient);
		}
	}
}
