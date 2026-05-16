using System.Threading;
using Microsoft.ServiceFabric.Services.Runtime;

namespace FinanceService
{
    internal static class Program
    {
        private static void Main()
        {
            ServiceRuntime.RegisterServiceAsync("FinanceServiceType",
                context => new FinanceService(context)).GetAwaiter().GetResult();

            ServiceEventSource.Current.ServiceTypeRegistered(
                System.Diagnostics.Process.GetCurrentProcess().Id,
                typeof(FinanceService).Name);

            Thread.Sleep(Timeout.Infinite);
        }
    }
}