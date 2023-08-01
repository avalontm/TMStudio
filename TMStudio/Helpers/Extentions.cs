using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Converters;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMStudio.Views.MapPage;
using TMFormat.Enums;

namespace TMStudio.Helpers
{
    public static class Extentions
    {
        public static SolidColorBrush ToBrush(this string HexColorString)
        {
            if (string.IsNullOrEmpty(HexColorString))
            {
                return new SolidColorBrush();
            }

            if (HexColorString[0] == '#')
            {
                HexColorString = HexColorString.Remove(0, 1);
            }
            int color = Convert.ToInt32(HexColorString, 16);
            return new SolidColorBrush((uint)color);
        }

        public static Bitmap ToImage(this byte[] byteArray)
        {
            try
            {
                using (MemoryStream stream = new MemoryStream(byteArray))
                {
                   return new Bitmap(stream);

                }
            }
            catch
            {
                return null;
            }
        }

        public static Rect GetAbsolutePlacement(this Visual element, bool relativeToScreen = false)
        {
            var absolutePos = element.PointToScreen(new Avalonia.Point(0, 0));
            if (relativeToScreen)
            {
                return new Rect(absolutePos.X, absolutePos.Y, element.Bounds.Width, element.Bounds.Height);
            }
            var posMW = MapMainView.Instance.PointToScreen(new Avalonia.Point(0, 0));
            var _absolutePos = new Avalonia.Point(absolutePos.X - posMW.X, absolutePos.Y - posMW.Y);
            return new Rect(_absolutePos.X, _absolutePos.Y, element.Bounds.Width, element.Bounds.Height);
        }

    }
}
