using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    public bool UseInterpolation = true;

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
    }

    private void FixedUpdate()
    {
        if (!IsLocalPlayer)
        {
            if (UseInterpolation)
            {
                NakamaLerp();
            }
            else
            {
                transform.position = _realPostion;
                transform.rotation = _realRotation;
            }
        }
        else
        {
            LocalClientUpdate();
        }

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
            Debug.Log("_isLerpingPosition:" + _lerpingPercentage);

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

    private void LocalClientUpdate()
    {
        if (!IsLocalPlayer)
            return;

        controller.Move(new Vector3(Input.GetAxis("Horizontal") * 0.2f, 0, Input.GetAxis("Vertical") * 0.2f));

        //is it too far off?
        Ray ray = new Ray(_realPostion, _lastRealPostion);
        if (Vector3.Cross(ray.direction, transform.position - ray.origin).magnitude > .2f)
        {
            //todo: lerp
            //transform.position = _realPostion;
        }
    }

    //This is Entity Interpolation
    public void SetPosition(Vector3 position, Quaternion rotation, float timeToLerp)
    {
        if(!UseInterpolation)
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
