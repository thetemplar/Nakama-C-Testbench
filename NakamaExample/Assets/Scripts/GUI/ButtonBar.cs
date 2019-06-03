using System;
using System.Collections;
using System.Collections.Generic;
using NakamaMinimalGame.PublicMatchState;
using UnityEngine;

public class ButtonBar : MonoBehaviour
{
    [SerializeField]
    private PlayerController _player;
    public void ButtonBarPress(int btnId)
    {
        if (_player.GCDUntil < Time.time && _player.CastTimeUntil < Time.time)
        {
            var cast = new Client_Cast { SpellId = btnId };
            Assets.Scripts.Manager.GameManager.Instance.AddMessageToSend(cast);
            _player.GCDUntil = Time.time + 1.5f;
        }
    }
}
