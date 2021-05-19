using System;
using System.IO;

namespace Schrabber.Workers
{
	public static class Cache
	{
		static Cache() => Directory.CreateDirectory(DownloadCacheFolder);

		public static readonly String DownloadCacheFolder = Path.Combine(
			Path.GetTempPath(),
			"Schrabber"
		);

		public static String GetTempCacheFilename() => Path.Combine(DownloadCacheFolder, Guid.NewGuid().ToString() + ".mp3");
		public static String CreateOutFolder(String name = null)
		{
			String path = Path.Combine(Path.GetTempPath(), name ?? DateTimeOffset.Now.ToUnixTimeSeconds().ToString());

			Directory.CreateDirectory(path);

			return path;
		}
	}
}
