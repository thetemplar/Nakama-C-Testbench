using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nakama;

namespace NakamaMinimalGame.NakamaClient
{
    class MatchManager
    {
        private readonly ISession _session;
        private readonly ISocket _socket;
        private readonly IClient _client;
        GameManager _gm = GameManager.Instance;

        private string _match = "";
        public bool IsInMatch => string.IsNullOrEmpty(_match);

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
            if (!string.IsNullOrEmpty(_match))
                throw new Exception("Already in a match!");
            var match = await _socket.CreateMatchAsync();
            Console.WriteLine("Created match with ID '{0}'. Authoritive: '{1}'", match.Id, match.Authoritative);
            _match = match.Id;

            _socket.OnMatchState += SocketOnOnMatchState;
            UpdateGameStatus?.Invoke();
        }

        private void SocketOnOnMatchState(object sender, IMatchState state)
        {
            var content = System.Text.Encoding.UTF8.GetString(state.State);
            switch (state.OpCode)
            {
                default:
                    Console.WriteLine("User {0} sent {1}", state.UserPresence.Username, content);
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
            if (string.IsNullOrEmpty(_match))
                throw new Exception("Not in a match!");
            await _socket.LeaveMatchAsync(matchId);
            _match = "";
            UpdateGameStatus?.Invoke();
        }

        public async Task ListMatches()
        {
            var list = await _client.ListMatchesAsync(_session, 2, 2, )
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
            if (string.IsNullOrEmpty(_match))
                throw new Exception("Not in a match!");

            var opCode = 1;
            var newState = "data";
            _socket.SendMatchState(_match, opCode, newState);
        }
    }
}
