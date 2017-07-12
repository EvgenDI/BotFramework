using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Bot_Application1.Dialogs
{
    public class ThreadRandom
    {
        private static Random global = new Random();

        [ThreadStatic]
        private static Random local;

        public static int Next()
        {
            Random inst = local;
            if (inst == null)
            {
                int seed;
                lock (global)
                {
                    seed = global.Next(1000,10000);
                }

                local = inst = new Random(seed);
            }

            return inst.Next(1000, 10000);
        }

        public static  Task<int> getRandom()
        {
            return  Task.FromResult(ThreadRandom.Next());
        }


    }
}