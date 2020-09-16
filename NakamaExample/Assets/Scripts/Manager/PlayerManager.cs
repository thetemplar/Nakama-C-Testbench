using Assets.Scripts.NakamaManager;
using NakamaMinimalGame.PublicMatchState;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Google.Protobuf;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Manager
{
    public class PlayerManager : Singleton<PlayerManager>
    {
        public PlayerController Player;
        public PlayerController ServerShadow;
        
        public GameObject PrefabBullet;
        public GameObject PrefabArea;
        public GameObject PrefabNPC;

       // public OrbitCamera CameraScript;

        [HideInInspector]
        public bool Spawned = false;
        [HideInInspector]
        public string ClassName = "";

        private Dictionary<string, PlayerController> _gameObjects = new Dictionary<string, PlayerController>();
        public Dictionary<string, string> UserNames = new Dictionary<string, string>();
        private List<Client_Message> _notAcknowledgedPackages = new List<Client_Message>();
        private Queue<Client_Message> _messagesToSend = new Queue<Client_Message>();

        private long _lastConfirmedServerTick;
        private long _clientTick;

        private float lastRotation;

        private InputActions _inputActions;
        Vector2 _move;
        float _rotate;

        public Cinemachine.CinemachineFreeLook Camera;

        private void Start()
        {
            GameManager.Instance.OnNewWorldUpdate += OnNewWorldUpdate;

            _inputActions = new InputActions();
            _inputActions.Enable();
            _inputActions.Standard.Movement.performed += ctx => _move = ctx.ReadValue<Vector2>();
            _inputActions.Standard.Turn.performed += Turn_performed;
            //_inputActions.Standard.Movement.canceled += ctx => _move = Vector2.zero;
        }

        private void Turn_performed(InputAction.CallbackContext obj)
        { 
            _rotate = obj.ReadValue<float>();
        }

        private void FixedUpdate()
        {
            if (NakamaManager.Instance == null)
                return;

           // CameraScript.enabled = true;
            if (string.IsNullOrEmpty(NakamaManager.Instance.MatchId) || !NakamaManager.Instance.IsConnected)
            {
                NakamaManager.Instance.Disconnect();
                SceneManager.LoadScene("MainMenu");
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
                        Target = (Player.Target != null) ? Player.Target.name : ""
                    }
                };
                _notAcknowledgedPackages.Add(send);
                GameManager.Instance.SendMatchStateMessage(send);

                //TODO: maybe change network code from "here am i" every frame to "this input differs now to the last"
                {
                    var XAxis = _move.x;
                    var YAxis = Mouse.current.rightButton.isPressed && Mouse.current.leftButton.isPressed ? 1 : (_move.y > 0 ? _move.y : _move.y / 2);
                    var rot = _rotate * Time.deltaTime * 130f;

                    var length = (float)Math.Sqrt(Math.Pow(XAxis, 2) + Math.Pow(YAxis, 2));
                    if (length > 1)
                    {
                        XAxis /= length;
                        YAxis /= length;
                    }

                    lastRotation = Player.Rotation;
                    var move = new Client_Message
                    {
                        ClientTick = _clientTick,
                        Move = new Client_Message.Types.Client_Movement
                        {
                            AbsoluteCoordinates = false,
                            XAxis = XAxis,
                            YAxis = YAxis,
                            Rotation = (Camera.m_XAxis.Value + rot)
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

        public void AddMessageToSend(Client_Message msg)
        {
            msg.ClientTick = _clientTick;
            _messagesToSend.Enqueue(msg);
        }

        private void OnNewWorldUpdate(PublicMatchState state, float diffTime)
        {
            //remove destroyed
            foreach (var player in _gameObjects.Where(x => !(x.Key.StartsWith("p_") || x.Key.StartsWith("a_")) && !state.Interactable.ContainsKey(x.Key)))
            {
                UnityThread.executeInUpdate(() => DestroyWorldObject(player.Key));
            }

            //player & NPCs            
            foreach (var entry in state.Combatlog)
            {
                if(entry.TypeCase == PublicMatchState.Types.CombatLogEntry.TypeOneofCase.Cast)
                {
                    switch (entry.Cast.Event)
                    {
                        case PublicMatchState.Types.CombatLogEntry.Types.CombatLogEntry_Cast.Types.CombatLogEntry_Cast_Event.Start:
                            _gameObjects[entry.SourceId].StartCast(entry.SourceSpellId);
                            break;
                        case PublicMatchState.Types.CombatLogEntry.Types.CombatLogEntry_Cast.Types.CombatLogEntry_Cast_Event.Interrupted:
                            _gameObjects[entry.SourceId].InterruptCast();
                            break;

                    }
                }
            }
            foreach (var player in state.Interactable)
            {
                if (!UserNames.ContainsKey(player.Key))
                {
                    UserNames.Add(player.Key, player.Value.Username);
                }
                if(UserNames[player.Key] != player.Value.Username)
                {
                    UserNames[player.Key] = player.Value.Username;
                }

                //handle player character
                if (player.Key == NakamaManager.Instance.Session.UserId)
                {
                    //spawn player
                    if (!Spawned)
                    {
                        Spawned = true;
                        ClassName = player.Value.Classname;
                        UnityThread.executeInUpdate(() =>
                        {
                            var p = Player.gameObject;
                            p.transform.Find("Mesh").gameObject.SetActive(true);
                            _gameObjects.Add(player.Key, p.GetComponent<PlayerController>());
                            p.name = player.Key;
                        });
                    }


                    _notAcknowledgedPackages.RemoveAll(x => x.ClientTick <= player.Value.LastProcessedClientTick);

                    if (Player.ShowGhost)
                        ServerShadow.SetLastServerAck(null, diffTime, player.Value);

                    Player.SetLastServerAck(_notAcknowledgedPackages, diffTime, player.Value);
                }
                else
                {
                    if (_gameObjects.ContainsKey(player.Key))
                    {
                        if (player.Value?.Position != null)
                        {
                            _gameObjects[player.Key].SetLastServerAck(null, diffTime, player.Value);
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
                        _gameObjects[projectile.Key].SetLastServerAck(null, diffTime, null);
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

            //remove destroyed
            foreach (var area in _gameObjects.Where(x => x.Key.StartsWith("a_") && !state.Area.ContainsKey(x.Key)))
            {
                UnityThread.executeInUpdate(() => DestroyWorldObject(area.Key));
            }

            //update area
            foreach (var area in state.Area)
            {
                if (_gameObjects.ContainsKey(area.Key))
                {
                    if (area.Value != null && area.Value?.Position != null)
                    {
                    }
                    else
                    {
                        UnityThread.executeInUpdate(() => DestroyWorldObject(area.Key));
                    }
                }
                //redundant
                else if (area.Value?.Position != null)
                {
                    UnityThread.executeInUpdate(() =>
                    {
                        var prefab = PrefabArea;
                        var radius = ((GameDB_Lib.GameDB_Effect_Persistent_Area_Aura)(GameManager.Instance.GameDB.Effects[area.Value.EffectId].Type)).Radius;
                        prefab.transform.localScale = new Vector3(radius * 2, 0.1f, radius * 2);
                        InstantiateWorldObject(prefab, area.Key, new Vector3(area.Value.Position.X, 0.52f, area.Value.Position.Y), 0);
                    });
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
            if (!_gameObjects.ContainsKey(name))
                Debug.LogError("key not found!");
            return _gameObjects[name].Position;
        }
    }
}
