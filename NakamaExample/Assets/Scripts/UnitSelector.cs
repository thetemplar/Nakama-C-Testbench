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
    private float _gcd;
    private float _cast;
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

        if (_cast - Time.time > 0)
        {
            CastBarSlider.gameObject.SetActive(true);
            CastBarSlider.value = CastBarSlider.maxValue - (_cast - Time.time);
        }
        else
        {
            CastBarSlider.gameObject.SetActive(false);
        }

        if (_gcd - Time.time > 0)
        {
            GCDSlider.gameObject.SetActive(true);
            GCDSlider.value = (_gcd - Time.time);
        }
        else if (_cast - Time.time <= 0)
        {
            GCDSlider.gameObject.SetActive(false);

            //cast 
            if (Input.GetKey("1")) //  && _gcd > Time.time
            {
                var cast = new Client_Cast { SpellId = 1 };
                GameManager.Instance.AddMessageToSend(cast);
                _gcd = Time.time + 1.5f;
                _cast = Time.time + 2f;
                CastBarSlider.maxValue = 2f;
            }
            if (Input.GetKey("2")) //  && _gcd > Time.time
            {
                var cast = new Client_Cast { SpellId = 2 };
                GameManager.Instance.AddMessageToSend(cast);
                _gcd = Time.time + 1.5f;
            }
            if (Input.GetKey("3")) //  && _gcd > Time.time
            {
                var cast = new Client_Cast { SpellId = 3 };
                GameManager.Instance.AddMessageToSend(cast);
                _gcd = Time.time + 1.5f;
            }
            if (Input.GetKey(KeyCode.Escape)) //  && _gcd > Time.time
            {
                var stop = new Client_CancelAttack { };
                GameManager.Instance.AddMessageToSend(stop);
                _cast = Time.time;
            }
            if (Input.GetKey(KeyCode.Space) && _autoattackCD < Time.time) //  && _gcd > Time.time
            {
                var cast = new Client_Autoattack { Attacktype = Client_Autoattack.Types.Type.Meele };
                GameManager.Instance.AddMessageToSend(cast);
                _autoattackCD = Time.time + .5f;
            }
        }
    }
}
