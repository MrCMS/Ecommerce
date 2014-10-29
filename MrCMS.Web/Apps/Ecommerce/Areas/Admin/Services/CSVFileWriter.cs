using System;
using System.Collections.Generic;
using System.IO;
using CsvHelper;
using MrCMS.Web.Apps.Ecommerce.Stock.Entities;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class CSVFileWriter : ICSVFileWriter
    {
        public byte[] GetFile<T>(IEnumerable<T> items, Dictionary<string, Func<T, object>> columns)
        {
            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            using (var csvWriter = new CsvWriter(streamWriter))
            {
                WriteHeaders(csvWriter, columns);

                foreach (var item in items)
                {
                    WriteItem(csvWriter, columns, item);
                }

                streamWriter.Flush();
                return memoryStream.ToArray();
            }
        }

        private void WriteItem<T>(CsvWriter csvWriter, Dictionary<string, Func<T, object>> columns, T item)
        {
            foreach (var pair in columns)
            {
                csvWriter.WriteField(pair.Value(item));
            }
            csvWriter.NextRecord();
        }

        private void WriteHeaders<T>(CsvWriter csvWriter, Dictionary<string, Func<T, object>> columns)
        {
            foreach (var pair in columns)
            {
                csvWriter.WriteField(pair.Key);
            }
            csvWriter.NextRecord();
        }
    }
}