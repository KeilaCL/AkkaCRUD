using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Newtonsoft.Json;
using static AkkaCRUD.UserActor;


namespace AkkaCRUD
{
    public class HttpActor : ReceiveActor
    {
        private readonly IActorRef _userActor;
        private readonly HttpListener _listener;

        public HttpActor(IActorRef userActor)
        {
            _userActor = userActor;
            _listener = new HttpListener();
            _listener.Prefixes.Add("http://localhost:8080/");
            _listener.Start();
            Console.WriteLine("API en ejecución en http://localhost:8080/.");

            ReceiveAsync<HttpListenerContext>(async context =>
            {
                var request = context.Request;
                var response = context.Response;
                var url = request.Url.LocalPath;

                switch (request.HttpMethod)
                {
                    case "POST":
                        if (url == "/users")
                        {
                            var userJson = new StreamReader(request.InputStream).ReadToEnd();
                            var user = JsonConvert.DeserializeObject<User>(userJson);
                            var createUser = new CreateUser { User = user };
                            var result = await _userActor.Ask<string>(createUser);
                            WriteResponse(response, result);
                        }
                        else
                        {
                            WriteResponse(response, "No se encontró el recurso solicitado.", statusCode: 404);
                        }
                        break;
                    case "GET":
                        if (url.StartsWith("/users/"))
                        {
                            var userId = url.Split('/')[2];
                            var readUser = new ReadUser { Id = userId };
                            var result = await _userActor.Ask<User>(readUser);
                            WriteResponse(response, JsonConvert.SerializeObject(result));
                        }
                        else
                        {
                            WriteResponse(response, "No se encontró el recurso solicitado.", statusCode: 404);
                        }
                        break;
                    case "PUT":
                        if (url.StartsWith("/users/"))
                        {
                            var userId = url.Split('/')[2];
                            var userJson = new StreamReader(request.InputStream).ReadToEnd();
                            var user = JsonConvert.DeserializeObject<User>(userJson);
                            var updateUser = new UpdateUser { Id = userId, User = user };
                            var result = await _userActor.Ask<string>(updateUser);
                            WriteResponse(response, result);
                        }
                        else
                        {
                            WriteResponse(response, "No se encontró el recurso solicitado.", statusCode: 404);
                        }
                        break;
                    case "DELETE":
                        if (url.StartsWith("/users/"))
                        {
                            var userId = url.Split('/')[2];
                            var deleteUser = new DeleteUser { Id = userId };
                            var result = await _userActor.Ask<string>(deleteUser);
                            WriteResponse(response, result);
                        }
                        else
                        {
                            WriteResponse(response, "No se encontró el recurso solicitado.", statusCode: 404);
                        }
                        break;
                    default:
                        WriteResponse(response, "Método HTTP no soportado.", statusCode: 405);
                        break;
                }

                context.Response.Close();
            });
        }

        private void WriteResponse(HttpListenerResponse response, string message, int statusCode = 200)
        {
            response.StatusCode = statusCode;
            response.ContentType = "application/json";
            var bytes = Encoding.UTF8.GetBytes(message);
            response.OutputStream.Write(bytes, 0, bytes.Length);
        }

        protected override void PostStop()
        {
            _listener.Stop();
            _listener.Close();
        }
    }
}