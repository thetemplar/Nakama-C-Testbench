using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nakama;
using Nakama.TinyJson;

namespace NakamaClient
{
    class Program
    {

        static string authtoken = "";
        private const string PrefKeyName = "nakama.session";

        private static IClient _client = new Client("defaultkey", "127.0.0.1", 7350, false);
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
            socket.OnConnect += (sender, args) =>
            {
                Console.WriteLine("[OnConnect] Socket connected.");
            };
            socket.OnDisconnect += (sender, args) =>
            {
                Console.WriteLine("[OnDisconnect] Socket disconnected.");
            };
            await socket.ConnectAsync(_session);
            

            Console.WriteLine(" * *  Handle events  * * ");
            socket.OnStatusPresence += (_, presence) =>
            {
                foreach (var join in presence.Joins)
                {
                    Console.WriteLine("[OnStatusPresence] User '{0}' joined.", join.Username);
                }
                foreach (var leave in presence.Leaves)
                {
                    Console.WriteLine("[OnStatusPresence] User '{0}' left.", leave.Username);
                }
            };
            Console.WriteLine(" * *  DONE  INIT  * * ");
            var matchmakerTicket = await socket.AddMatchmakerAsync("*", 2);
            socket.OnMatchmakerMatched += async (_, matched) =>
            {
                Console.WriteLine("[OnMatchmakerMatched] Received MatchmakerMatched message: {0}", matched);
                var opponents = string.Join(",", matched.Users); // printable list.
                Console.WriteLine("[OnMatchmakerMatched] Matched opponents: {0}", opponents);
                IMatch match = await socket.JoinMatchAsync(matched);


                var id = match.Id;
                var opCode = 1;
                var timer = new System.Threading.Timer((e) =>
                {
                    socket.SendMatchState(id, opCode, "SendMatchState :) " + _session.UserId);
                }, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            };
            

            Console.WriteLine(" * *  Receive data messages  * * ");
            socket.OnMatchState += (_, state) => {
                var in_content = System.Text.Encoding.UTF8.GetString(state.State);

                Console.WriteLine("[OnMatchState] User  sent {0} with Opcode: {1}", in_content, state.OpCode);
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