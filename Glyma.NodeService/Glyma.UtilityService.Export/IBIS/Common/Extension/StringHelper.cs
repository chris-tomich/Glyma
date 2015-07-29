using System;
using System.Text;

namespace Glyma.UtilityService.Export.IBIS.Common.Extension
{
    public static class StringHelper
    {
        private static Random _random;

        private static Random Random
        {
            get
            {
                if (_random == null)
                {
                    _random = new Random((int) DateTime.Now.Ticks);
                }
                return _random;
            }
        }

        public static string RandomString(int size)
        {
            if (size > 0)
            {
                var builder = new StringBuilder();
                char ch;
                for (var i = 0; i < size; i++)
                {
                    ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * Random.NextDouble() + 65)));
                    builder.Append(ch);
                }
                return builder.ToString();
            }
            return string.Empty;
        }
    }
}
