using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftNPCs.Core.Pathing
{
    public static class PathManager
    {
        public static readonly List<Path> Paths = new();

        public static Path AddPath(out int index)
        {
            Path p = new();
            index = Paths.Count;
            Paths.Add(p);
            return p;
        }

        public static void RemovePath(int index) => Paths.RemoveAt(index);

        public static void RemovePath(Path p) => Paths.Remove(p);

        public static void ClearPaths() => Paths.Clear();

        public static Path GetPath(int index)
        {
            if (index >= 0 && index < Paths.Count)
                return Paths[index];
            return null;
        }

        public static bool TryGetPath(int index, out Path p)
        {
            p = GetPath(index);
            return p != null;
        }
    }
}
