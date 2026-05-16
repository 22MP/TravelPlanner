using System.Threading;
using Microsoft.ServiceFabric.Services.Runtime;

namespace BackendSF
{
    internal static class Program
    {
        private static void Main()
        {
            ServiceRuntime.RegisterServiceAsync("BackendSFType",
                context => new BackendSF(context)).GetAwaiter().GetResult();

            ServiceEventSource.Current.ServiceTypeRegistered(
                System.Diagnostics.Process.GetCurrentProcess().Id,
                typeof(BackendSF).Name);

            Thread.Sleep(Timeout.Infinite);
        }
    }
}