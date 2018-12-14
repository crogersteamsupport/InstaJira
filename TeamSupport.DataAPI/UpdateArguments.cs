using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.DataAPI
{
    /// <summary>
    /// Simple class converting name-value pairs into an update query
    /// </summary>
    public class UpdateArguments
    {
        public static string ToSql(DateTime dateTime) { return dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff"); }
        public static string ToSql(bool value) { return value ? "1" : "0"; }

        public string Args { get; private set; }

        public UpdateArguments() { }
        public void Append(string name, int value) { PrivateAppend(name, value.ToString()); }
        public void Append(string name, DateTime value) { PrivateAppend(name, ToSql(value)); }
        public void Append(string name, bool value) { PrivateAppend(name, ToSql(value)); }
        public void Append(string name, string value) { PrivateAppend(name, '\'' + value + '\''); }

        public UpdateArguments(string name, int value)
        {
            Args = $"{name}={value}";
        }

        // do NOT allow inlining strings - they MUST be sql injection checked by the DB
        private void PrivateAppend(string name, string value)
        {
            if (String.IsNullOrEmpty(Args))
                Args = $"{name}={value}";
            else
                Args += $", {name}={value}";
        }

        public override string ToString()
        {
            return Args;
        }
    }

}
