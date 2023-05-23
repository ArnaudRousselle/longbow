namespace LongBow.Common.Interfaces.File
{
	public class FileItem
	{
		public string ShortName { get; set; }
		public string FilePath { get; set; }

		public string CompleteName
		{
			get { return string.Format("{0} ({1})", ShortName, FilePath); }
		}
	}
}
