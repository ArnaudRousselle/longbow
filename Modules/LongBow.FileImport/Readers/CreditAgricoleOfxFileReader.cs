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
    [ExportMetadata("Extension", ".ofx")]
    [ExportMetadata("BankPlugin", BankPlugin.CreditAgricole)]
    public class CreditAgricoleOfxFileReader : IFileReader
    {
        public DataFromFile ReadFile(string path, out string error)
        {
            var res = new DataFromFile
            {
                Lines = new List<DataLine>(),
            };

            var lineIndex = 0;
            var dataStr = new StringBuilder();
            var dataStarted = false;
            string currentAccountId = string.Empty;
            var creditCardData = false;

            try
            {
                using (var reader = new StreamReader(File.OpenRead(path), Encoding.GetEncoding(1252)))
                {
                    string line;

                    while ((line = reader.ReadLine()) != null)
                    {
                        if (!dataStarted && line.Contains("<ACCTID>"))
                        {
                            currentAccountId = line.Replace("<ACCTID>", "");
                        }

                        if (!dataStarted && line.Contains("<CREDITCARDMSGSRSV1>"))
                        {
                            creditCardData = true;
                        }

                        if (!dataStarted && line.Contains("</CREDITCARDMSGSRSV1>"))
                        {
                            creditCardData = false;
                        }

                        if (line.Contains("<STMTTRN>"))
                        {
                            dataStarted = true;
                        }

                        if (dataStarted)
                        {
                            dataStr.Append(line.Trim());
                        }

                        if (dataStarted && line.Contains("</STMTTRN>"))
                        {
                            var str = dataStr.ToString();

                            res
                                .Lines
                                .Add(new DataLine
                                     {
                                         AccountId = currentAccountId,
                                         DtPosted =
                                            creditCardData
                                            ? DateTime.ParseExact(TryGetValue<string>(str, "<FITID>").Substring(0, 8), "yyyyMMdd", CultureInfo.InvariantCulture)
                                             :DateTime.ParseExact(TryGetValue<string>(str, "<DTPOSTED>"), "yyyyMMdd", CultureInfo.InvariantCulture),
                                         FitId = TryGetValue<string>(str, "<FITID>"),
                                         Name = TryGetValue<string>(str, "<NAME>"),
                                         Memo = TryGetValue<string>(str, "<MEMO>"),
                                         TrnAmt = TryGetValue<double>(str, "<TRNAMT>", CultureInfo.InvariantCulture)
                                     });

                            dataStarted = false;

                            dataStr.Clear();
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

        private T TryGetValue<T>(string sourceStr, string tag, IFormatProvider formatProvider = null)
        {
            var indexOfTag = sourceStr.IndexOf(tag, StringComparison.Ordinal);

            if (indexOfTag < 0)
                return default(T);

            var dataStartIndex = indexOfTag + tag.Length;

            var dataEndIndex = sourceStr.IndexOf("<", dataStartIndex, StringComparison.Ordinal);

            if (dataEndIndex < 0)
                return default(T);

            var data = sourceStr.Substring(dataStartIndex, dataEndIndex - dataStartIndex);

            return (T)Convert.ChangeType(data, typeof(T), formatProvider);
        }
    }
}
