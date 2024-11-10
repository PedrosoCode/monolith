using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monolith
{
    class Utils
    {
        public static int? ParseNullableInt(object value)
        {
            if (value == null)
                return null;

            if (int.TryParse(value.ToString(), out int result))
                return result;

            return null;
        }

        public static string ParseNullableString(object value)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return null;

            return value.ToString().Trim();
        }

    }
}
