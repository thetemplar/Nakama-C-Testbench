using Assets.Scripts.NakamaManager;
using Google.Protobuf;
using Nakama;
using NakamaMinimalGame.PublicMatchState;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.Manager
{
    public class MatchManager : Singleton<MatchManager>
    {
        private ISocket _socket { get { return NakamaManager.Instance.Socket; } }
        
        public string MatchId
        {
            get;
            private set;
        }

        //private bool _matchJoined;
        private bool _isLeaving;

        private DateTime _timeOfLastState;

        //public event Action OnGameStarted;
        //public event Action OnGameEnded;
        
        public event Action<PublicMatchState, float> OnNewWorldUpdate;

        public void StartOrJoin(Dropdown classSelected)
        {
            Debug.Log("StartOrJoin started1");
            Task.Run(async () => {
                Thread.Sleep(1000);
                string id = await NakamaManager.Instance.StartOrJoinGameAsync();
                Debug.Log("StartOrJoin: " + id);
                JoinMatchAsync(id, "Mage");
            });
#if !UNITY_EDITOR
            SceneManager.LoadScene("Main");
#endif
        }

        public void StartOrJoin()
        {
            Task.Run(async () => {
                Debug.Log("StartOrJoin started2");
                Thread.Sleep(1000);
                string id = await NakamaManager.Instance.StartOrJoinGameAsync();
                Debug.Log("StartOrJoin: " + id);
                JoinMatchAsync(id, "Mage");
            });
#if !UNITY_EDITOR
            SceneManager.LoadScene("Main");
#endif
        }

        public async void JoinMatchAsync(string matchId, string className)
        {
            try
            {
                // Listen to incomming match messages and user connection changes
                //_socket.OnMatchPresence += OnMatchPresence;
                _socket.OnMatchState += ReceiveMatchStateMessage;

                // Join the match
                IMatch match = await _socket.JoinMatchAsync(matchId);
                Debug.Log("Created & joined match with ID: " + matchId);
                // Set current match id
                // It will be used to leave the match later
                MatchId = match.Id;
                CombatLogGUI.CombatLog.Add(new PublicMatchState.Types.CombatLogEntry { SystemMessage = "Joined match with id: " + match.Id + "; presences count: " + match.Presences.Count() });

                var c = new Client_SelectCharacter { Classname = "Mage" };
                Thread.Sleep(50);
                SendMatchStateMessage(100, c.ToByteArray());



                // Add all players already connected to the match
                // If both players uses the same account, exit the game
                //AddConnectedPlayers(match);
                //_matchJoined = true;
                //StartGame();
            }
            catch (Exception e)
            {
                Debug.LogError("Couldn't join match: " + e.Message);
            }
        }
        
        public void LeaveGame()
        {
            if (_isLeaving == true)
            {
                Debug.Log("Already leaving");
                return;
            }
            _isLeaving = true;
            //_socket.OnMatchPresence -= OnMatchPresence;
            _socket.OnMatchState -= ReceiveMatchStateMessage;

            //Starts coroutine which is loading main menu and also disconnects player from match
            //StartCoroutine(LoadMenuCoroutine());
        }

        private void ReceiveMatchStateMessage(object sender, IMatchState e)
        {
            var diffTime = (float)(DateTime.Now - _timeOfLastState).TotalSeconds;

            PublicMatchState state = PublicMatchState.Parser.ParseFrom(e.State);
            CombatLogGUI.CombatLog.AddRange(state.Combatlog.ToArray());
            OnNewWorldUpdate?.Invoke(state, diffTime);

            _timeOfLastState = DateTime.Now;
        }

        public void SendMatchStateMessage(object msg)
        {
            byte[] toSend;
            long opCode = -1;
            switch (msg.GetType().ToString())
            {
                case "NakamaMinimalGame.PublicMatchState.Client_Cast":
                    toSend = ((Client_Cast)msg).ToByteArray();
                    opCode = 1;
                    break;
                case "NakamaMinimalGame.PublicMatchState.Client_Autoattack":
                    toSend = ((Client_Autoattack)msg).ToByteArray();
                    opCode = 2;
                    break;
                case "NakamaMinimalGame.PublicMatchState.Client_CancelAttack":
                    toSend = ((Client_CancelAttack)msg).ToByteArray();
                    opCode = 3;
                    break;
                case "NakamaMinimalGame.PublicMatchState.Client_SelectCharacter":
                    toSend = ((Client_SelectCharacter)msg).ToByteArray();
                    opCode = 100;
                    break;
                default:
                    Debug.Log(msg.GetType().ToString() + " not known in sender list");
                    return;
            }
            try
            {
                _socket.SendMatchState(MatchId, opCode, toSend);
            }
            catch (Exception e)
            {
                Debug.LogError("Error while sending match state: " + e.Message);
            }
        }

        public void SendMatchStateMessage(long opCode, byte[] msg)
        {
            try
            {
                _socket.SendMatchState(MatchId, opCode, msg);
            }
            catch (Exception e)
            {
                Debug.LogError("Error while sending match state: " + e.Message);
            }
        }
    }
}
