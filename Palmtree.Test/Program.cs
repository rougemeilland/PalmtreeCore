using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using Palmtree;
using Palmtree.Data;
using Palmtree.Collection;
using Palmtree.Threading;

namespace Palmtree.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var reader = new StreamReader(@"sample1.json"))
            {
                var o = SimpleJsonDeserializer.Deserialize(reader);
                Console.WriteLine(SimpleJsonSerializer.Serialize(o));
            }
            Console.ReadLine();
        }
    }
}
