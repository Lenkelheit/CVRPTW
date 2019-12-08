using Ganss.Excel;
using OfficeOpenXml;

using Domains.Models.Input;
using Domains.Models.Output;

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace API.Services
{
    public class FileService
    {
        public FileInput Parse(Stream fileStream)
        {
            ExcelMapper excelMapper = new ExcelMapper(fileStream);

            return new FileInput
            {
                Distances = excelMapper.Fetch<Distances>(nameof(Distances)).ToArray(),
                Locations = excelMapper.Fetch<Locations>(nameof(Locations)).ToArray(),
                Vehicles = excelMapper.Fetch<Vehicles>(nameof(Vehicles)).ToArray()
            };
        }

        #region Save
        public byte[] Save(FileOutput fileOutput)
        {
            using (ExcelPackage excel = new ExcelPackage())
            {
                AddWorksheet(fileOutput.Summaries, excel);
                AddWorksheet(fileOutput.Totals, excel);
                AddWorksheet(fileOutput.Itineraries, excel);
                AddWorksheet(fileOutput.DroppedLocation, excel);

                return excel.GetAsByteArray();
            }
        }

        private void AddWorksheet<TModel>(IList<TModel> results, ExcelPackage excel)
        {
            ExcelWorksheet workSheet = excel.Workbook.Worksheets.Add(typeof(TModel).Name);

            workSheet.Cells[1, 1].LoadFromCollection(results, PrintHeaders: true);
            workSheet.Cells[workSheet.Dimension.Address].AutoFitColumns();
            ConfigureFormats(workSheet, results);
        }

        private void ConfigureFormats<TModel>(ExcelWorksheet workSheet, IList<TModel> results)
        {
            typeof(TModel).GetProperties()
                .Select((v, i) => new { v.PropertyType, index = i })
                .Where(a => a.PropertyType == typeof(DateTime))
                .Select(a => a.index)
                .ToList()
                .ForEach(colIndex => workSheet.Cells[FromRow: 2, ToRow: results.Count + 1, FromCol: colIndex + 1, ToCol: colIndex + 1].Style.Numberformat.Format = "HH:mm:ss");
        }
        #endregion
    }
}
