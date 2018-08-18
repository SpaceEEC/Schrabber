using Schrabber.Interfaces;
using System;

namespace Schrabber
{
	public class Part : IPart
	{
		public String Album { get; set; } = "";
		public String Author { get; set; } = "";
		public TimeSpan Start { get; set; } = TimeSpan.FromSeconds(0);
		public TimeSpan? Stop { get; set; } = null;
		public String Title { get; set; } = "";
	}
}
