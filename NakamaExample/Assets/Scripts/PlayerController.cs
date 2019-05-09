using System;
using System.Collections;
using System.Collections.Generic;
using NakamaMinimalGame.PublicMatchState;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]

    public enum LevelOfNetworking
    {
        A_Dumb,
        B_Prediction,
        C_Reconciliation,
    };

    [SerializeField]
    public LevelOfNetworking Level = LevelOfNetworking.A_Dumb;

    [SerializeField]
    public bool IsLocalPlayer;
    public bool UseInterpolation;

    CharacterController controller;

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
        
        public void SetNext(T value, float timeToLerp)
        {        
            LastValue = Value;
            Value = value;  
            if (EqualityComparer<T>.Default.Equals(LastValue, Value))
            {
                IsLerping = true;
            }
            TimeToLerp = timeToLerp;
            TimeStarted = 0;
        }
    }

    private LerpingParameters<Vector3> _lerpPosition = new LerpingParameters<Vector3>();
    private LerpingParameters<float> _lerpRotation = new LerpingParameters<float>();

    public float Rotation => _lerpRotation.Value;
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame  
    void Update()
    {
        var pos = GetPosition();
        if (float.IsNaN(pos.x))
            Debug.Log("NANAAAAAA");
        transform.position = pos;
        transform.rotation = GetRotation();
    }

    void FixedUpdate()
    {
        if(IsLocalPlayer)
            _lerpRotation.SetNext(transform.rotation.eulerAngles.y + (Input.GetKey("q") ? -10 : 0) + (Input.GetKey("e") ? 10 : 0), Time.fixedDeltaTime);
    }

    private Vector3 GetPosition()
    {
        if (!UseInterpolation || !_lerpPosition.IsLerping)
            return _lerpPosition.Value;

        return Vector3.Lerp(_lerpPosition.LastValue, _lerpPosition.Value, _lerpPosition.Percentage);
    }
    private Quaternion GetRotation()
    {
        if (!UseInterpolation || !_lerpRotation.IsLerping)
            return Quaternion.AngleAxis(_lerpRotation.Value, Vector3.up);

        return Quaternion.Lerp(Quaternion.AngleAxis(_lerpRotation.LastValue, Vector3.up), Quaternion.AngleAxis(_lerpRotation.Value, Vector3.up), _lerpRotation.Percentage);
    }

    public void ApplyPredictedInput(float XAxis, float YAxis, float rotation, float timeToLerp)
    {
        if (Level >= LevelOfNetworking.B_Prediction && IsLocalPlayer)
        {
            var rotated = Rotate(new Vector2(XAxis, YAxis), _lerpRotation.Value);
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

    public void SetLastServerAck(Vector3 position, float rotation, List<SendPackage> notAcknowledgedPackages, float timeToLerp)
    {
        position.y = 0;
        if (Level >= LevelOfNetworking.C_Reconciliation && IsLocalPlayer)
        {
            foreach (var package in notAcknowledgedPackages.ToArray())
            {
                var rotated = Rotate(new Vector2(package.XAxis, package.YAxis), package.Rotation);
                position.x += rotated.x;
                position.z += rotated.y;
                rotation = package.Rotation;
            }
        }
        
        if(!IsLocalPlayer)        
        {
            _lerpPosition.SetNext(position, timeToLerp);
            _lerpRotation.SetNext(rotation, timeToLerp);
            return;
        }

        if(Vector3.Distance(position, _lerpPosition.Value) > 0.2f)
        {
            Debug.Log("dist too big:" + Vector3.Distance(position, _lerpPosition.Value));
            _lerpPosition.IsLerping = false;
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



