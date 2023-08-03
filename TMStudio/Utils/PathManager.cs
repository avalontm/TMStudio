using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMStudio.Utils
{
    public static class PathManager
    {
        public readonly static string Root = System.IO.Directory.GetCurrentDirectory();
        public readonly static string Data = Path.Combine(Root, "data");
        public readonly static string Maps = Path.Combine(Data, "maps");
        public readonly static string Creatures = Path.Combine(Data, "creatures");
    }
}
