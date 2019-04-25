using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using Palmtree;
using Palmtree.Collection;
using Palmtree.Threading;

namespace Palmtree.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var pattern = new Regex(@"""(?<id>[a-zA-Z_][a-zA-Z0-9_]*?)""");
            Console.WriteLine(string.Join(", ", pattern.Matches(@"""aa"" ""05"" ""0a"" ""a5""").AsEnumerable(item => (Match)item).Select(item => item.Value)));
            Console.ReadLine();
        }
    }
}
