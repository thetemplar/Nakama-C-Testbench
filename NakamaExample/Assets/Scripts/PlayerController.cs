using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Manager;
using NakamaMinimalGame.PublicMatchState;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] public enum LevelOfNetworking
    {
        A_Dumb,
        B_Prediction,
        C_Reconciliation,
    };

    [SerializeField] public LevelOfNetworking Level = LevelOfNetworking.A_Dumb;

    [SerializeField] private bool isLocalPlayer;
    [SerializeField] private bool useInterpolation;
    [SerializeField] public bool ShowGhost;

    [HideInInspector] public float MaxHealth;
    [HideInInspector] public float CurrentHealth;

    [HideInInspector] public float MaxPower;
    [HideInInspector] public float CurrentPower;

    [HideInInspector] public float CastTimeUntil;
    [HideInInspector] public float FullCastTime;

    [HideInInspector] public float GCDUntil;

    [HideInInspector] public List<PublicMatchState.Types.Aura> Auras = new List<PublicMatchState.Types.Aura>();

    [HideInInspector] public GameDB_Lib.GameDB_Class playerClass;

    private Animator animator;

    class LerpingParameters<T> {
        public bool IsLerping;
        public T Value;
        public T LastValue;
        public float TimeStarted;
        public float TimeToLerp;
        public float Percentage {
            get {
                if (TimeStarted == 0)
                    TimeStarted = Time.time;

                float perc = TimeToLerp > 0 ? (Time.time - TimeStarted) / TimeToLerp : 1;
                if (perc >= 1)
                {
                    IsLerping = false;
                }

                return perc;
            }
        }

        public LerpingParameters()
        {
        }

        public LerpingParameters(T initial)
        {
            Value = initial;
        }

        public void SetNext(T value, float timeToLerp)
        {
            LastValue = Value;
            Value = value;
            if (!EqualityComparer<T>.Default.Equals(LastValue, Value))
            {
                IsLerping = true;
            }
            TimeToLerp = timeToLerp;
            TimeStarted = 0;
        }
    }

    private LerpingParameters<Vector3> _lerpPosition;
    private LerpingParameters<float> _lerpRotation;

    public float Rotation => _lerpRotation.Value;
    public Vector3 Position => _lerpPosition.Value;
    

    void Awake()
    {
        _lerpPosition = new LerpingParameters<Vector3>(transform.position);
        _lerpRotation = new LerpingParameters<float>(transform.rotation.eulerAngles.y);
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void AttackAnimation()
    {
        animator.SetTrigger("AutoAttack");
    }

    // Update is called once per frame  
    void Update()
    {
        transform.position = GetPosition();
        transform.rotation = GetRotation();
    }

    void FixedUpdate()
    {
    }

    public void SetRotation(float rotation)
    {
        _lerpRotation.SetNext(rotation, 0);
    }

    private Vector3 GetPosition()
    {
        if (!useInterpolation || !_lerpPosition.IsLerping)
            return _lerpPosition.Value;
        return Vector3.Lerp(_lerpPosition.LastValue, _lerpPosition.Value, _lerpPosition.Percentage);
    }

    private Quaternion GetRotation()
    {
        if (!useInterpolation || !_lerpRotation.IsLerping)
            return Quaternion.AngleAxis(_lerpRotation.Value, Vector3.up);

        return Quaternion.Lerp(Quaternion.AngleAxis(_lerpRotation.LastValue, Vector3.up), Quaternion.AngleAxis(_lerpRotation.Value, Vector3.up), _lerpRotation.Percentage);
    }

    public void ApplyPredictedInput(float XAxis, float YAxis, float rotation, float timeToLerp)
    {
        if (playerClass.Name == "")
        {
            return;
        }

        if (Level >= LevelOfNetworking.B_Prediction && isLocalPlayer)
        {
            var rotated = Rotate(new Vector2(XAxis * playerClass.MovementSpeed / 100f, YAxis * playerClass.MovementSpeed / 100f), _lerpRotation.Value);
            var newPos = _lerpPosition.Value + new Vector3(rotated.x, 0, rotated.y);

            _lerpPosition.SetNext(newPos, timeToLerp);     
        }
    }
    
    public Vector2 Rotate(Vector2 v, float degrees)
    {
        float ca = (float)Math.Cos((360 - degrees) * 0.01745329251);
        float sa = (float)Math.Sin((360 - degrees) * 0.01745329251);
        return new Vector2(ca * v.x - sa * v.y, sa * v.x + ca * v.y);
    }

    public void SetLastServerAck(Vector3 position, float rotation, List<Client_Message> notAcknowledgedPackages, float timeToLerp, PublicMatchState.Types.Interactable player = null)
    {
        if (player != null)
        {
            if (playerClass.Name == null || playerClass.Name == "")
            {
                playerClass = GameManager.Instance.GameDB.Classes[player.Classname];
            }
            this.CurrentHealth = player.CurrentHealth;
            this.CurrentPower = player.CurrentPower;
            this.MaxHealth = (playerClass.BaseStamina + playerClass.GainStamina * player.Level) * 10;
            this.MaxPower = (playerClass.BaseIntellect + playerClass.GainIntellect * player.Level) * 10;
            this.Auras.Clear();
            foreach (var aura in player.Auras)
            {
                this.Auras.Add(aura);
            }
        }

        position.y = 0;
        if (Level >= LevelOfNetworking.C_Reconciliation && isLocalPlayer)
        {
            foreach (var package in notAcknowledgedPackages.ToArray())
            {
                if (package.TypeCase != Client_Message.TypeOneofCase.Move)
                    continue;
                var rotated = Rotate(new Vector2(package.Move.XAxis, package.Move.YAxis), package.Move.Rotation);
                float length = (float)Math.Sqrt(Math.Pow(rotated.x, 2) + Math.Pow(rotated.y, 2));
                if(length > 1)
                {
                    position.x += (rotated.x / length);
                    position.z += (rotated.y / length);
                } else
                {
                    position.x += rotated.x;
                    position.z += rotated.y;
                }
                rotation = package.Move.Rotation;
            }
        }
        
        if(!isLocalPlayer)
        {
            _lerpPosition.SetNext(position, timeToLerp);
            _lerpRotation.SetNext(rotation, timeToLerp);
            return;
        }

        if(Vector3.Distance(position, _lerpPosition.Value) > 0.5f)
        {
            Debug.Log("dist too big:" + Vector3.Distance(position, _lerpPosition.Value));
            //_lerpPosition.IsLerping = false;
            _lerpPosition.Value = position;

        }
        if(180 - Math.Abs(Math.Abs(rotation - _lerpRotation.Value) - 180) > 5f)
        {
            Debug.Log("angle too big:" + (180 - Math.Abs(Math.Abs(rotation - _lerpRotation.Value) - 180)).ToString());
            _lerpRotation.IsLerping = false;
            _lerpRotation.Value = rotation;        
        }
    }
}



