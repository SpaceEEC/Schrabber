using System;

namespace Schrabber
{
	public class Part
	{
		public TimeSpan Timestamp { get; set; }
		public String Title { get; set; } = "";
		public String Author { get; set; } = "";
		public String Album { get; set; } = "";
	}
}
