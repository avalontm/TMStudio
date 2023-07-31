using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMFormat.Formats;

namespace TMapEditor.Models
{
    public class GroupSprites
    {
        public List<TMSprite> Items { set; get; }

        public GroupSprites()
        {
            Items = new List<TMSprite>();
        }
    }
}
