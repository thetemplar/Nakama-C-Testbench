using Assets.Scripts.NakamaManager;
using NakamaMinimalGame.PublicMatchState;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Google.Protobuf;

namespace Assets.Scripts.Manager
{
    public class PlayerManager : Singleton<PlayerManager>
    {
        public PlayerController Player;
        public PlayerController ServerShadow;

        public PlayerGUI UnitSelector;
        
        public GameObject PrefabBullet;
        public GameObject PrefabNPC;

        [HideInInspector]
        public bool Spawned = false;
        public string ClassName = "";

        private Dictionary<string, PlayerController> _gameObjects = new Dictionary<string, PlayerController>();
        private List<Client_Message> _notAcknowledgedPackages = new List<Client_Message>();
        private Queue<Client_Message> _messagesToSend = new Queue<Client_Message>();

        private long _lastConfirmedServerTick;
        private long _clientTick;


#if UNITY_EDITOR
        private bool _startJoin = false;
#endif

        private void Start()
        {
            GameManager.Instance.OnNewWorldUpdate += OnNewWorldUpdate;
        }

        private void FixedUpdate()
        {
            if (string.IsNullOrEmpty(GameManager.Instance.MatchId) || !NakamaManager.Instance.IsConnected)
            {
#if UNITY_EDITOR
                if (!_startJoin)
                {
                    _startJoin = true;
                    GameManager.Instance.Join();
                }
#else
                SceneManager.LoadScene("MainMenu");
#endif
                return;
            }
            if (Player.gameObject.activeSelf)
            {

                Client_Message send = new Client_Message
                {
                    ClientTick = _clientTick,
                    Character = new Client_Message.Types.Client_Character
                    {
                        LastConfirmedServerTick = _lastConfirmedServerTick,
                        Target = (UnitSelector.SelectedUnit != null) ? UnitSelector.SelectedUnit.name : ""
                    }
                };
                _notAcknowledgedPackages.Add(send);
                GameManager.Instance.SendMatchStateMessage(send);

                if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) // TODO: AND ROTATION DID NOT CHANGE!
                {
                    var move = new Client_Message
                    {
                        ClientTick = _clientTick,
                        Move = new Client_Message.Types.Client_Movement
                        {
                            AbsoluteCoordinates = false,
                            XAxis = Input.GetAxis("Horizontal") * (Input.GetMouseButton(1) ? 1 : 0) + (Input.GetKey("q") ? -1 : 0) + (Input.GetKey("e") ? 1 : 0),
                            YAxis = Input.GetAxis("Vertical"),
                            Rotation = Player.Rotation
                        }
                    };
                    _notAcknowledgedPackages.Add(move);
                    GameManager.Instance.SendMatchStateMessage(move);
                    Player.ApplyPredictedInput(move.Move.XAxis, move.Move.YAxis, move.Move.Rotation, Time.fixedDeltaTime);
                }
            }
            else
            {
                if(_clientTick%10==0)
                    GameManager.Instance.SendMatchStateMessage(null); //no-op
            }
            
            while(_messagesToSend.Count > 0)
            {
                var msg = _messagesToSend.Dequeue();
                Debug.Log("SendMatchStateMessage> " + msg);
                GameManager.Instance.SendMatchStateMessage(msg);
            }

            _clientTick++;
        }

        void OnGUI()
        {
#if UNITY_EDITOR
            if (Application.isEditor)  // or check the app debug flag
            {
                GUI.Label(new Rect(Screen.width - 100, 0, 100, 100), _notAcknowledgedPackages.Count.ToString());
            }
#endif
        }

        public void AddMessageToSend(Client_Message msg)
        {
            msg.ClientTick = _clientTick;
            _messagesToSend.Enqueue(msg);
        }

        private void OnNewWorldUpdate(PublicMatchState state, float diffTime)
        {
            //player & NPCs
            foreach (var player in state.Interactable)
            {
                //handle player character
                if (player.Key == NakamaManager.Instance.Session.UserId)
                {
                    //spawn player
                    if (!Spawned)
                    {
                        Spawned = true;
                        ClassName = player.Value.Character.Classname;
                    }
                    _notAcknowledgedPackages.RemoveAll(x => x.ClientTick <= player.Value.LastProcessedClientTick);

                    if (Player.ShowGhost)
                        ServerShadow.SetLastServerAck(new Vector3(player.Value.Position.X, 1.5f, player.Value.Position.Y), player.Value.Rotation, null, diffTime, null);

                    Player.SetLastServerAck(new Vector3(player.Value.Position.X, 1.5f, player.Value.Position.Y), player.Value.Rotation, _notAcknowledgedPackages, diffTime, player.Value);
                }
                else
                {
                    if (_gameObjects.ContainsKey(player.Key))
                    {
                        if (player.Value?.Position != null)
                        {
                            _gameObjects[player.Key].SetLastServerAck(new Vector3(player.Value.Position.X, 1.5f, player.Value.Position.Y), player.Value.Rotation, null, diffTime, player.Value);
                        }
                        else
                        {
                            UnityThread.executeInUpdate(() => DestroyWorldObject(player.Key));
                        }
                    }
                    else
                    {
                        if (player.Value?.Position != null)
                        {
                            UnityThread.executeInUpdate(() => InstantiateWorldObject(PrefabNPC, player.Key, new Vector3(player.Value.Position.X, 0f, player.Value.Position.Y), player.Value.Rotation));
                        }
                    }
                }
            }

            //remove destroyed
            foreach (var projectile in _gameObjects.Where(x => x.Key.StartsWith("p_") && !state.Projectile.ContainsKey(x.Key)))
            {
                UnityThread.executeInUpdate(() => DestroyWorldObject(projectile.Key));
            }

            //update projectiles
            foreach (var projectile in state.Projectile)
            {
                if (_gameObjects.ContainsKey(projectile.Key))
                {
                    if (projectile.Value != null && projectile.Value?.Position != null)
                    {
                        _gameObjects[projectile.Key].SetLastServerAck(new Vector3(projectile.Value.Position.X, 1.5f, projectile.Value.Position.Y), projectile.Value.Rotation, null, diffTime, null);
                    }
                    else
                    {
                        UnityThread.executeInUpdate(() => DestroyWorldObject(projectile.Key));
                    }
                }
                //redundant
                else if (projectile.Value?.Position != null)
                {
                    UnityThread.executeInUpdate(() => InstantiateWorldObject(PrefabBullet, projectile.Key, new Vector3(projectile.Value.Position.X, 0f, projectile.Value.Position.Y), projectile.Value.Rotation));
                }
            }

            _lastConfirmedServerTick = state.Tick;
        }

        private void DestroyWorldObject(string key)
        {
            if (_gameObjects.ContainsKey(key))
            {
                var del = _gameObjects.Where(x => x.Key == key).FirstOrDefault();
                Destroy(del.Value.gameObject);
                _gameObjects.Remove(key);
            }
        }

        private GameObject InstantiateWorldObject(GameObject prefab, string key, Vector3 pos, float angle)
        {
            if (!_gameObjects.ContainsKey(key))
            {
                GameObject obj = Instantiate(prefab, pos, Quaternion.AngleAxis(angle, Vector3.up));
                obj.name = key;
                _gameObjects.Add(key, obj.GetComponent<PlayerController>());

                return obj;
            }
            return null;
        }

        public Vector3 GetGameObjectPosition(string name)
        {
            if(!_gameObjects.ContainsKey(name))
                Debug.LogError("key not found!");
            return _gameObjects[name].Position;
        }
    }
}
