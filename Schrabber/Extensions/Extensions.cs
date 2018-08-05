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
			CancellationToken cancellationToken = default(CancellationToken)
		)
		{
			Int64 totalBytesCopied = 0;
			Int32 bytesCopied;
			do
			{
				Byte[] buffer = new Byte[4096];
				bytesCopied = await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false);
				await destination.WriteAsync(buffer, 0, bytesCopied, cancellationToken).ConfigureAwait(false);
				totalBytesCopied += bytesCopied;
				progress?.Report((float)totalBytesCopied / source.Length);
			} while (bytesCopied > 0);
		}
	}
}
