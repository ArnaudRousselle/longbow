using System.Collections.Generic;
using LongBow.FileImport.Enums;
using LongBow.FileImport.Objects;

namespace LongBow.FileImport.Interfaces
{
	public interface IFileReaderFactory
	{
		List<BankPlugin> GetAvailableBankPlugins(string path);
		DataFromFile ReadFile(string path, out string error, BankPlugin? bankPlugin = null);
	}
}
