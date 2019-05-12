﻿using System;
using System.Collections;
using System.Collections.Generic;
using NakamaMinimalGame.PublicMatchState;
using UnityEngine;
using UnityEngine.UI;

public class UnitSelector : MonoBehaviour
{
    public Text Me;
    public Text Enemy;
    public Text GCD;

    public GameObject SelectedUnit;
    private float _gcd;

    private List<Client_Cast> _sendMessages = new List<Client_Cast>();


    // Start is called before the first frame update
    void Start()
    {
        Me.text = "100HP";
        Enemy.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject go = hit.transform.root.gameObject;
                if (go.layer == 8)
                {
                    SelectedUnit = go;
                }
            }
        }

        if (SelectedUnit != null && Input.GetKey(KeyCode.Escape))
            SelectedUnit = null;

        if (SelectedUnit != null)
        {
            Enemy.text = "100HP";
        }
        else
        {
            Enemy.text = "";
        }

        GCD.text = (Time.time - _gcd <= 0) ? (Time.time - _gcd).ToString() : "";

        //cast 
        if (Input.GetKey("1")) //  && _gcd > Time.time
        {
            var cast = new Client_Cast { Spellname = "fireball" };
            _sendMessages.Add(cast);
            _gcd = Time.time + 1.5f;
        }
    }

    internal Client_Cast[] GetCastMessages()
    {
        var tmp = _sendMessages.ToArray();
        _sendMessages.Clear();
        return tmp;
    }
}
