using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMStudio.Models
{
    public class ItemPropertiesModel
    {
        public int Type { get; set; }
        public string Name { get; set; }
        public int Index { get; set; }
        public object Value { get; set; }
        public object Items { get; set; }
       
    }
}
