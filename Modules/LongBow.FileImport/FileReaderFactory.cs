using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using LongBow.FileImport.Enums;
using LongBow.FileImport.Interfaces;
using LongBow.FileImport.Objects;

namespace LongBow.FileImport
{
	[Export(typeof(IFileReaderFactory)), PartCreationPolicy(CreationPolicy.Shared)]
	public class FileReaderFactory : IFileReaderFactory
	{
		[ImportMany]
// ReSharper disable once UnassignedField.Compiler
		private IEnumerable<Lazy<IFileReader, IFileReaderData>> _allReaders;

		public List<BankPlugin> GetAvailableBankPlugins(string path)
		{
			var fileInfo = new FileInfo(path);

			var readers = _allReaders
				.Where(n => n.Metadata.Extension == fileInfo.Extension)
				.ToList();

			return readers
				.Where(n => n.Metadata.BankPlugin.HasValue)
				.Select(n => n.Metadata.BankPlugin.Value)
				.ToList();
		}

		public DataFromFile ReadFile(string path, out string error, BankPlugin? bankPlugin = null)
		{
			var fileInfo = new FileInfo(path);

			var readersRequest = _allReaders
				.Where(n => n.Metadata.Extension == fileInfo.Extension);

			if (bankPlugin != null)
				readersRequest = readersRequest.Where(n => n.Metadata.BankPlugin == bankPlugin);

			var readers = readersRequest.ToList();

			if (readers.Count != 1)
			{
				error = "Unable to determine reader";
				return null;
			}

			return readers[0].Value.ReadFile(path, out error);
		}
	}
}
