using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.Utilities
{
    public static class StringHelper
    {
        public static string ToCamelCases(this string str)
        {
            return str.Substring(0, 1).ToLower() + str.Substring(1);
        }
    }
}
