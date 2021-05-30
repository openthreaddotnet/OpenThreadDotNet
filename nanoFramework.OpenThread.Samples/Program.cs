using nanoFramework.OpenThread.NCP;
using System;
using System.Diagnostics;
using System.Threading;

namespace nanoFramework.OpenThread.Samples
{
    public class Program
    {
        private static LoWPAN loWPAN = new LoWPAN("");

        public static void Main()
        {
            Debug.WriteLine("Hello from nanoFramework!");
            // Browse our samples repository: https://github.com/nanoframework/samples
            // Check our documentation online: https://docs.nanoframework.net/
            // Join our lively Discord community: https://discord.gg/gCyBu8T             
            loWPAN.Open();

            Thread.Sleep(Timeout.Infinite);
        }
    }
}
