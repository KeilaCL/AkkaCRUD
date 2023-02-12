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

            var httpActor = system.ActorOf(Props.Create(() => new HttpActor(userActor)), "HttpActor");

            using (var listener = new HttpListener())
            {
                listener.Prefixes.Add("http://localhost:8080/");
                listener.Start();

                while (true)
                {
                    var context = listener.GetContext();

                    // Enviar la solicitud al actor HttpActor
                    httpActor.Tell(context);
                }
            }

        }
    }
}
