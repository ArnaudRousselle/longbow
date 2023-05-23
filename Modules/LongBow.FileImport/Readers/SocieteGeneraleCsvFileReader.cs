using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Text;
using LongBow.FileImport.Enums;
using LongBow.FileImport.Interfaces;
using LongBow.FileImport.Objects;

namespace LongBow.FileImport.Readers
{
	[Export(typeof(IFileReader))]
	[ExportMetadata("Extension", ".csv")]
	[ExportMetadata("BankPlugin", BankPlugin.SocieteGenerale)]
	public class SocieteGeneraleCsvFileReader : IFileReader
	{
		private readonly CultureInfo _cultureInfo = CultureInfo.GetCultureInfo("FR-fr");

		public DataFromFile ReadFile(string path, out string error)
		{
			var res = new DataFromFile
			{
				Lines = new List<DataLine>(),
			};

			var lineIndex = 0;

			try
			{
				using (var reader = new StreamReader(File.OpenRead(path), Encoding.GetEncoding(1252)))
				{
					string line;

					var dataLineStarted = false;

					while ((line = reader.ReadLine()) != null)
					{
						lineIndex++;

						if (dataLineStarted && !IsDataLine(line, lineIndex))
						{
							break;
						}

						if (!IsDataLine(line, lineIndex))
							continue;

						ReadDataLine(res, line, lineIndex);
						dataLineStarted = true;
					}

					reader.Close();
				}
			}
			catch (Exception ex)
			{
				error = "error on line " + lineIndex + " " + ex.Message;
				return null;
			}

			error = null;
			return res;
		}

		private bool IsDataLine(string line, int lineIndex)
		{
			return line != null
				&& lineIndex > 1
				&& line.Length >= 1
				&& line[0] >= '0'
				&& line[1] <= '9';
		}

		private void ReadDataLine(DataFromFile dataFromFile, string line, int lineIndex)
		{
			var tab = line.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

			var dataLine = new DataLine();

			dataLine.DtPosted = DateTime.Parse(tab[0]);

			dataLine.FitId = lineIndex.ToString();

			dataLine.Name = tab[2].Trim();

			dataLine.TrnAmt = double.Parse(tab[3], _cultureInfo);

			dataFromFile.Lines.Add(dataLine);
		}
	}
}
