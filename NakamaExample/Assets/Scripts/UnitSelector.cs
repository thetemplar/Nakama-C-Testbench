using System;
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
    public Slider MeSlider;
    public Slider EnemySlider;
    public Slider GCDSlider;

    public GameObject SelectedUnit;
    private float _gcd;

    private List<Client_Cast> _sendMessages = new List<Client_Cast>();


    // Start is called before the first frame update
    void Start()
    {
        EnemySlider.gameObject.SetActive(false);
        GCDSlider.gameObject.SetActive(false);

        Me.text = "100HP";
        MeSlider.value = 100;
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
            EnemySlider.gameObject.SetActive(true);
            Enemy.text = "100HP";
            EnemySlider.value = 100;
        }
        else
        {
            Enemy.text = "";
            EnemySlider.gameObject.SetActive(false);
        }

        if (_gcd - Time.time > 0)
        {
            GCDSlider.gameObject.SetActive(true);
            GCD.gameObject.SetActive(true);
            GCD.text = (_gcd - Time.time) + "ms";
            GCDSlider.value = (_gcd - Time.time);
        }
        else
        {
            GCD.gameObject.SetActive(false);
            GCDSlider.gameObject.SetActive(false);
        }
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
