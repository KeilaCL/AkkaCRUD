using Akka.Actor;
using System;

namespace AkkaCRUD
{
    class Program
    {
        static void Main(string[] args)
        {
            var system = ActorSystem.Create("demo-system");
            var userActor = system.ActorOf<UserActor>("user-actor");
            var httpActor = system.ActorOf(Props.Create(() => new HttpActor(userActor)), "http-actor");
            Console.ReadLine();
            system.Terminate().Wait();

        }
    }
}
