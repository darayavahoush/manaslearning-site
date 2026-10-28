using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;

namespace manaslearning.Helpers
{
    public static class ExtensionMethods
    {
        public static string ToWords(this int number)
        {
            if (number == 0) { return "zero"; }
            if (number < 0) { return "minus " + ToWords(Math.Abs(number)); }
            string words = "";
            if ((number / 10000000) > 0)
            {
                words += ToWords(number / 10000000) + " Crore "; number %= 10000000;
            }
            if ((number / 100000) > 0)
            {
                words += ToWords(number / 100000) + " Lakh "; number %= 100000;
            }
            if ((number / 1000) > 0)
            {
                words += ToWords(number / 1000) + " Thousand "; number %= 1000;
            }
            if ((number / 100) > 0)
            {
                words += ToWords(number / 100) + " Hundred "; number %= 100;
            }
            if (number > 0)
            {
                if (words != "") { words += "and "; }
                var unitsMap = new[] { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
                var tensMap = new[] { "Zero", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "seventy", "Eighty", "Ninety" };
                if (number < 20) { words += unitsMap[number]; }
                else { words += tensMap[number / 10]; if ((number % 10) > 0) { words += "-" + unitsMap[number % 10]; } }
            }
            return words;
        }

        public static DataTable ConvertToDataTable<T>(this IEnumerable<T> data)
        {            
            DataTable table = new DataTable();
            if (typeof(T).IsPrimitive || typeof(T) == typeof(string))
            {
                table.Columns.Add("Value", typeof(T));
                foreach (T item in data)
                {
                    table.Rows.Add(item);
                }
            }
            else
            {
                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
                foreach (PropertyDescriptor prop in properties)
                    table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);

                foreach (T item in data)
                {
                    DataRow row = table.NewRow();
                    foreach (PropertyDescriptor prop in properties)
                        row[prop.Name] = !string.IsNullOrEmpty(prop.GetValue(item)?.ToString()) ? prop.GetValue(item) : DBNull.Value;
                    table.Rows.Add(row);
                }
            }      
            return table;
        }

        public static void For<T>(this IEnumerable<T> sequence, Action<T> action)
        {
            if (sequence != null)
            {
                List<T> list = sequence.Any() ? sequence.ToList() : null;
                if (list != null)
                {
                    for (int initializer = 0; initializer < list.Count(); initializer++)
                    {
                        T item = (T)list[initializer];
                        action(item);
                    }
                }
            }
        }
    }
}
