using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMFormat.Formats;

namespace TMStudio.Models
{
    public class TileModel
    {
        public int X { set; get; }
        public int Y { set; get; }
        public int Z { set; get; }
        public TMSprite Tile { set; get; }
        public List<TMSprite> Items { set; get; }

        public TileModel() {
            Items = new List<TMSprite>();
        }
    }
}
