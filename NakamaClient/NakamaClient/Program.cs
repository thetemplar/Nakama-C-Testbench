using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Nakama;
using Nakama.TinyJson;

namespace NakamaClient
{
    class Program
    {

        static string authtoken = "";
        private const string PrefKeyName = "nakama.session";

        private static IClient _client = new Client("defaultkey", "18.185.43.148", 7350, false);
        private static ISession _session;

        private static async Task LogIn()
        {

            Console.WriteLine(" * *  Authenticate  * * ");
            if (!string.IsNullOrEmpty(authtoken))
            {
                var session = Session.Restore(authtoken);
                if (!session.IsExpired)
                {
                    _session = session;
                    Console.WriteLine(_session);
                    return;
                }
            }

            _session = await _client.AuthenticateDeviceAsync(Environment.UserName + "_auth_user" + DateTime.Now.Second);
            authtoken = _session.AuthToken;
            Console.WriteLine(_session);



            Console.WriteLine(" * *  Sessions  * * ");
            Console.WriteLine(""); // raw JWT token
            Console.WriteLine(_session.AuthToken); // raw JWT token
            Console.WriteLine("User id '{0}'", _session.UserId);
            Console.WriteLine("User username '{0}'", _session.Username);
            Console.WriteLine("Session has expired: {0}", _session.IsExpired);
            Console.WriteLine("Session expires at: {0}", _session.ExpireTime); // in seconds.


            Console.WriteLine(" * *  Send requests  * * ");
            var account = await _client.GetAccountAsync(_session);
            Console.WriteLine("User id '{0}'", account.User.Id);
            Console.WriteLine("User username '{0}'", account.User.Username);
            Console.WriteLine("Account virtual wallet '{0}'", account.Wallet);
            


            Console.WriteLine(" * *  Socket messages  * * ");
            var socket = _client.CreateWebSocket();
            socket.OnConnect += async (sender, args) =>
            {
                Console.WriteLine("[OnConnect] Socket connected.");

                var match = await _client.RpcAsync(_session, "createMatch");
                await socket.JoinMatchAsync(match.Id);
            };
            socket.OnDisconnect += (sender, args) =>
            {
                Console.WriteLine("[OnDisconnect] Socket disconnected.");
            };
            await socket.ConnectAsync(_session);
            
            
            Console.WriteLine(" * *  Receive data messages  * * ");
            socket.OnMatchState += (_, state) => {
                var in_content = System.Text.Encoding.UTF8.GetString(state.State);

                Console.WriteLine("[OnMatchState] User  sent {0} with Opcode: {1}", in_content, state.OpCode);
                PublicMatchState msg = PublicMatchState.Parser.ParseFrom(state.State);
                Console.WriteLine("Stopwatch-Server: 0:" + (msg.Stopwatch[0] / 1000f) + "ms 1:" + (msg.Stopwatch[1] / 1000f) + "ms 2:" + (msg.Stopwatch[2] / 1000f) + "ms 3:" + (msg.Stopwatch[3] / 1000f) + "ms 4:" + (msg.Stopwatch[4] / 1000f) + "ms");

            };
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Hello Mono World");
            LogIn();
            Console.ReadLine();
        }
    }
}