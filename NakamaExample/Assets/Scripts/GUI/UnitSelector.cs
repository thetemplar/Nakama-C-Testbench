using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Manager;
using NakamaMinimalGame.PublicMatchState;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UnitSelector : MonoBehaviour
{
    public Text Me;
    public Text Enemy;
    public Slider MeSlider;
    public Slider EnemySlider;
    public Slider GCDSlider;
    public Slider CastBarSlider;

    public GameObject SelectedUnit;
    private PlayerController _selectedUnitPlayerController;
    public PlayerController MyPlayerController;
    private float _autoattackCD;

    // Start is called before the first frame update
    void Start()
    {
        EnemySlider.gameObject.SetActive(false);
        GCDSlider.gameObject.SetActive(false);
        CastBarSlider.gameObject.SetActive(false);

        Me.text = MyPlayerController.CurrentHealth + "HP";
        MeSlider.value = MyPlayerController.CurrentHealth;
        MeSlider.maxValue = MyPlayerController.MaxHealth;
        Enemy.text = "";
    }

    // Update is called once per frame
    void OnGUI()
    {
        Me.text = MyPlayerController.CurrentHealth + "HP";
        MeSlider.value = MyPlayerController.CurrentHealth;
        MeSlider.maxValue = MyPlayerController.MaxHealth;
        
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
                    _selectedUnitPlayerController = go.GetComponent<PlayerController>();
                    EnemySlider.maxValue = _selectedUnitPlayerController.MaxHealth;
                }
            }
        }

        if (SelectedUnit != null && Input.GetKey(KeyCode.Escape))
        {
            SelectedUnit = null;
            _selectedUnitPlayerController = null;
        }

        if (SelectedUnit != null)
        {
            EnemySlider.gameObject.SetActive(true);
            EnemySlider.value = _selectedUnitPlayerController.CurrentHealth;
            Enemy.text = _selectedUnitPlayerController.CurrentHealth + "HP";
        }
        else
        {
            Enemy.text = "";
            EnemySlider.gameObject.SetActive(false);
        }
    }
}
