using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;

namespace SolaBid.Extensions
{
    public static class ClassBuilderExtensions
    {
        public static string GetDataTableColumNames(this DataTable dataTable, string contentRootPath)
        {
            string columNames = string.Empty;
            foreach (DataColumn columnName in dataTable.Columns)
            {
                columNames += $"public {columnName.DataType} {columnName.ColumnName.Replace(" ", "").Replace("/", "")}" + " " + "{ get; set; }" + Environment.NewLine;
            }

            if (File.Exists(Path.Combine(contentRootPath, "ColNames.txt")))
                File.Delete(Path.Combine(contentRootPath, "ColNames.txt"));

            using (StreamWriter sw = new StreamWriter(Path.Combine(contentRootPath, "ColNames.txt"), true))
            {
                sw.WriteLine(columNames);
                sw.Close();
                sw.Dispose();
            }
            return columNames;
        }

        public static List<T> ConvertToClassListModel<T>(this DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }

        public static T ConvertToClassModel<T>(this DataTable dt)
        {
            if (dt.Rows.Count > 0)
            {
                return GetItem<T>(dt.Rows[0]);
            }
            else
            {
                return Activator.CreateInstance<T>();
            }
        }

        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName.Replace(" ", "").Replace("/", ""))
                    {
                        if (dr[column.ColumnName] != DBNull.Value)
                        {
                            pro.SetValue(obj, dr[column.ColumnName], null);
                        }
                    }
                    else
                        continue;
                }
            }
            return obj;
        }
    }
}
