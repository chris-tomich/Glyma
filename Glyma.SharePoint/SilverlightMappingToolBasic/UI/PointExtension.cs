using System.Windows;

namespace SilverlightMappingToolBasic.UI
{
    public static class PointExtension
    {
        public static bool IsMovedALot(Point originLocation, Point newLocation, out Point dif)
        {
            dif = originLocation.Subtract(newLocation);
            if (dif.X > 400 || dif.X < -400 || dif.Y > 400 || dif.Y < -400)
            {
                return true;
            }
            return false;
        }
    }
}
