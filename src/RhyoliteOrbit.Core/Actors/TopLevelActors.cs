using Akka.Actor;

namespace RhyoliteOrbit.Actors;

public class TopLevelActors
{
    public delegate IActorRef ApiCredentialSyncActorProvider();
    public delegate IActorRef DataSupportEntitySyncActorProvider();
    public delegate IActorRef PayslipDispatchActorProvider();
    public delegate IActorRef EmailActorProvider();
    public delegate IActorRef StudentBillActorProvider();
}