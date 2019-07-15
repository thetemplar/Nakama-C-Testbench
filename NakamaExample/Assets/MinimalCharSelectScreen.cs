﻿using GameDB_Lib;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Manager;

public class MinimalCharSelectScreen : MonoBehaviour
{
    public Dropdown DropDown;
    public Button SpawnButton;
    public OrbitCamera CameraScript;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        List<string> classes = GameManager.Instance.GameDB.Classes.Select(x => x.Value.Name).ToList();
        DropDown.ClearOptions();
        DropDown.AddOptions(classes);
    }

    public void Spawn()
    {
        GameManager.Instance.Spawn(DropDown);
        CameraScript.enabled = true;
        this.gameObject.SetActive(false);


    }
}