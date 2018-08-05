using System;
using System.Diagnostics;
using System.IO;
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

		public static async Task<MemoryStream> SplitMp3Stream(Stream input, TimeSpan start, TimeSpan stop, IProgress<Double> progress = null)
		{
			MemoryStream output = new MemoryStream();
			await SplitMp3Stream(input, output, start, stop, progress).ConfigureAwait(false);
			return output;
		}
		public static Task SplitMp3Stream(Stream input, Stream output, TimeSpan start, TimeSpan stop, IProgress<Double> progress = null)
			=> _run($"-i - -acodec copy -ss {start.TotalSeconds} -to {stop.ToString()} -f mp3 -", input, output, progress);
		public static Task RunWrite(String args, Stream input, IProgress<Double> progress = null)
			=> _run(args, input, null, progress);
		public static async Task<MemoryStream> RunRead(String args, IProgress<Double> progress = null)
		{
			MemoryStream output = new MemoryStream();
			await RunRead(args, output, progress).ConfigureAwait(false);
			return output;
		}
		public static Task RunRead(String args, Stream output, IProgress<Double> progress = null)
			=> _run(args, null, output, progress);
		public static async Task<MemoryStream> RunDuplex(String args, Stream input, IProgress<Double> progress = null)
		{
			MemoryStream output = new MemoryStream();
			await RunDuplex(args, input, output, progress).ConfigureAwait(false);
			return output;
		}
		public static Task RunDuplex(String args, Stream input, Stream output, IProgress<Double> progress = null)
			=> _run(args, input, output, progress);

		private static async Task _run(String args, Stream input, Stream output, IProgress<Double> progress)
		{
			using (Process ffmpeg = GetProcess(args, input != null, output != null))
			{
				ffmpeg.Start();

				Stream ffmpegIn = ffmpeg.StandardInput.BaseStream;
				Stream ffmpegOut = ffmpeg.StandardOutput.BaseStream;

				Task copyFfmpegOutTask = output != null ? ffmpegOut.CopyToAsync(output) : null;

				if (input != null)
				{

					try { await input.CopyToAsync(ffmpegIn, progress); }
					catch (IOException)
					{
						// ffmpeg seems to close the pipe as soon it finished seeking
						// no clue how to avoid the exception here
						if (!args.Contains("-ss")) throw;
					}

					await ffmpegIn.FlushAsync();
					ffmpegIn.Close();

					if (input.CanSeek) input.Position = 0;
				}

				if (output != null)
				{
					await copyFfmpegOutTask;
					await ffmpegOut.FlushAsync();
					ffmpegOut.Close();

					if (output.CanSeek) output.Position = 0;
				}


				ffmpeg.WaitForExit();
			}
		}
	}
}
