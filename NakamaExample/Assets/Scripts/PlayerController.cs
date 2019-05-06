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
        C_PredictionWithFeedback,
        D_Reconciliation,
        E_Interpolation
    };

    [SerializeField]
    public LevelOfNetworking Level = LevelOfNetworking.A_Dumb;

    [SerializeField]
    public bool IsLocalPlayer;

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
        //if (UseInterpolation)    NakamaLerp();
        
        if(Level >= LevelOfNetworking.B_Prediction)
        {
            if (Level >= LevelOfNetworking.C_PredictionWithFeedback)
            {
                if (_newSetPos)
                {
                    _newSetPos = false;
                    transform.position = _realPostion;
                    transform.rotation = _realRotation;
                }
            }
        }
        else // LevelOfNetworking.A_Dumb
        {
            transform.position = _realPostion;
            transform.rotation = _realRotation;
        }        
    }

    private void FixedUpdate()
    {
        if (Level >= LevelOfNetworking.B_Prediction)
        {
            MovementPrediction();
        }
    }

    private void MovementPrediction()
    {
        if (!IsLocalPlayer)
            return;

        //controller.Move(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")));
        transform.position = new Vector3(transform.position.x + Input.GetAxis("Horizontal"), transform.position.y, transform.position.z + Input.GetAxis("Vertical"));
    }

    private void NakamaLerp()
    {
        if (_isLerpingPosition || _isLerpingRotation)
        {
            if (_timeStartedLerping == 0)
                _timeStartedLerping = Time.time;
            _lerpingPercentage = (Time.time - _timeStartedLerping) / _timeToLerp;
        }

        if (_isLerpingPosition)
        {
            transform.position = Vector3.Lerp(_lastRealPostion, _realPostion, _lerpingPercentage);

            if (_lerpingPercentage >= 1)
            {
                _isLerpingPosition = false;
            }
        }
        if (_isLerpingRotation)
        {
            transform.rotation = Quaternion.Lerp(_lastRealRotation, _realRotation, _lerpingPercentage);

            if (_lerpingPercentage >= 1)
            {
                _isLerpingRotation = false;
            }
        }
    }

    public void SetLastServerAck(Vector3 position, Quaternion rotation, long lastReceivedTick, List<SendPackage> sentPackagesSinceLastServerFrame, float timeToLerp)
    {
        if (Level < LevelOfNetworking.D_Reconciliation)
        {
            SetPosition(position, rotation, timeToLerp);
            return;
        }

        foreach (var package in sentPackagesSinceLastServerFrame)
        {
            position.x += package.XAxis;
            position.z += package.YAxis;
        }
        SetPosition(position, rotation, timeToLerp);
    }

    //This is Entity Interpolation
    public void SetPosition(Vector3 position, Quaternion rotation, float timeToLerp)
    {
        _newSetPos = true;
        if (Level < LevelOfNetworking.E_Interpolation)
        {
            _realPostion = position;
            _realRotation = rotation;
            return;
        }

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
