using GameDB_Lib;
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
   // public OrbitCamera CameraScript;

    bool init = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!init)
        {
            List<string> classes = GameManager.Instance.GameDB.Classes.Select(x => x.Value.Name).ToList();
            DropDown.ClearOptions();
            DropDown.AddOptions(classes);
            init = true;
        }
    }

    public void Spawn()
    {
        GameManager.Instance.SpawnPlayer(DropDown);
     //   CameraScript.enabled = true;
        this.gameObject.SetActive(false);
    }

    public void Disconnect()
    {
        GameManager.Instance.LeaveGame();
    }
}
