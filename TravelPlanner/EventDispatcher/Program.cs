using System.Threading;
using Microsoft.ServiceFabric.Services.Runtime;

namespace EventDispatcher
{
    internal static class Program
    {
        private static void Main()
        {
            ServiceRuntime.RegisterServiceAsync("EventDispatcherType",
                context => new EventDispatcher(context)).GetAwaiter().GetResult();

            ServiceEventSource.Current.ServiceTypeRegistered(
                System.Diagnostics.Process.GetCurrentProcess().Id,
                typeof(EventDispatcher).Name);

            Thread.Sleep(Timeout.Infinite);
        }
    }
}