﻿using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Schrabber.Extensions;

namespace Schrabber
{
	internal static class Ffmpeg
	{
		internal static Process GetProcess(String arguments, Boolean stdin = false, Boolean stdout = false)
		{
			return new Process()
			{
				StartInfo =
				{
					FileName = "ffmpeg.exe",
					Arguments = arguments,
					UseShellExecute = false,
					CreateNoWindow = true,
					RedirectStandardInput = stdin,
					RedirectStandardOutput = stdout,
				}
			};
		}

		public static async Task<MemoryStream> SplitMp3Stream(
			Stream input,
			TimeSpan? start,
			TimeSpan? stop,
			IProgress<Double> progress = null,
			CancellationToken token = default(CancellationToken)
		)
		{
			MemoryStream output = new MemoryStream();
			String arguments = "-i - -acodec copy ";
			if (start != null) arguments += $"-ss {start.Value.TotalSeconds} ";
			if (stop != null) arguments += $"-to {stop.Value.TotalSeconds} ";
			arguments += "-f mp3 -";

			await _run(arguments, input, output, progress, token).ConfigureAwait(false);
			return output;
		}

		public static async Task<MemoryStream> RunDuplex(
			String args,
			Stream input,
			IProgress<Double> progress = null,
			CancellationToken token = default(CancellationToken)
		)
		{
			MemoryStream output = new MemoryStream();
			await _run(args, input, output, progress, token).ConfigureAwait(false);
			return output;
		}

		private static async Task _run(String args, Stream input, Stream output, IProgress<Double> progress, CancellationToken token)
		{
			using (Process ffmpeg = GetProcess(args, input != null, output != null))
			{
				ffmpeg.Start();

				Stream ffmpegIn = ffmpeg.StandardInput.BaseStream;
				Stream ffmpegOut = ffmpeg.StandardOutput.BaseStream;

				Task copyFfmpegOutTask = output != null ? ffmpegOut.CopyToAsync(output, token: token) : null;

				if (input != null)
				{

					try { await input.CopyToAsync(ffmpegIn, progress, token); }
					catch (IOException)
					{
						// ffmpeg seems to close the pipe as soon it finished seeking
						// no clue how to avoid the exception here
						if (!args.Contains("-to")) throw;
					}

					await ffmpegIn.FlushAsync(token);
					ffmpegIn.Close();

					if (input.CanSeek) input.Position = 0;
				}

				if (output != null)
				{
					await copyFfmpegOutTask;
					await ffmpegOut.FlushAsync(token);
					ffmpegOut.Close();

					if (output.CanSeek) output.Position = 0;
				}


				ffmpeg.WaitForExit();
			}
		}
	}
}
