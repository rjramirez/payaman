using OfficeOpenXml;
using System.ComponentModel;
using System.Reflection;
using WebApp.Services.Interfaces;

namespace WebApp.Services
{
    public class ExportService : IExportService
    {
        public async Task<byte[]> ConvertToExcelByte<TSource>(IEnumerable<TSource> reportResults)
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using ExcelPackage excelPackage = new ExcelPackage();
            using ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");

            int excelRow = 1;
            int excelColumn = 1;
            PropertyInfo[] columnProps = typeof(TSource).GetProperties();
            foreach (PropertyInfo columnProp in columnProps)
            {
                DisplayNameAttribute displayName = columnProp.GetCustomAttributes(typeof(DisplayNameAttribute), true).Cast<DisplayNameAttribute>().SingleOrDefault();
                if (displayName != null)
                {
                    worksheet.Cells[1, excelColumn].Value = displayName.DisplayName;
                    worksheet.Cells[1, excelColumn].Style.Font.Bold = true;
                    excelColumn++;
                }
            }

            excelColumn = 1;
            excelRow++;
            foreach (TSource reportResult in reportResults)
            {
                Type typeParameterType = reportResult.GetType();
                PropertyInfo[] propValues = typeParameterType.GetProperties();
                foreach (PropertyInfo propValue in propValues)
                {
                    DisplayNameAttribute displayName = propValue.GetCustomAttributes(typeof(DisplayNameAttribute), true).Cast<DisplayNameAttribute>().SingleOrDefault();
                    if (displayName != null)
                    {
                        if (propValue.PropertyType == typeof(string))
                        {
                            string value = typeParameterType.GetProperty(propValue.Name).GetValue(reportResult, null)?.ToString();
                            worksheet.Cells[excelRow, excelColumn].Value = value;
                        }
                        else
                        {
                            IEnumerable<string> value = (IEnumerable<string>)typeParameterType.GetProperty(propValue.Name).GetValue(reportResult, null);
                            worksheet.Cells[excelRow, excelColumn].Value = string.Join("|", value);
                        }
                        excelColumn++;
                    }
                }
                excelRow++;
                excelColumn = 1;
            }

            worksheet.Cells.AutoFitColumns();
            return await excelPackage.GetAsByteArrayAsync();
        }
    }
}