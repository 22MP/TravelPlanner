using System;
using System.Threading;
using Microsoft.ServiceFabric.Services.Runtime;

namespace TravelService
{
    internal static class Program
    {
        private static void Main()
        {
            ServiceRuntime.RegisterServiceAsync("TravelServiceType",
                context => new TravelService(context)).GetAwaiter().GetResult();

            ServiceEventSource.Current.ServiceTypeRegistered(
                System.Diagnostics.Process.GetCurrentProcess().Id,
                typeof(TravelService).Name);

            Thread.Sleep(Timeout.Infinite);
        }
    }
}