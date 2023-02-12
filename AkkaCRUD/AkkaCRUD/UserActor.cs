using Akka.Actor;
using System;
using System.Collections.Generic;

namespace AkkaCRUD
{
    public class UserActor : ReceiveActor
    {
        private readonly IDictionary<string, User> _users = new Dictionary<string, User>();

        public UserActor()
        {
            Receive<CreateUser>(message =>
            {
                if (_users.ContainsKey(message.User.Id))
                {
                    Sender.Tell(new Exception($"El usuario con ID {message.User.Id} ya existe."));
                }
                else
                {
                    _users.Add(message.User.Id, message.User);
                    Sender.Tell("Usuario creado exitosamente.");
                }
            });

            Receive<ReadUser>(message =>
            {
                if (_users.TryGetValue(message.Id, out var user))
                {
                    Sender.Tell(user);
                }
                else
                {
                    Sender.Tell(new Exception($"No se encontró el usuario con ID {message.Id}."));
                }
            });

            Receive<UpdateUser>(message =>
            {
                if (_users.ContainsKey(message.Id))
                {
                    _users[message.Id] = message.User;
                    Sender.Tell("Usuario actualizado exitosamente.");
                }
                else
                {
                    Sender.Tell(new Exception($"No se encontró el usuario con ID {message.Id}."));
                }
            });

            Receive<DeleteUser>(message =>
            {
                if (_users.ContainsKey(message.Id))
                {
                    _users.Remove(message.Id);
                    Sender.Tell("Usuario eliminado exitosamente.");
                }
                else
                {
                    Sender.Tell(new Exception($"No se encontró el usuario con ID {message.Id}."));
                }
            });
        }
    }
}