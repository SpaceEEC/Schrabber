// https://github.com/Tyrrrz/YoutubeExplode/blob/88b88d7a337e450d25d5e65a8642637f968eb9fd/YoutubeExplode/Internal/Extensions.cs#L209

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Schrabber.Extensions
{
	internal static class Extensions
	{
		public static async Task CopyToAsync(
			this Stream source,
			Stream destination,
			IProgress<double> progress = null,
			CancellationToken token = default(CancellationToken)
		)
		{
			Byte[] buffer = new Byte[4096];

			Int64 totalBytesCopied = 0;
			Int32 bytesCopied;
			do
			{
				bytesCopied = await source.ReadAsync(buffer, 0, buffer.Length, token).ConfigureAwait(false);
				await destination.WriteAsync(buffer, 0, bytesCopied, token).ConfigureAwait(false);
				totalBytesCopied += bytesCopied;
				progress?.Report((float)totalBytesCopied / source.Length);
			} while (bytesCopied > 0);
		}
	}
}
