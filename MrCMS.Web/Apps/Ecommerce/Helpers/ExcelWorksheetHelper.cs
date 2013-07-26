using System;
using OfficeOpenXml;

namespace MrCMS.Web.Apps.Ecommerce.Helpers
{
    public static class ExcelWorksheetHelper
    {
         public static bool HasValue(this ExcelWorksheet sheet, int row, int column)
         {
             return GetBaseValue(sheet,row,column) != null;
         }

         private static object GetBaseValue(ExcelWorksheet sheet, int row, int column)
         {
             return sheet.Cells[row, column] != null ? sheet.Cells[row, column].Value : null;
         }
         
         public static T GetValue<T>(this ExcelWorksheet sheet, int row, int column) where T : class
         {
             return HasValue(sheet, row, column)
                        ? Convert.ChangeType(GetBaseValue(sheet, row, column), typeof (T)) as T
                        : null;
         }
    }
}