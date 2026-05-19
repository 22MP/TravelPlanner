using System;
using System.Threading;
using Microsoft.ServiceFabric.Services.Runtime;

namespace AuthService
{
    internal static class Program
    {
        private static void Main()
        {
            ServiceRuntime.RegisterServiceAsync("AuthServiceType",
                context => new AuthService(context)).GetAwaiter().GetResult();

            ServiceEventSource.Current.ServiceTypeRegistered(
                System.Diagnostics.Process.GetCurrentProcess().Id,
                typeof(AuthService).Name);

            Thread.Sleep(Timeout.Infinite);
        }
    }
}