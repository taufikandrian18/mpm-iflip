using Abp.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MPM.FLP.Utilities
{
    public static class DictionaryHelper
    {
        public static IDictionary<string, string> AsDictionary(this object source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance, string prefix = "", bool isCamelCase = true, bool removeNullProperties = false)
        {
            if (source == null)
                return new Dictionary<string, string> {
                {"",""}
            };

            Dictionary<string, string> dict = source.GetType().GetProperties(bindingAttr).Where(x => x.GetValue(source, null) != null || !removeNullProperties).ToDictionary
            (
                propInfo => isCamelCase ? (prefix + propInfo.Name).ToCamelCase() : prefix + propInfo.Name,
                propInfo => propInfo.GetValue(source, null).GetSafeStringValue(propInfo.Name)
            );

            return dict;
        }


        public static string GetSafeStringValue(this object obj, string fieldName)
        {
            if (obj == null)
                return "";

            if (obj is DateTime)
                return GetStringValue((DateTime)obj);

            // More specical convert...

            //if (SPECIAL_FILTER_DICT.ContainsKey(fieldName))
            //    return SPECIAL_FILTER_DICT[fieldName];

            // Override ToString() method if needs
            return obj.ToString();
        }


        private static string GetStringValue(DateTime dateTime)
        {
            return dateTime.ToString("dd-MMM-yyyy");
        }
    }
}
