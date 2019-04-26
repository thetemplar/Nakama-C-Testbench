using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using Nakama;
using Newtonsoft.Json.Linq;

namespace NakamaMinimalGame.NakamaClient
{
    class MatchManager
    {
        private readonly ISession _session;
        private readonly ISocket _socket;
        private readonly IClient _client;
        GameManager _gm = GameManager.Instance;

        private string _match = "";
        private string _ticket = "";
        public bool IsInMatch => !string.IsNullOrEmpty(_match);
        public bool IsQueued => !string.IsNullOrEmpty(_ticket);

        public delegate void UpdateGameStatusHandler();
        public event UpdateGameStatusHandler UpdateGameStatus;

        public MatchManager(ISession session, IClient client, ISocket socket)
        {
            _session = session;
            _client = client;
            _socket = socket;
        }

        public async Task CreateMatch()
        {
            if (IsInMatch)
                throw new Exception("Already in a match!");
            var match = await _client.RpcAsync(_session, "createMatch");

            Console.WriteLine("Created match with ID '{0}'.'", match.Payload);
            _match = match.Payload;
            
            _socket.OnMatchState += SocketOnOnMatchState;
            UpdateGameStatus?.Invoke();
        }

        private void SocketOnOnMatchState(object sender, IMatchState state)
        {
            var content = Encoding.UTF8.GetString(state.State);
            switch (state.OpCode)
            {
                default:
                    Console.WriteLine("MatchData: Opcode {0} - Data: {1}", state.OpCode, content);
                    break;
            }
        }

        public async Task JoinMatch(string matchId)
        {
            var match = await _socket.JoinMatchAsync(matchId);
            foreach (var presence in match.Presences)
            {
                Console.WriteLine("User id '{0}' name '{1}'.", presence.UserId, presence.Username);
            }
            _match = match.Id;
            UpdateGameStatus?.Invoke();
        }

        public async Task LeaveMatch(string matchId)
        {
            if (!IsInMatch)
                throw new Exception("Not in a match!");
            await _socket.LeaveMatchAsync(matchId);
            _match = "";
            UpdateGameStatus?.Invoke();
        }

        public async Task ListMatches()
        {
           //var list = await _client.ListMatchesAsync(_session, 2, 2, )
            _match = "";
            UpdateGameStatus?.Invoke();
        }

        public async Task ListAllOpponents()
        {
            /*
            if (string.IsNullOrEmpty(_match))
                throw new Exception("Not in a match!");
            var connectedOpponents = new List<IUserPresence>(0);
            _socket.OnMatchPresence += (_, presence) =>
            {
                connectedOpponents.AddRange(presence.Joins);
                foreach (var leave in presence.Leaves)
                {
                    connectedOpponents.RemoveAll(item => item.SessionId.Equals(leave.SessionId));
                };
            };*/
        }

        public async Task SendData()
        {
            if (!IsInMatch)
                throw new Exception("Not in a match!");

            var opCode = 1;
            var newState = "data";
            _socket.SendMatchState(_match, opCode, newState);
        }

        public async Task QueueMatchmaking()
        {
            if (IsQueued)
                throw new Exception("Already queued!");
            if (IsInMatch)
                throw new Exception("Already in a match!");

            var matchmakerTicket = await _socket.AddMatchmakerAsync("*", 2);
            _ticket = matchmakerTicket.Ticket;

            _socket.OnMatchmakerMatched += async (_, matched) =>
            {
                Console.WriteLine("[OnMatchmakerMatched] Received MatchmakerMatched message: {0}", matched);
                var opponents = string.Join(",", matched.Users.Select(x => x.Presence.Username)); // printable list.
                Console.WriteLine("[OnMatchmakerMatched] Matched opponents: {0}", opponents);
                IMatch match = await _socket.JoinMatchAsync(matched);
                _ticket = "";
                _match = match.Id;

                _socket.OnMatchState += SocketOnOnMatchState;
                var id = match.Id;
                var opCode = 1;
                var timer = new System.Threading.Timer((e) =>
                {
                    _socket.SendMatchStateAsync(id, opCode, "SendMatchState :) " + _session.UserId);
                }, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            };
        }

        public async Task CancelMatchmaking()
        {
            if (!IsQueued)
                throw new Exception("Not queued!");

            await _socket.RemoveMatchmakerAsync(_ticket);
            _ticket = "";
        }
    }
}
