using SixLabors.ImageSharp.ColorSpaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMStudio.Models
{
    public class MouseModel
    {
        public static MouseModel Zero = new MouseModel(0, 0);

        public float X { set; get; }
        public float Y { set; get; }

        public MouseModel(float x, float y)
        {
            X = x;
            Y = y;
        }   
    }
}
