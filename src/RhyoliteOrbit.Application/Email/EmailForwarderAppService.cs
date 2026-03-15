using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.Actors;
using RhyoliteOrbit.Actors.MessageModels;

namespace RhyoliteOrbit.Email;

public class EmailForwarderAppService: RhyoliteOrbitAppServiceBase
{
    private readonly IActorRef _emailActorProvider;

    public EmailForwarderAppService(TopLevelActors.EmailActorProvider emailActorProvider)
    {
        _emailActorProvider = emailActorProvider();
    }
    
    
    [HttpPost]
    public async Task Send(EmailNotification dto)
    {
        await Task.Delay(1);
        _emailActorProvider.Tell(dto);
    }
}