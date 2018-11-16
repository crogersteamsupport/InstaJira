using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Data;
using Newtonsoft.Json;

namespace TeamSupport.UnitTest
{
    class ObjectCompare
    {
        public static bool AreEqual(DataTable dt1, DataTable dt2)
        {
            int? rowNumIndex = null;
            string[] columns = dt1.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToArray();
            for(int i = 0; i < columns.Length; ++i)
            {
                if (columns[i] != "RowNum")
                    continue;

                rowNumIndex = i;
                break;
            }

            string first = AsJson(dt1, rowNumIndex);
            string second = AsJson(dt2, rowNumIndex);
            bool result = first == second;
            if (!result)
            {
                System.IO.File.WriteAllText(@"one.json", first);    // use a diff tool to compare the files
                System.IO.File.WriteAllText(@"two.json", second);
                Debugger.Break();
            }
            return result;
        }

        private static string AsJson(DataTable dt, int? rowNumIndex)
        {
            // SQL ignores whitespace and is case insensitive in GROUP BY
            object[][] optimized = dt.AsEnumerable().Select(i => i.ItemArray).ToArray();
            foreach(object[] row in optimized)
            {
                for(int i = 0; i < row.Length; ++i)
                {
                    if (row[i] is string)
                        row[i] = ((string)row[i]).ToLower().TrimEnd().TrimStart(new char[] { '\uFEFF' });
                }
            }

            // toss RowNum
            if (rowNumIndex.HasValue)
            {
                foreach (var row in optimized)
                    row[rowNumIndex.Value] = -1;
            }

            SortRows(optimized);
            return JsonConvert.SerializeObject(optimized, Formatting.Indented);
        }

        public static void SortRows(object[] rows)
        {
            Array.Sort(rows, TypedCompare);
        }

        /// <summary>
        /// Can throw if different data types
        /// </summary>
        public static int TypedCompare(object one, object two)
        {
            // DBNull
            if (one is DBNull && two is DBNull)
                return 0;
            else if (one is DBNull)
                return -1;
            else if (two is DBNull)
                return 1;

            int result = 0;
            switch (one)
            {
                case object[] value:
                    // compare as array
                    result = TypedArrayCompare(value, (object[])two);
                    break;
                case DateTime value:
                    result = value.CompareTo((DateTime)two);
                    break;
                case int value:
                    result = value.CompareTo((int)two);
                    break;
                case long value:
                    result = value.CompareTo((long)two);
                    break;
                case string value:
                    result = value.CompareTo((string)two);
                    break;
                case bool value:
                    result = value.CompareTo((bool)two);
                    break;
                case decimal value:
                    result = value.CompareTo((decimal)two);
                    break;
                case null:
                default:
                    if (Debugger.IsAttached) Debugger.Break();
                    break;
            }
            return result;
        }

        public static int TypedArrayCompare(object[] one, object[] two)
        {
            int result = one.Length.CompareTo(two.Length);
            if (result != 0)
                return result;

            string tmp1 = String.Empty;
            string tmp2 = String.Empty;
            for (int i = 0; i < one.Length; ++i)
            {
                tmp1 += one[i].ToString();
                tmp2 += two[i].ToString();

                result = TypedCompare(one[i], two[i]);
                if (result != 0)
                    break;
            }

            return result;
        }

    }
}
