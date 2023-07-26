using Avalonia.Controls;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMapEditor.Helpers
{
    public static class Extentions
    {
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

    }
}
