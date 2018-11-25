using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Schrabber.Workers
{
	public static class FFmpeg
	{
		public static String FindExecutablePath()
		{
			using (Process process = new Process()
			{
				StartInfo =
				{
					UseShellExecute=false,
					FileName="where",
					Arguments="ffmpeg.exe",
					RedirectStandardOutput=true,
				}
			})
			{
				process.Start();
				String res = process.StandardOutput.ReadToEnd();
				process.WaitForExit();

				if (process.ExitCode != 0)
					return null;

				return res.Substring(0, res.IndexOf(Environment.NewLine));
			}
		}

		// https://github.com/Tyrrrz/YoutubeExplode.Converter/blob/master/YoutubeExplode.Converter/public/FfmpegCli.cs
		public static void Split(String source, String dest, TimeSpan? start, TimeSpan? stop, IProgress<Double> progress)
		{
			List<String> arguments = new List<String>
			{
				$"-i \"{source}\""
			};

			if (start.HasValue && start.Value.TotalSeconds != 0) arguments.Add($"-ss {start.Value.TotalSeconds}");

			if (stop.HasValue && stop.Value.TotalSeconds != 0) arguments.Add($"-to {stop.Value.TotalSeconds}");

			arguments.Add("-c copy");
			arguments.Add("-nostdin");
			arguments.Add("-y");
			arguments.Add($"\"{dest}\"");

			Debug.WriteLine($"Spawning ffmpeg with {String.Join(" ", arguments)}");

			using (Process ffmpeg = FFmpeg.GetProcess(String.Join(" ", arguments)))
			{
				if (progress != null)
				{
					TimeSpan duration = stop.HasValue ?
						stop.Value - start ?? TimeSpan.Zero
						:TimeSpan.Zero;
					ffmpeg.ErrorDataReceived += (sender, args) =>
					{
						if (args.Data == null) return;

						if (duration == TimeSpan.Zero)
						{
							String match = Regex.Match(args.Data, @"Duration:\s(\d\d:\d\d:\d\d.\d\d)").Groups[1].Value;
							if (String.IsNullOrWhiteSpace(match)) return;

							duration = TimeSpan.ParseExact(match, "c", CultureInfo.InvariantCulture);

							if (start.HasValue) duration -= start.Value;
						}
						else
						{
							String match = Regex.Match(args.Data, @"time=(\d\d:\d\d:\d\d.\d\d)").Groups[1].Value;
							if (String.IsNullOrWhiteSpace(match)) return;

							TimeSpan current = TimeSpan.ParseExact(match, "c", CultureInfo.InvariantCulture);

							progress.Report(Math.Min(current.TotalMilliseconds / duration.TotalMilliseconds, 1D));
						}
					};
				}

				ffmpeg.Start();
				ffmpeg.BeginErrorReadLine();

				ffmpeg.WaitForExit();

				if (ffmpeg.ExitCode != 0)
					throw new Exception($"FFmpeg exit code was {ffmpeg.ExitCode}.");
			}
		}

		public static Process GetProcess(String arguments)
		{
			return new Process()
			{
				StartInfo =
				{
					FileName = "ffmpeg.exe",
					Arguments = arguments,
					UseShellExecute = false,
					CreateNoWindow = true,
					RedirectStandardError = true,
				}
			};
		}
	}
}
