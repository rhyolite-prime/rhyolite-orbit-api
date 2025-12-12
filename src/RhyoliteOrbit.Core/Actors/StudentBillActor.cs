using System.Threading.Tasks;
using Akka.Actor;
using RhyoliteOrbit.Actors.MessageModels;

namespace RhyoliteOrbit.Actors;

public class StudentBillActor: ReceiveActor
{
    public StudentBillActor()
    {
        ReceiveAsync<StudentBillDto>(async a => await HandleMessage(a));
    }
    
    private async Task HandleMessage(StudentBillDto msg)
    {
        await Task.Delay(1);
        
        
    }
}