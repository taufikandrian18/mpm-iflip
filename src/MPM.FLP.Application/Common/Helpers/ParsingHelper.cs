using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.Common.Helpers
{
    public class ParsingHelper
    {
        public static bool TryParseNullableDateTime(string text, out DateTime? result)
        {
            DateTime value;
            var isValid = true;

            if (string.IsNullOrEmpty(text))
            {
                result = null;
                return isValid;
            }

            isValid = DateTime.TryParse(text, out value);
            result = isValid ? value : (DateTime?)null;

            return isValid;
        }
    }
}
