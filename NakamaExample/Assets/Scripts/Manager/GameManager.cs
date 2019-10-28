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
using GameDB_Lib;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Assets.Scripts.Manager
{
    public class GameManager : Singleton<GameManager>
    {
        private GameDB _gameDB;
        public GameDB GameDB { 
            get {
                if(_gameDB == null) 
                {
                    //Format the object as Binary
                    BinaryFormatter formatter = new BinaryFormatter();

                    //Reading the file from the server
                    FileStream fs = File.Open(@"C:\Users\Kristian\source\repos\Nakama-C-Testbench\NakamaExample\Assets\save.bin", FileMode.Open);

                    _gameDB = (GameDB) formatter.Deserialize(fs);
                    fs.Flush();
                    fs.Close();
                    fs.Dispose();
                }  
                return _gameDB;
            } 
        }
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

        public void Join()
        {
            Task.Run(async () => {
                Thread.Sleep(1000);
                string id = await NakamaManager.Instance.StartOrJoinGameAsync();
                Debug.Log("StartOrJoin: " + id);
                JoinMatchAsync(id);
            });
#if !UNITY_EDITOR
            SceneManager.LoadScene("Main");
#endif
        }

        public void Spawn(Dropdown classSelected)
        {
            var c = new Client_Message
            {
                SelectChar = new Client_Message.Types.Client_SelectCharacter
                {
                    Classname = classSelected.options[classSelected.value].text
                }
            };
            Thread.Sleep(50);
            SendMatchStateMessage(100, c.ToByteArray());
        }

        public async void JoinMatchAsync(string matchId)
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

        public void SendMatchStateMessage(Client_Message msg)
        {
            byte[] toSend;
            long opCode = -1;
            switch (msg.GetType().ToString())
            {
                case "NakamaMinimalGame.PublicMatchState.Client_Cast":
                    toSend = msg.ToByteArray();
                    opCode = 1;
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
