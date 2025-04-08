using ClosedXML.Excel;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace SolaBid.Extensions
{
    public static class Utility
    {
        public static IEnumerable<TSource> CustomDistinctBy<TSource, TKey>
        (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        public static DataTable ToDataTable<T>(this IList<T> data)
        {
            PropertyDescriptorCollection properties =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }

        public static void AddValue(this IXLWorksheet worksheet, int row, int col, string value, sbyte mergedColCount = -1)
        {
            if (value == "ü")
            {
                worksheet.Cell(row, col).Style.Font.FontName = "Wingdings";
                worksheet.Cell(row, col).Style.Font.FontSize = 12;
            }

            worksheet.Cell(row, col).Value = value;

            if (mergedColCount == 0)
            {
                worksheet.Cell(row, col).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            }
            else if (mergedColCount > 0)
            {
                worksheet.Range(row, col, row, col + mergedColCount).Merge();
                worksheet.Range(row, col, row, col + mergedColCount).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            }
        }

        public static string FormatDecimalWithSpace(this decimal value)
        {
            try
            {
                var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
                nfi.NumberGroupSeparator = " ";
                return value.ToString("#,0.00", nfi);
            }
            catch (Exception)
            {
                return "0";
            }
        }

        public static string GetDisplayAttribute(this object value)
        {
            Type type = value.GetType();
            var field = type.GetField(value.ToString());
            var result = field == null ? null : field.GetCustomAttribute<DisplayAttribute>();
            return result.Name;
        }

        public static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                    {
                        switch (column.DataType.Name)
                        {
                            case "DateTime":
                                pro.SetValue(obj, dr[column.ColumnName] == DBNull.Value ? null : Convert.ToDateTime(dr[column.ColumnName]).ToString("dd'/'MM'/'yyyy"), null);
                                break;
                            case "Decimal":
                                pro.SetValue(obj, dr[column.ColumnName] == DBNull.Value ? "0" : Convert.ToDecimal(dr[column.ColumnName]).FormatDecimalWithSpace(), null);
                                break;
                            default:
                                pro.SetValue(obj, dr[column.ColumnName] == DBNull.Value ?
                                    null : column.ColumnName == "RFQSuppliers" ?
                                    dr[column.ColumnName].ToString().TrimStart(';') : column.ColumnName == "PRLines" ?
                                    dr[column.ColumnName].ToString().TrimEnd(';') : dr[column.ColumnName], null);
                                break;
                        }
                    }
                    else
                        continue;
                }
            }
            return obj;
        }


        public static DateTime? ConvertHyphenStringToDatetime(string date)
        {
            if (date == null)
                return null;
            if (date.Length != 10)
            {
                try
                {
                    string tryDay = date.Substring(0, 2);
                    int.Parse(tryDay);
                }
                catch (Exception)
                {
                    date = "0" + date;
                }
                try
                {
                    string tryMonth = date.Substring(3, 2);
                    int.Parse(tryMonth);
                }
                catch (Exception)
                {
                    date = date.Insert(3, "0");
                }

            }


            int day = int.Parse(new StringSegment(date, 0, 2));
            int month = int.Parse(new StringSegment(date, 3, 2));
            int year = int.Parse(new StringSegment(date, 6, 4));
            DateTime convertedDate = new DateTime(year, month, day);
            return convertedDate;
        }
    }
}
