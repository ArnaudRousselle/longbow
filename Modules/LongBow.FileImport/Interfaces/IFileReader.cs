using LongBow.FileImport.Objects;

namespace LongBow.FileImport.Interfaces
{
	public interface IFileReader
	{
		DataFromFile ReadFile(string path, out string error);
	}
}
