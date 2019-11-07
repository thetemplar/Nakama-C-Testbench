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
                    FileStream fs = File.Open(@"C:\Users\Kristian\Documents\nakama-project\Nakama-GameDB-CodeGen\GameDB_CodeGen\save.bin", FileMode.Open);

                    _gameDB = (GameDB) formatter.Deserialize(fs);
                    fs.Flush();
                    fs.Close();
                    fs.Dispose();
                }  
                return _gameDB;
            } 
        }


        private ISocket _socket { get { return NakamaManager.Instance.Socket; } }

        //private bool _matchJoined;
        private bool _isLeaving;

        private DateTime _timeOfLastState;

        //public event Action OnGameStarted;
        //public event Action OnGameEnded;
        
        public event Action<PublicMatchState, float> OnNewWorldUpdate;

        public void Join()
        {
            SpawnPlayer("Mage");
        }

        public void SpawnPlayer(Dropdown classSelected)
        {
            SpawnPlayer(classSelected.options[classSelected.value].text);
        }

        public void SpawnPlayer(string classSelected)
        {
            var c = new Client_Message
            {
                ClientTick = 1,
                SelectChar = new Client_Message.Types.Client_SelectCharacter
                {
                    Classname = classSelected
                }
            };
            Thread.Sleep(50);
            SendMatchStateMessage(c);
        }
        private void Start()
        {
            JoinMatchAsync(NakamaManager.Instance.MatchId);
        }

        public async void JoinMatchAsync(string matchId)
        {
            try
            {
                _socket.OnMatchState += ReceiveMatchStateMessage;

                // Join the match
                IMatch match = await _socket.JoinMatchAsync(matchId);
                Debug.Log("Joined match ID: " + matchId);
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
            SceneManager.LoadScene("MainMenu");
        }

        private /*unsafe*/ void ReceiveMatchStateMessage(object sender, IMatchState e)
        {
            var diffTime = (float)(DateTime.Now - _timeOfLastState).TotalSeconds;

            PublicMatchState state = PublicMatchState.Parser.ParseFrom(e.State);
            //TypedReference tr = __makeref(state);
            //IntPtr ptr = **(IntPtr**)(&tr);
            OnNewWorldUpdate?.Invoke(state, diffTime);

            _timeOfLastState = DateTime.Now;
        }

        public void SendMatchStateMessage(Client_Message msg)
        {
            byte[] toSend;
            long opCode = -1;
            if (msg == null)
            {
                toSend = new byte[] { };
                opCode = 255;
            } 
            else
            {
                toSend = msg.ToByteArray();
                switch (msg.TypeCase)
                {
                    case Client_Message.TypeOneofCase.Move:
                        opCode = 2;
                        break;
                    case Client_Message.TypeOneofCase.SelectChar:
                        opCode = 100;
                        break;
                    default:
                        opCode = 1;
                        break;
                }
            }
            try
            {
                _socket.SendMatchState(NakamaManager.Instance.MatchId, opCode, toSend);
            }
            catch (Exception e)
            {
                Debug.LogError("Error while sending match state: " + e.Message);
            }
        }
    }
}
