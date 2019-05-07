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

    [Header("Lerping Properties")]
    private bool _isLerpingPosition;
    private bool _isLerpingRotation;
    private Vector3 _realPostion;
    private Quaternion _realRotation;
    private Vector3 _lastRealPostion;
    private Quaternion _lastRealRotation;
    private float _timeStartedLerping;
    private float _timeToLerp;
    private float _lerpingPercentage;

    private bool _newSetPos;
    private Vector3 _applyPredictedInput;
    private bool _applyPredictedInputDone;

    private List<Vector3> _l = new List<Vector3>();
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (!IsLocalPlayer)
        {
            _isLerpingPosition = false;
            _isLerpingRotation = false;

            _realPostion = transform.position;
            _realRotation = transform.rotation;
        }
    }

    // Update is called once per frame  
    void Update()
    {
        if (Level >= LevelOfNetworking.B_Prediction && IsLocalPlayer)
        {
            if (!_applyPredictedInputDone)
            {

                _realPostion += _applyPredictedInput;
                _applyPredictedInputDone = true;
            }
        }

        transform.position = GetPosition();
        if(IsLocalPlayer)
            Debug.DrawRay(transform.position, new Vector3(0, 1, 0), Color.black, 10);
        else
            foreach (var l in _l.ToArray())
            {
                Debug.DrawRay(l, new Vector3(0, 1, 0), Color.white, 10);
                _l.Remove(l);
            }
    }

    private Vector3 GetPosition()
    {
        if (!UseInterpolation || !_isLerpingPosition)
            return _realPostion;

        if (_timeStartedLerping == 0)
            _timeStartedLerping = Time.time;

        _lerpingPercentage = (Time.time - _timeStartedLerping) / _timeToLerp;

        if (_lerpingPercentage >= 1)
        {
            _isLerpingPosition = false;
        }
        return Vector3.Lerp(_lastRealPostion, _realPostion, _lerpingPercentage);
    }

    public void ApplyPredictedInput(float XAxis, float YAxis)
    {
        _applyPredictedInput = new Vector3(XAxis, 0, YAxis);
        _applyPredictedInputDone = false;
    }

    public void SetLastServerAck(Vector3 position, Quaternion rotation, List<SendPackage> notAcknowledgedPackages, float timeToLerp)
    {
        if (Level >= LevelOfNetworking.C_Reconciliation && IsLocalPlayer)
        {
            foreach (var package in notAcknowledgedPackages.ToArray())
            {
                position.x += package.XAxis;
                position.z += package.YAxis;
            }
        }
        
        _l.Add(position);
        position.y = 0;
        _newSetPos = true;

        _lastRealPostion = _realPostion;
        _lastRealRotation = _realRotation;

        _realPostion = position;
        _realRotation = rotation;

        if (_realPostion != _lastRealPostion)
        {
            _isLerpingPosition = true;
        }
        if (_realRotation.eulerAngles != _lastRealRotation.eulerAngles)
        {
            _isLerpingPosition = true;
        }

        _timeToLerp = timeToLerp;
        _timeStartedLerping = 0;

    }
}



