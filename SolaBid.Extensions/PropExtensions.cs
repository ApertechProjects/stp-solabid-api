using Microsoft.Extensions.Primitives;
using System;

namespace SolaBid.Extensions
{
    public static class PropExtensions
    {
        public static DateTime ConvertStringToDatetime(this string date)
        {
            //try
            //{
            //    var splittedData = str.Split('.');
            //    var s1 = int.Parse(splittedData[0]);
            //    var s2 = int.Parse(splittedData[1]);
            //    var s3 = int.Parse(splittedData[2]);
            //    return new DateTime(s3, s2, s1);
            //}
            //catch (Exception)
            //{
            //    return DateTime.Now;
            //}
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
