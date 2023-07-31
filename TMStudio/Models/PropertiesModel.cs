using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMStudio.Models
{
    public class PropertiesModel
    {
        public string? Name { get; set; }
        public object? Value { get; set; }
        public object? Items { get; set; }
        public int Type { get; set; }
    }
}
