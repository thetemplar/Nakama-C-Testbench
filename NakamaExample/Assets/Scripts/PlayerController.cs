using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Manager;
using NakamaMinimalGame.PublicMatchState;
using RPGCharacterAnims;
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

    [HideInInspector] public GameDB_Lib.GameDB_Spell castingSpell;
    [HideInInspector] public float CastTimeUntil;

    [HideInInspector] public float GCDUntil;

    [HideInInspector] public List<PublicMatchState.Types.Aura> Auras = new List<PublicMatchState.Types.Aura>();

    [HideInInspector] public GameDB_Lib.GameDB_Class playerClass;

    [HideInInspector] public PlayerController Target;

    public event EventHandler StartCastEvent;
    public event EventHandler InterruptCastEvent;

    public event EventHandler LostAura;
    public event EventHandler GotAura;

    static float _lastTime;

    public bool showLine = false;


    public RPGCharacterController RpgCharacterController;
    public RPGCharacterMovementController RpgCharacterMovementController;
    public RPGCharacterWeaponController RpgCharacterWeaponController;

    class LerpingParameters<T> {
        public bool IsLerping;
        public T Value;
        public T LastValue;
        public float TimeStarted;
        public float TimeToLerp;
        public float Percentage {
            get {
                //if (TimeStarted == 0)
                //    TimeStarted = Time.time;

                float perc = TimeToLerp > 0 ? (_lastTime - TimeStarted) / TimeToLerp : 1;
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
            TimeStarted = _lastTime;
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

    // Update is called once per frame  
    int ccc = 0;
    void Update()
    {
        _lastTime = Time.time;

        transform.position = GetPosition();
        transform.rotation = GetRotation();

        Color color = new Color(1.0f, 1.0f, 1.0f);
        Color color2 = new Color(0.0f, 1.0f, 1.0f);
        var diff = _lerpPosition.Value - _lerpPosition.LastValue;
        Vector3 myForward = transform.TransformDirection(Vector3.forward);
        float angle = Vector3.Angle(diff, transform.forward);
        //Debug.LogWarning(diff + " --- " + myForward + " === " + angle);

        if(showLine)
            Debug.DrawLine(new Vector3(_lerpPosition.LastValue.x, 1, _lerpPosition.LastValue.z), new Vector3(_lerpPosition.Value.x, 1, _lerpPosition.Value.z), ccc % 2 == 0 ? color : color2, 10000);

        if (RpgCharacterMovementController != null)
            RpgCharacterMovementController.currentVelocity = _lerpPosition.IsLerping ? diff * 10 : Vector3.zero;
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
        var vector = new Vector2(XAxis, YAxis);

        if (playerClass.Name == "")
            return;

        if (vector.magnitude > 1)
            vector = new Vector2(vector.x /= vector.magnitude, vector.y /= vector.magnitude);

        if (Level >= LevelOfNetworking.B_Prediction && isLocalPlayer)
        {
            var rotated = Rotate(new Vector2(vector.x * playerClass.MovementSpeed / 100f, vector.y * playerClass.MovementSpeed / 100f), rotation);

            //Debug.Log("Add: (" + vector.x + " | " + vector.y + " | " + rotation + ") " + rotated.x + " | " + rotated.y);
            var newPos = _lerpPosition.Value + new Vector3(rotated.x, 0, rotated.y);

            _lerpPosition.SetNext(newPos, timeToLerp);
            SetRotation((rotation + 360) % 360);
        }
    }
    
    public Vector2 Rotate(Vector2 v, float degrees)
    {
        float ca = (float)Math.Cos((360 - degrees) * 0.01745329251);
        float sa = (float)Math.Sin((360 - degrees) * 0.01745329251);
        return new Vector2(ca * v.x - sa * v.y, sa * v.x + ca * v.y);
    }

    public void SetLastServerAck(List<Client_Message> notAcknowledgedPackages, float timeToLerp, PublicMatchState.Types.Interactable player)
    {
        if (player == null)
            throw new Exception();

        var position = new Vector3(player.Position.X, 0f, player.Position.Y);
        var rotation = player.Rotation;

        if (playerClass.Name == null || playerClass.Name == "")
        {
            playerClass = GameManager.Instance.GameDB.Classes[player.Classname];
        }
        this.CurrentHealth = player.CurrentHealth;
        this.CurrentPower = player.CurrentPower;
        this.MaxHealth = (playerClass.BaseStamina + playerClass.GainStamina * player.Level) * 10;
        this.MaxPower = (playerClass.BaseIntellect + playerClass.GainIntellect * player.Level) * 10;

        foreach (var aura in player.Auras.Except(this.Auras))
        {
            this.Auras.Add(aura);
            GotAura?.Invoke(aura.EffectId, EventArgs.Empty);
        }

        foreach (var aura in this.Auras.Except(player.Auras))
        {
            LostAura?.Invoke(aura.EffectId, EventArgs.Empty);
            this.Auras.Remove(aura);
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


        _lerpPosition.Value = Vector3.Lerp(_lerpPosition.Value, position, Math.Min(1, Vector3.Distance(position, _lerpPosition.Value)));
        Quaternion.Lerp(Quaternion.AngleAxis(_lerpRotation.Value, Vector3.up), Quaternion.AngleAxis(rotation, Vector3.up), Math.Min(1, Math.Abs(_lerpRotation.Value - rotation) / 10));
        
        /* outdated, two Lerps above *should* do the trick :)
         
        if (Vector3.Distance(position, _lerpPosition.Value) >= 0.25f)
        {
            Debug.Log("dist too big:" + Vector3.Distance(position, _lerpPosition.Value));
            _lerpPosition.IsLerping = false;
            _lerpPosition.Value = position;
        }
        if (180 - Math.Abs(Math.Abs(rotation - _lerpRotation.Value) - 180) > 5f)
        {
            Debug.Log("angle too big:" + (180 - Math.Abs(Math.Abs(rotation - _lerpRotation.Value) - 180)).ToString());
            _lerpRotation.IsLerping = false;
            _lerpRotation.Value = rotation;
        }
        */
    }

    internal void InterruptCast()
    {
        castingSpell = new GameDB_Lib.GameDB_Spell();
        InterruptCastEvent?.Invoke(this, EventArgs.Empty);
    }

    internal void StartCast(long spellId)
    {
        castingSpell = GameManager.Instance.GameDB.Spells[spellId];
        StartCastEvent?.Invoke(this, EventArgs.Empty);
    }
}



