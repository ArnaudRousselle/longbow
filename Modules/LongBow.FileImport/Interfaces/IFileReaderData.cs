using LongBow.FileImport.Enums;

namespace LongBow.FileImport.Interfaces
{
	public interface IFileReaderData
	{
		string Extension { get; }
		BankPlugin? BankPlugin { get; }
	}
}
