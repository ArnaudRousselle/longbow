using System.ComponentModel.Composition;

namespace LongBow.Common.Interfaces.File.Implementation
{
	[Export(typeof(IFileInfo))]
	public class FileInfo : IFileInfo
	{
		public bool Exists(string path)
		{
			return System.IO.File.Exists(path);
		}
	}
}
