using System;
using System.Windows.Media;

namespace SilverlightMappingToolBasic.Code.ColorsManagement
{
    public static class ColorConverter
    {
        public static Color FromHex(string hexaColor)
        {
            return Color.FromArgb(
                    Convert.ToByte(hexaColor.Substring(1, 2), 16),
                    Convert.ToByte(hexaColor.Substring(3, 2), 16),
                    Convert.ToByte(hexaColor.Substring(5, 2), 16),
                    Convert.ToByte(hexaColor.Substring(7, 2), 16));
        }
    }
}
