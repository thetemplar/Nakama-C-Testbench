using Assets.Scripts.NakamaManager;
using NakamaMinimalGame.PublicMatchState;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Google.Protobuf;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Manager
{
    public class GameManager : Singleton<GameManager>
    {
        public PlayerController Player;
        public PlayerController ServerShadow;
        public UnitSelector UnitSelector;

        public GameObject PrefabBullet;
        public GameObject PrefabNPC;

        private Dictionary<string, PlayerController> _gameObjects = new Dictionary<string, PlayerController>();
        private List<Client_Character> _notAcknowledgedPackages = new List<Client_Character>();
        private Stack<object> _messagesToSend = new Stack<object>();

        private long _lastConfirmedServerTick;
        private long _clientTick;

        private void Start()
        {
            MatchManager.Instance.OnNewWorldUpdate += OnNewWorldUpdate;
        }

        private void FixedUpdate()
        {
            if (string.IsNullOrEmpty(MatchManager.Instance.MatchId) || !NakamaManager.Instance.IsConnected)
            {
                SceneManager.LoadScene("MainMenu");
                return;
            }

            Client_Character send = new Client_Character
            {
                XAxis = Input.GetAxis("Horizontal") * (Input.GetMouseButton(1) ? 1 : 0) + (Input.GetKey("q") ? -1 : 0) + (Input.GetKey("e") ? 1 : 0),
                YAxis = Input.GetAxis("Vertical"),
                Rotation = Player.Rotation,
                LastConfirmedServerTick = _lastConfirmedServerTick,
                ClientTick = _clientTick,
                Target = (UnitSelector.SelectedUnit != null) ? UnitSelector.SelectedUnit.name : "",
            };

            if (Player.Level >= PlayerController.LevelOfNetworking.B_Prediction)
                Player.ApplyPredictedInput(send.XAxis, send.YAxis, send.Rotation, Time.fixedDeltaTime);

            _notAcknowledgedPackages.Add(send);
            MatchManager.Instance.SendMatchStateMessage(0, send.ToByteArray());
            
            while(_messagesToSend.Count > 0)
            {
                var msg = _messagesToSend.Pop();
                MatchManager.Instance.SendMatchStateMessage(msg);
            }

            _clientTick++;
        }

        void OnGUI()
        {
            if (Application.isEditor)  // or check the app debug flag
            {
                GUI.Label(new Rect(Screen.width - 100, 0,100,100), _notAcknowledgedPackages.Count.ToString());
            }
        }

        public void AddMessageToSend(object msg)
        {
            _messagesToSend.Push(msg);
        }

        private void OnNewWorldUpdate(PublicMatchState state, float diffTime)
        {
            //player & NPCs
            foreach (var player in state.Interactable)
            {
                //handle player character
                if (player.Key == NakamaManager.Instance.Session.UserId)
                {
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

        private void InstantiateWorldObject(GameObject prefab, string key, Vector3 pos, float angle)
        {
            if (!_gameObjects.ContainsKey(key))
            {
                GameObject obj = Instantiate(prefab, pos, Quaternion.AngleAxis(angle, Vector3.up));
                obj.name = key;
                _gameObjects.Add(key, obj.GetComponent<PlayerController>());
            }
        }
    }
}
