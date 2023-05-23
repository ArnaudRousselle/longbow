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
	[ExportMetadata("BankPlugin", BankPlugin.CaisseEpargne)]
	public class CaisseEpargneCsvFileReader : IFileReader
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

						if (dataLineStarted && !IsDataLine(line))
						{
							break;
						}

						if (!IsDataLine(line))
						{
							ReadInformationLine(res, line);
						}
						else
						{
							ReadDataLine(res, line);
							dataLineStarted = true;
						}
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

		private bool IsDataLine(string line)
		{
			return line != null
				&& line.Length >= 1
				&& line[0] >= '0'
				&& line[1] <= '9';
		}

		private void ReadInformationLine(DataFromFile dataFromFile, string line)
		{
			string value;

			if (dataFromFile.BankId == null
				&& SearchForTagAndGetValue("Code de la banque :", line, out value))
				dataFromFile.BankId = value;

			if (dataFromFile.BranchId == null
				&& SearchForTagAndGetValue("Code de l'agence :", line, out value))
				dataFromFile.BranchId = value;

			if (dataFromFile.AcctId == null
				&& SearchForTagAndGetValue("Numéro de compte :", line, out value))
				dataFromFile.AcctId = value;

			if (dataFromFile.CurDef == null
				&& SearchForTagAndGetValue("Devise :", line, out value))
				dataFromFile.CurDef = value;
		}

		private bool SearchForTagAndGetValue(string tag, string line, out string value)
		{
			var startIndex = line.IndexOf(tag, StringComparison.InvariantCulture);

			if (startIndex < 0)
			{
				value = null;
				return false;
			}

			startIndex += tag.Length;

			var endIndex = line.IndexOf(';', startIndex);

			if (endIndex < 0)
			{
				value = null;
				return false;
			}

			value = line.Substring(startIndex, endIndex - startIndex).Trim();
			return true;
		}

		private void ReadDataLine(DataFromFile dataFromFile, string line)
		{
			var tab = line.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);

			var dataLine = new DataLine();

			dataLine.DtPosted = DateTime.Parse(tab[0]);

			dataLine.FitId = tab[1].Trim();

			dataLine.Name = tab[2].Trim();

			dataLine.TrnAmt = tab[3].Trim() != ""
				? double.Parse(tab[3], _cultureInfo)
				: double.Parse(tab[4], _cultureInfo);

			dataFromFile.Lines.Add(dataLine);
		}

	}
}
