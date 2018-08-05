using System;
using System.IO;
using static TagLib.File;

namespace Schrabber
{
	class FileStreamAbstraction : IFileAbstraction
	{
		public String Name { get; }
		public Stream ReadStream { get; }
		public Stream WriteStream { get; }

		public FileStreamAbstraction(String name, Stream stream)
		{
			Name = name;
			ReadStream = stream;
			WriteStream = stream;
		}

		public void CloseStream(Stream stream)
		{
			// Causes a StackOverflowException
			// The actual stream gets disposed everywhere in using statements anyway
			// stream.Dispose();
		}
	}
}
