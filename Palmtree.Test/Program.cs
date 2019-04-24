using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Palmtree;
using Palmtree.Threading;

namespace Palmtree.Test
{
    class Program
    {
        static AutoResetEvent ev1 = new AutoResetEvent(false);
        static AutoResetEvent ev2 = new AutoResetEvent(false);
        static CancellationTokenSource cts = new CancellationTokenSource();

        static void Main(string[] args)
        {
            var t = ThreadFunc(cts.Token);
            Console.ReadLine();
            ev1.Set();
            Console.ReadLine();
            cts.Cancel();
            //ev2.Set();
            Console.ReadLine();
        }

        static async Task ThreadFunc(CancellationToken ct)
        {
            try
            {
                Console.WriteLine("P1");
                await ev1.WaitOneAsync(ct);
                Console.WriteLine("P2");
                await ev2.WaitOneAsync(ct);
                Console.WriteLine("P3");
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
