using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using TravelPlanner.Shared.Interfaces;
using TravelPlanner.Shared.Models;

namespace EventDispatcher
{
    internal sealed class EventDispatcher : StatefulService, IEventDispatcherService
    {
        public EventDispatcher(StatefulServiceContext context)
            : base(context) { }

        // Metoda koju pozivaju drugi servisi kroz Remoting
        public async Task EnqueueShareTripEventAsync(ShareTripEvent tripEvent)
        {
            var queue = await StateManager
                .GetOrAddAsync<IReliableQueue<ShareTripEvent>>("shareTripQueue");

            using (var tx = StateManager.CreateTransaction())
            {
                await queue.EnqueueAsync(tx, tripEvent);
                await tx.CommitAsync();

                ServiceEventSource.Current.ServiceMessage(
                    Context,
                    $"Event enqueued: TripId={tripEvent.TripId}, AccessType={tripEvent.AccessType}");
            }
        }

        // Background worker — stalno obraðuje red
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            var queue = await StateManager
                .GetOrAddAsync<IReliableQueue<ShareTripEvent>>("shareTripQueue");

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var tx = StateManager.CreateTransaction())
                {
                    var result = await queue.TryDequeueAsync(tx);

                    if (result.HasValue)
                    {
                        var tripEvent = result.Value;

                        // Ovdje bi isla logika slanja emaila, notifikacije itd.
                        // Za sada logujemo
                        ServiceEventSource.Current.ServiceMessage(
                            Context,
                            $"Processing event: TripId={tripEvent.TripId}, " +
                            $"Token={tripEvent.Token}, " +
                            $"AccessType={tripEvent.AccessType}, " +
                            $"CreatedAt={tripEvent.CreatedAt}");

                        await tx.CommitAsync();
                    }
                    else
                    {
                        // Red je prazan, cekaj 1 sekundu
                        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                    }
                }
            }
        }

        // Otvori Remoting listener
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return this.CreateServiceRemotingReplicaListeners();
        }
    }
}