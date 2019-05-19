using System;
using System.Collections;
using System.Collections.Generic;
using NakamaMinimalGame.PublicMatchState;
using UnityEditor;
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
    private PlayerController _selectedUnitPlayerController;
    public PlayerController MyPlayerController;
    private float _gcd;

    private List<Client_Cast> _sendMessages = new List<Client_Cast>();


    // Start is called before the first frame update
    void Start()
    {
        EnemySlider.gameObject.SetActive(false);
        GCDSlider.gameObject.SetActive(false);

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

            //cast 
            if (Input.GetKey("1")) //  && _gcd > Time.time
            {
                var cast = new Client_Cast { SpellId = 1 };
                _sendMessages.Add(cast);
                _gcd = Time.time + 1.5f;
            }
            if (Input.GetKey("2")) //  && _gcd > Time.time
            {
                var cast = new Client_Cast { SpellId = 2 };
                _sendMessages.Add(cast);
                _gcd = Time.time + 1.5f;
            }
            if (Input.GetKey("3")) //  && _gcd > Time.time
            {
                var cast = new Client_Cast { SpellId = 3 };
                _sendMessages.Add(cast);
                _gcd = Time.time + 1.5f;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Handles.Label(new Vector3(5.944444f, 0.1f, 12.33333f), "0");
        Debug.DrawLine(new Vector3(4.833333f, 0.1f, 12f), new Vector3(4.333333f, 0.1f, 12.5f), Color.red);
        Debug.DrawLine(new Vector3(4.833333f, 0.1f, 12f), new Vector3(8.666667f, 0.1f, 12.5f), Color.blue);
        Debug.DrawLine(new Vector3(4.333333f, 0.1f, 12.5f), new Vector3(8.666667f, 0.1f, 12.5f), Color.blue);

        Handles.Label(new Vector3(6.222222f, 0.1f, 11f), "1");
        Debug.DrawLine(new Vector3(5.166667f, 0.1f, 8.5f), new Vector3(4.833333f, 0.1f, 12f), Color.red);
        Debug.DrawLine(new Vector3(5.166667f, 0.1f, 8.5f), new Vector3(8.666667f, 0.1f, 12.5f), Color.blue);
        Debug.DrawLine(new Vector3(4.833333f, 0.1f, 12f), new Vector3(8.666667f, 0.1f, 12.5f), Color.blue);

        Handles.Label(new Vector3(9.555554f, 0.1f, 8.888889f), "2");
        Debug.DrawLine(new Vector3(5.166667f, 0.1f, 8.5f), new Vector3(8.666667f, 0.1f, 12.5f), Color.blue);
        Debug.DrawLine(new Vector3(5.166667f, 0.1f, 8.5f), new Vector3(14.83333f, 0.1f, 5.666667f), Color.blue);
        Debug.DrawLine(new Vector3(8.666667f, 0.1f, 12.5f), new Vector3(14.83333f, 0.1f, 5.666667f), Color.red);

        Handles.Label(new Vector3(10.33333f, 0.1f, 5.111111f), "3");
        Debug.DrawLine(new Vector3(5.166667f, 0.1f, 8.5f), new Vector3(14.83333f, 0.1f, 5.666667f), Color.blue);
        Debug.DrawLine(new Vector3(5.166667f, 0.1f, 8.5f), new Vector3(11f, 0.1f, 1.166667f), Color.blue);
        Debug.DrawLine(new Vector3(14.83333f, 0.1f, 5.666667f), new Vector3(11f, 0.1f, 1.166667f), Color.blue);

        Handles.Label(new Vector3(8.944446f, 0.1f, 3.5f), "4");
        Debug.DrawLine(new Vector3(5.166667f, 0.1f, 8.5f), new Vector3(11f, 0.1f, 1.166667f), Color.blue);
        Debug.DrawLine(new Vector3(5.166667f, 0.1f, 8.5f), new Vector3(10.66667f, 0.1f, 0.8333334f), Color.blue);
        Debug.DrawLine(new Vector3(11f, 0.1f, 1.166667f), new Vector3(10.66667f, 0.1f, 0.8333334f), Color.red);

        Handles.Label(new Vector3(13.55555f, 0.1f, 2.666667f), "5");
        Debug.DrawLine(new Vector3(14.83333f, 0.1f, 5.666667f), new Vector3(14.83333f, 0.1f, 1.166667f), Color.red);
        Debug.DrawLine(new Vector3(14.83333f, 0.1f, 5.666667f), new Vector3(11f, 0.1f, 1.166667f), Color.blue);
        Debug.DrawLine(new Vector3(14.83333f, 0.1f, 1.166667f), new Vector3(11f, 0.1f, 1.166667f), Color.red);

        Handles.Label(new Vector3(3.111111f, 0.1f, 5.5f), "6");
        Debug.DrawLine(new Vector3(0f, 0.1f, 0f), new Vector3(4.166667f, 0.1f, 8f), Color.blue);
        Debug.DrawLine(new Vector3(0f, 0.1f, 0f), new Vector3(5.166667f, 0.1f, 8.5f), Color.blue);
        Debug.DrawLine(new Vector3(4.166667f, 0.1f, 8f), new Vector3(5.166667f, 0.1f, 8.5f), Color.red);

        Handles.Label(new Vector3(5.277779f, 0.1f, 3.111111f), "7");
        Debug.DrawLine(new Vector3(0f, 0.1f, 0f), new Vector3(5.166667f, 0.1f, 8.5f), Color.blue);
        Debug.DrawLine(new Vector3(0f, 0.1f, 0f), new Vector3(10.66667f, 0.1f, 0.8333334f), Color.blue);
        Debug.DrawLine(new Vector3(5.166667f, 0.1f, 8.5f), new Vector3(10.66667f, 0.1f, 0.8333334f), Color.blue);

        Handles.Label(new Vector3(7.111113f, 0.1f, 0.2777778f), "8");
        Debug.DrawLine(new Vector3(0f, 0.1f, 0f), new Vector3(10.66667f, 0.1f, 0.8333334f), Color.blue);
        Debug.DrawLine(new Vector3(0f, 0.1f, 0f), new Vector3(10.66667f, 0.1f, 0f), Color.blue);
        Debug.DrawLine(new Vector3(10.66667f, 0.1f, 0.8333334f), new Vector3(10.66667f, 0.1f, 0f), Color.red);

        Handles.Label(new Vector3(1.388889f, 0.1f, 5.166667f), "9");
        Debug.DrawLine(new Vector3(0f, 0.1f, 0f), new Vector3(0f, 0.1f, 7.5f), Color.blue);
        Debug.DrawLine(new Vector3(0f, 0.1f, 0f), new Vector3(4.166667f, 0.1f, 8f), Color.blue);
        Debug.DrawLine(new Vector3(0f, 0.1f, 7.5f), new Vector3(4.166667f, 0.1f, 8f), Color.red);

        Handles.Label(new Vector3(19.5f, 0.1f, 2.166667f), "10");
        Debug.DrawLine(new Vector3(24.5f, 0.1f, 0f), new Vector3(17f, 0.1f, 0f), Color.blue);
        Debug.DrawLine(new Vector3(24.5f, 0.1f, 0f), new Vector3(17f, 0.1f, 6.5f), Color.blue);
        Debug.DrawLine(new Vector3(17f, 0.1f, 0f), new Vector3(17f, 0.1f, 6.5f), Color.red);

        Handles.Label(new Vector3(22f, 0.1f, 7.555557f), "11");
        Debug.DrawLine(new Vector3(24.5f, 0.1f, 16.16667f), new Vector3(24.5f, 0.1f, 0f), Color.red);
        Debug.DrawLine(new Vector3(24.5f, 0.1f, 16.16667f), new Vector3(17f, 0.1f, 6.5f), Color.blue);
        Debug.DrawLine(new Vector3(24.5f, 0.1f, 0f), new Vector3(17f, 0.1f, 6.5f), Color.blue);

        Handles.Label(new Vector3(16.61111f, 0.1f, 12.94445f), "12");
        Debug.DrawLine(new Vector3(24.5f, 0.1f, 16.16667f), new Vector3(17f, 0.1f, 6.5f), Color.blue);
        Debug.DrawLine(new Vector3(24.5f, 0.1f, 16.16667f), new Vector3(8.333334f, 0.1f, 16.16667f), Color.blue);
        Debug.DrawLine(new Vector3(17f, 0.1f, 6.5f), new Vector3(8.333334f, 0.1f, 16.16667f), Color.red);

        Handles.Label(new Vector3(19.11111f, 0.1f, 18.94445f), "13");
        Debug.DrawLine(new Vector3(24.5f, 0.1f, 24.5f), new Vector3(24.5f, 0.1f, 16.16667f), Color.red);
        Debug.DrawLine(new Vector3(24.5f, 0.1f, 24.5f), new Vector3(8.333334f, 0.1f, 16.16667f), Color.blue);
        Debug.DrawLine(new Vector3(24.5f, 0.1f, 16.16667f), new Vector3(8.333334f, 0.1f, 16.16667f), Color.blue);

        Handles.Label(new Vector3(10.94444f, 0.1f, 21.72222f), "14");
        Debug.DrawLine(new Vector3(24.5f, 0.1f, 24.5f), new Vector3(8.333334f, 0.1f, 16.16667f), Color.blue);
        Debug.DrawLine(new Vector3(24.5f, 0.1f, 24.5f), new Vector3(0f, 0.1f, 24.5f), Color.red);
        Debug.DrawLine(new Vector3(8.333334f, 0.1f, 16.16667f), new Vector3(0f, 0.1f, 24.5f), Color.blue);

        Handles.Label(new Vector3(6.611111f, 0.1f, 13.16667f), "15");
        Debug.DrawLine(new Vector3(6.833333f, 0.1f, 14.5f), new Vector3(8.666667f, 0.1f, 12.5f), Color.red);
        Debug.DrawLine(new Vector3(6.833333f, 0.1f, 14.5f), new Vector3(4.333333f, 0.1f, 12.5f), Color.blue);
        Debug.DrawLine(new Vector3(8.666667f, 0.1f, 12.5f), new Vector3(4.333333f, 0.1f, 12.5f), Color.blue);

        Handles.Label(new Vector3(5f, 0.1f, 13.16667f), "16");
        Debug.DrawLine(new Vector3(6.833333f, 0.1f, 14.5f), new Vector3(4.333333f, 0.1f, 12.5f), Color.blue);
        Debug.DrawLine(new Vector3(6.833333f, 0.1f, 14.5f), new Vector3(3.833333f, 0.1f, 12.5f), Color.blue);
        Debug.DrawLine(new Vector3(4.333333f, 0.1f, 12.5f), new Vector3(3.833333f, 0.1f, 12.5f), Color.red);

        Handles.Label(new Vector3(5.388889f, 0.1f, 18.94445f), "17");
        Debug.DrawLine(new Vector3(8.333334f, 0.1f, 16.16667f), new Vector3(7.833333f, 0.1f, 16.16667f), Color.red);
        Debug.DrawLine(new Vector3(8.333334f, 0.1f, 16.16667f), new Vector3(0f, 0.1f, 24.5f), Color.blue);
        Debug.DrawLine(new Vector3(7.833333f, 0.1f, 16.16667f), new Vector3(0f, 0.1f, 24.5f), Color.blue);

        Handles.Label(new Vector3(5.833333f, 0.1f, 14.11111f), "18");
        Debug.DrawLine(new Vector3(6.833333f, 0.1f, 15.33333f), new Vector3(6.833333f, 0.1f, 14.5f), Color.red);
        Debug.DrawLine(new Vector3(6.833333f, 0.1f, 15.33333f), new Vector3(3.833333f, 0.1f, 12.5f), Color.blue);
        Debug.DrawLine(new Vector3(6.833333f, 0.1f, 14.5f), new Vector3(3.833333f, 0.1f, 12.5f), Color.blue);

        Handles.Label(new Vector3(3.555555f, 0.1f, 13.27778f), "19");
        Debug.DrawLine(new Vector3(6.833333f, 0.1f, 15.33333f), new Vector3(3.833333f, 0.1f, 12.5f), Color.blue);
        Debug.DrawLine(new Vector3(6.833333f, 0.1f, 15.33333f), new Vector3(0f, 0.1f, 12f), Color.blue);
        Debug.DrawLine(new Vector3(3.833333f, 0.1f, 12.5f), new Vector3(0f, 0.1f, 12f), Color.red);

        Handles.Label(new Vector3(4.888889f, 0.1f, 14.5f), "20");
        Debug.DrawLine(new Vector3(7.833333f, 0.1f, 16.16667f), new Vector3(6.833333f, 0.1f, 15.33333f), Color.red);
        Debug.DrawLine(new Vector3(7.833333f, 0.1f, 16.16667f), new Vector3(0f, 0.1f, 12f), Color.blue);
        Debug.DrawLine(new Vector3(6.833333f, 0.1f, 15.33333f), new Vector3(0f, 0.1f, 12f), Color.blue);

        Handles.Label(new Vector3(2.611111f, 0.1f, 17.55556f), "21");
        Debug.DrawLine(new Vector3(7.833333f, 0.1f, 16.16667f), new Vector3(0f, 0.1f, 12f), Color.blue);
        Debug.DrawLine(new Vector3(7.833333f, 0.1f, 16.16667f), new Vector3(0f, 0.1f, 24.5f), Color.blue);
        Debug.DrawLine(new Vector3(0f, 0.1f, 12f), new Vector3(0f, 0.1f, 24.5f), Color.blue);

        Handles.Label(new Vector3(-11.05555f, 0.1f, 1.888889f), "22");
        Debug.DrawLine(new Vector3(-24.33333f, 0.1f, 2.833333f), new Vector3(-3.5f, 0.1f, 2.833333f), Color.blue);
        Debug.DrawLine(new Vector3(-24.33333f, 0.1f, 2.833333f), new Vector3(-5.333332f, 0.1f, 0f), Color.blue);
        Debug.DrawLine(new Vector3(-3.5f, 0.1f, 2.833333f), new Vector3(-5.333332f, 0.1f, 0f), Color.red);

        Handles.Label(new Vector3(-18f, 0.1f, 0.9444444f), "23");
        Debug.DrawLine(new Vector3(-24.33333f, 0.1f, 2.833333f), new Vector3(-5.333332f, 0.1f, 0f), Color.blue);
        Debug.DrawLine(new Vector3(-24.33333f, 0.1f, 2.833333f), new Vector3(-24.33333f, 0.1f, 0f), Color.red);
        Debug.DrawLine(new Vector3(-5.333332f, 0.1f, 0f), new Vector3(-24.33333f, 0.1f, 0f), Color.blue);

        Handles.Label(new Vector3(-19.11111f, 0.1f, 20.72222f), "24");
        Debug.DrawLine(new Vector3(-8.666666f, 0.1f, 18.83333f), new Vector3(-24.33333f, 0.1f, 18.83333f), Color.blue);
        Debug.DrawLine(new Vector3(-8.666666f, 0.1f, 18.83333f), new Vector3(-24.33333f, 0.1f, 24.5f), Color.blue);
        Debug.DrawLine(new Vector3(-24.33333f, 0.1f, 18.83333f), new Vector3(-24.33333f, 0.1f, 24.5f), Color.red);

        Handles.Label(new Vector3(-11f, 0.1f, 22.61111f), "25");
        Debug.DrawLine(new Vector3(-8.666666f, 0.1f, 18.83333f), new Vector3(-24.33333f, 0.1f, 24.5f), Color.blue);
        Debug.DrawLine(new Vector3(-8.666666f, 0.1f, 18.83333f), new Vector3(0f, 0.1f, 24.5f), Color.blue);
        Debug.DrawLine(new Vector3(-24.33333f, 0.1f, 24.5f), new Vector3(0f, 0.1f, 24.5f), Color.red);

        Handles.Label(new Vector3(-1.499999f, 0.1f, 0.4444443f), "26");
        Debug.DrawLine(new Vector3(0f, 0.1f, 0f), new Vector3(-2.666666f, 0.1f, 0f), Color.blue);
        Debug.DrawLine(new Vector3(0f, 0.1f, 0f), new Vector3(-1.833332f, 0.1f, 1.333333f), Color.blue);
        Debug.DrawLine(new Vector3(-2.666666f, 0.1f, 0f), new Vector3(-1.833332f, 0.1f, 1.333333f), Color.red);

        Handles.Label(new Vector3(-5.277777f, 0.1f, 20.72222f), "27");
        Debug.DrawLine(new Vector3(0f, 0.1f, 24.5f), new Vector3(-7.166666f, 0.1f, 18.83333f), Color.blue);
        Debug.DrawLine(new Vector3(0f, 0.1f, 24.5f), new Vector3(-8.666666f, 0.1f, 18.83333f), Color.blue);
        Debug.DrawLine(new Vector3(-7.166666f, 0.1f, 18.83333f), new Vector3(-8.666666f, 0.1f, 18.83333f), Color.red);

        Handles.Label(new Vector3(-1.888889f, 0.1f, 7.333333f), "28");
        Debug.DrawLine(new Vector3(-5f, 0.1f, 6.666667f), new Vector3(-0.666666f, 0.1f, 7.833333f), Color.blue);
        Debug.DrawLine(new Vector3(-5f, 0.1f, 6.666667f), new Vector3(0f, 0.1f, 7.5f), Color.blue);
        Debug.DrawLine(new Vector3(-0.666666f, 0.1f, 7.833333f), new Vector3(0f, 0.1f, 7.5f), Color.red);

        Handles.Label(new Vector3(-2.277777f, 0.1f, 5.444445f), "29");
        Debug.DrawLine(new Vector3(-5f, 0.1f, 6.666667f), new Vector3(0f, 0.1f, 7.5f), Color.blue);
        Debug.DrawLine(new Vector3(-5f, 0.1f, 6.666667f), new Vector3(-1.833332f, 0.1f, 2.166667f), Color.blue);
        Debug.DrawLine(new Vector3(0f, 0.1f, 7.5f), new Vector3(-1.833332f, 0.1f, 2.166667f), Color.blue);

        Handles.Label(new Vector3(-3.166666f, 0.1f, 3.888889f), "30");
        Debug.DrawLine(new Vector3(-5f, 0.1f, 6.666667f), new Vector3(-1.833332f, 0.1f, 2.166667f), Color.blue);
        Debug.DrawLine(new Vector3(-5f, 0.1f, 6.666667f), new Vector3(-2.666666f, 0.1f, 2.833333f), Color.blue);
        Debug.DrawLine(new Vector3(-1.833332f, 0.1f, 2.166667f), new Vector3(-2.666666f, 0.1f, 2.833333f), Color.red);

        Handles.Label(new Vector3(-2.333333f, 0.1f, 9.611111f), "31");
        Debug.DrawLine(new Vector3(-1f, 0.1f, 11.5f), new Vector3(-1f, 0.1f, 10f), Color.red);
        Debug.DrawLine(new Vector3(-1f, 0.1f, 11.5f), new Vector3(-5f, 0.1f, 7.333333f), Color.blue);
        Debug.DrawLine(new Vector3(-1f, 0.1f, 10f), new Vector3(-5f, 0.1f, 7.333333f), Color.blue);

        Handles.Label(new Vector3(-4.277777f, 0.1f, 9.222222f), "32");
        Debug.DrawLine(new Vector3(-1f, 0.1f, 11.5f), new Vector3(-5f, 0.1f, 7.333333f), Color.blue);
        Debug.DrawLine(new Vector3(-1f, 0.1f, 11.5f), new Vector3(-6.833332f, 0.1f, 8.833334f), Color.blue);
        Debug.DrawLine(new Vector3(-5f, 0.1f, 7.333333f), new Vector3(-6.833332f, 0.1f, 8.833334f), Color.red);

        Handles.Label(new Vector3(-12.33333f, 0.1f, 2.888889f), "33");
        Debug.DrawLine(new Vector3(-9.166666f, 0.1f, 3f), new Vector3(-3.5f, 0.1f, 2.833333f), Color.blue);
        Debug.DrawLine(new Vector3(-9.166666f, 0.1f, 3f), new Vector3(-24.33333f, 0.1f, 2.833333f), Color.blue);
        Debug.DrawLine(new Vector3(-3.5f, 0.1f, 2.833333f), new Vector3(-24.33333f, 0.1f, 2.833333f), Color.blue);

        Handles.Label(new Vector3(-19.27777f, 0.1f, 2.944444f), "34");
        Debug.DrawLine(new Vector3(-9.166666f, 0.1f, 3f), new Vector3(-24.33333f, 0.1f, 2.833333f), Color.blue);
        Debug.DrawLine(new Vector3(-9.166666f, 0.1f, 3f), new Vector3(-24.33333f, 0.1f, 3f), Color.blue);
        Debug.DrawLine(new Vector3(-24.33333f, 0.1f, 2.833333f), new Vector3(-24.33333f, 0.1f, 3f), Color.red);

        Handles.Label(new Vector3(-7.055555f, 0.1f, 2.944444f), "35");
        Debug.DrawLine(new Vector3(-9.166666f, 0.1f, 3f), new Vector3(-8.5f, 0.1f, 3f), Color.red);
        Debug.DrawLine(new Vector3(-9.166666f, 0.1f, 3f), new Vector3(-3.5f, 0.1f, 2.833333f), Color.blue);
        Debug.DrawLine(new Vector3(-8.5f, 0.1f, 3f), new Vector3(-3.5f, 0.1f, 2.833333f), Color.blue);

        Handles.Label(new Vector3(-9.88889f, 0.1f, 11.11111f), "36");
        Debug.DrawLine(new Vector3(-9f, 0.1f, 18.5f), new Vector3(-9f, 0.1f, 8.833334f), Color.red);
        Debug.DrawLine(new Vector3(-9f, 0.1f, 18.5f), new Vector3(-11.66667f, 0.1f, 6f), Color.blue);
        Debug.DrawLine(new Vector3(-9f, 0.1f, 8.833334f), new Vector3(-11.66667f, 0.1f, 6f), Color.red);

        Handles.Label(new Vector3(-15f, 0.1f, 9.166667f), "37");
        Debug.DrawLine(new Vector3(-9f, 0.1f, 18.5f), new Vector3(-11.66667f, 0.1f, 6f), Color.blue);
        Debug.DrawLine(new Vector3(-9f, 0.1f, 18.5f), new Vector3(-24.33333f, 0.1f, 3f), Color.blue);
        Debug.DrawLine(new Vector3(-11.66667f, 0.1f, 6f), new Vector3(-24.33333f, 0.1f, 3f), Color.blue);

        Handles.Label(new Vector3(-19.22222f, 0.1f, 13.44444f), "38");
        Debug.DrawLine(new Vector3(-9f, 0.1f, 18.5f), new Vector3(-24.33333f, 0.1f, 3f), Color.blue);
        Debug.DrawLine(new Vector3(-9f, 0.1f, 18.5f), new Vector3(-24.33333f, 0.1f, 18.83333f), Color.blue);
        Debug.DrawLine(new Vector3(-24.33333f, 0.1f, 3f), new Vector3(-24.33333f, 0.1f, 18.83333f), Color.red);

        Handles.Label(new Vector3(-15.05556f, 0.1f, 3.777778f), "39");
        Debug.DrawLine(new Vector3(-11.66667f, 0.1f, 5.333333f), new Vector3(-9.166666f, 0.1f, 3f), Color.red);
        Debug.DrawLine(new Vector3(-11.66667f, 0.1f, 5.333333f), new Vector3(-24.33333f, 0.1f, 3f), Color.blue);
        Debug.DrawLine(new Vector3(-9.166666f, 0.1f, 3f), new Vector3(-24.33333f, 0.1f, 3f), Color.blue);

        Handles.Label(new Vector3(-15.88889f, 0.1f, 4.777778f), "40");
        Debug.DrawLine(new Vector3(-11.66667f, 0.1f, 6f), new Vector3(-11.66667f, 0.1f, 5.333333f), Color.red);
        Debug.DrawLine(new Vector3(-11.66667f, 0.1f, 6f), new Vector3(-24.33333f, 0.1f, 3f), Color.blue);
        Debug.DrawLine(new Vector3(-11.66667f, 0.1f, 5.333333f), new Vector3(-24.33333f, 0.1f, 3f), Color.blue);

        Handles.Label(new Vector3(-14f, 0.1f, 18.72222f), "41");
        Debug.DrawLine(new Vector3(-24.33333f, 0.1f, 18.83333f), new Vector3(-8.666666f, 0.1f, 18.83333f), Color.blue);
        Debug.DrawLine(new Vector3(-24.33333f, 0.1f, 18.83333f), new Vector3(-9f, 0.1f, 18.5f), Color.blue);
        Debug.DrawLine(new Vector3(-8.666666f, 0.1f, 18.83333f), new Vector3(-9f, 0.1f, 18.5f), Color.red);

        Handles.Label(new Vector3(-3.555555f, 0.1f, 7.277778f), "42");
        Debug.DrawLine(new Vector3(-0.666666f, 0.1f, 7.833333f), new Vector3(-5f, 0.1f, 6.666667f), Color.blue);
        Debug.DrawLine(new Vector3(-0.666666f, 0.1f, 7.833333f), new Vector3(-5f, 0.1f, 7.333333f), Color.blue);
        Debug.DrawLine(new Vector3(-5f, 0.1f, 6.666667f), new Vector3(-5f, 0.1f, 7.333333f), Color.red);

        Handles.Label(new Vector3(-2.222222f, 0.1f, 8.388888f), "43");
        Debug.DrawLine(new Vector3(-0.666666f, 0.1f, 7.833333f), new Vector3(-5f, 0.1f, 7.333333f), Color.blue);
        Debug.DrawLine(new Vector3(-0.666666f, 0.1f, 7.833333f), new Vector3(-1f, 0.1f, 10f), Color.red);
        Debug.DrawLine(new Vector3(-5f, 0.1f, 7.333333f), new Vector3(-1f, 0.1f, 10f), Color.blue);

        Handles.Label(new Vector3(-5.666667f, 0.1f, 4.166667f), "44");
        Debug.DrawLine(new Vector3(-3.5f, 0.1f, 2.833333f), new Vector3(-8.5f, 0.1f, 3f), Color.blue);
        Debug.DrawLine(new Vector3(-3.5f, 0.1f, 2.833333f), new Vector3(-5f, 0.1f, 6.666667f), Color.blue);
        Debug.DrawLine(new Vector3(-8.5f, 0.1f, 3f), new Vector3(-5f, 0.1f, 6.666667f), Color.red);

        Handles.Label(new Vector3(-3.722222f, 0.1f, 4.111111f), "45");
        Debug.DrawLine(new Vector3(-3.5f, 0.1f, 2.833333f), new Vector3(-5f, 0.1f, 6.666667f), Color.blue);
        Debug.DrawLine(new Vector3(-3.5f, 0.1f, 2.833333f), new Vector3(-2.666666f, 0.1f, 2.833333f), Color.red);
        Debug.DrawLine(new Vector3(-5f, 0.1f, 6.666667f), new Vector3(-2.666666f, 0.1f, 2.833333f), Color.blue);

        Handles.Label(new Vector3(-1.222221f, 0.1f, 1.166667f), "46");
        Debug.DrawLine(new Vector3(0f, 0.1f, 0f), new Vector3(-1.833332f, 0.1f, 1.333333f), Color.blue);
        Debug.DrawLine(new Vector3(0f, 0.1f, 0f), new Vector3(-1.833332f, 0.1f, 2.166667f), Color.blue);
        Debug.DrawLine(new Vector3(-1.833332f, 0.1f, 1.333333f), new Vector3(-1.833332f, 0.1f, 2.166667f), Color.red);

        Handles.Label(new Vector3(-0.6111106f, 0.1f, 3.222222f), "47");
        Debug.DrawLine(new Vector3(0f, 0.1f, 0f), new Vector3(-1.833332f, 0.1f, 2.166667f), Color.blue);
        Debug.DrawLine(new Vector3(0f, 0.1f, 0f), new Vector3(0f, 0.1f, 7.5f), Color.blue);
        Debug.DrawLine(new Vector3(-1.833332f, 0.1f, 2.166667f), new Vector3(0f, 0.1f, 7.5f), Color.blue);

        Handles.Label(new Vector3(-2.611111f, 0.1f, 10.77778f), "48");
        Debug.DrawLine(new Vector3(0f, 0.1f, 12f), new Vector3(-1f, 0.1f, 11.5f), Color.red);
        Debug.DrawLine(new Vector3(0f, 0.1f, 12f), new Vector3(-6.833332f, 0.1f, 8.833334f), Color.blue);
        Debug.DrawLine(new Vector3(-1f, 0.1f, 11.5f), new Vector3(-6.833332f, 0.1f, 8.833334f), Color.blue);

        Handles.Label(new Vector3(-4.555555f, 0.1f, 13.11111f), "49");
        Debug.DrawLine(new Vector3(0f, 0.1f, 12f), new Vector3(-6.833332f, 0.1f, 8.833334f), Color.blue);
        Debug.DrawLine(new Vector3(0f, 0.1f, 12f), new Vector3(-6.833332f, 0.1f, 18.5f), Color.blue);
        Debug.DrawLine(new Vector3(-6.833332f, 0.1f, 8.833334f), new Vector3(-6.833332f, 0.1f, 18.5f), Color.red);

        Handles.Label(new Vector3(-2.277777f, 0.1f, 18.33333f), "50");
        Debug.DrawLine(new Vector3(0f, 0.1f, 12f), new Vector3(-6.833332f, 0.1f, 18.5f), Color.blue);
        Debug.DrawLine(new Vector3(0f, 0.1f, 12f), new Vector3(0f, 0.1f, 24.5f), Color.blue);
        Debug.DrawLine(new Vector3(-6.833332f, 0.1f, 18.5f), new Vector3(0f, 0.1f, 24.5f), Color.blue);

        Handles.Label(new Vector3(-4.666666f, 0.1f, 20.61111f), "51");
        Debug.DrawLine(new Vector3(-6.833332f, 0.1f, 18.5f), new Vector3(-7.166666f, 0.1f, 18.83333f), Color.red);
        Debug.DrawLine(new Vector3(-6.833332f, 0.1f, 18.5f), new Vector3(0f, 0.1f, 24.5f), Color.blue);
        Debug.DrawLine(new Vector3(-7.166666f, 0.1f, 18.83333f), new Vector3(0f, 0.1f, 24.5f), Color.blue);

        Handles.Label(new Vector3(22.94444f, 0.1f, -21.55556f), "52");
        Debug.DrawLine(new Vector3(19.83333f, 0.1f, -20.16667f), new Vector3(24.5f, 0.1f, -20.16667f), Color.blue);
        Debug.DrawLine(new Vector3(19.83333f, 0.1f, -20.16667f), new Vector3(24.5f, 0.1f, -24.33333f), Color.blue);
        Debug.DrawLine(new Vector3(24.5f, 0.1f, -20.16667f), new Vector3(24.5f, 0.1f, -24.33333f), Color.red);

        Handles.Label(new Vector3(21f, 0.1f, -21.55556f), "53");
        Debug.DrawLine(new Vector3(18.66667f, 0.1f, -20.16667f), new Vector3(19.83333f, 0.1f, -20.16667f), Color.red);
        Debug.DrawLine(new Vector3(18.66667f, 0.1f, -20.16667f), new Vector3(24.5f, 0.1f, -24.33333f), Color.blue);
        Debug.DrawLine(new Vector3(19.83333f, 0.1f, -20.16667f), new Vector3(24.5f, 0.1f, -24.33333f), Color.blue);

        Handles.Label(new Vector3(13.27778f, 0.1f, -16.66667f), "54");
        Debug.DrawLine(new Vector3(3.833333f, 0.1f, -13.83333f), new Vector3(17.33333f, 0.1f, -16f), Color.blue);
        Debug.DrawLine(new Vector3(3.833333f, 0.1f, -13.83333f), new Vector3(18.66667f, 0.1f, -20.16667f), Color.blue);
        Debug.DrawLine(new Vector3(17.33333f, 0.1f, -16f), new Vector3(18.66667f, 0.1f, -20.16667f), Color.red);

        Handles.Label(new Vector3(1.777778f, 0.1f, -14.22222f), "55");
        Debug.DrawLine(new Vector3(0.6666667f, 0.1f, -15f), new Vector3(1.5f, 0.1f, -13.83333f), Color.red);
        Debug.DrawLine(new Vector3(0.6666667f, 0.1f, -15f), new Vector3(3.166667f, 0.1f, -13.83333f), Color.blue);
        Debug.DrawLine(new Vector3(1.5f, 0.1f, -13.83333f), new Vector3(3.166667f, 0.1f, -13.83333f), Color.blue);

        Handles.Label(new Vector3(7.555557f, 0.1f, -1.777777f), "56");
        Debug.DrawLine(new Vector3(10.66667f, 0.1f, 0f), new Vector3(6.333333f, 0.1f, -2.666666f), Color.blue);
        Debug.DrawLine(new Vector3(10.66667f, 0.1f, 0f), new Vector3(5.666667f, 0.1f, -2.666666f), Color.blue);
        Debug.DrawLine(new Vector3(6.333333f, 0.1f, -2.666666f), new Vector3(5.666667f, 0.1f, -2.666666f), Color.red);

        Handles.Label(new Vector3(9.277778f, 0.1f, -1.777777f), "57");
        Debug.DrawLine(new Vector3(10.66667f, 0.1f, 0f), new Vector3(10.83333f, 0.1f, -2.666666f), Color.red);
        Debug.DrawLine(new Vector3(10.66667f, 0.1f, 0f), new Vector3(6.333333f, 0.1f, -2.666666f), Color.blue);
        Debug.DrawLine(new Vector3(10.83333f, 0.1f, -2.666666f), new Vector3(6.333333f, 0.1f, -2.666666f), Color.blue);

        Handles.Label(new Vector3(12.83333f, 0.1f, -14.88889f), "58");
        Debug.DrawLine(new Vector3(3.833333f, 0.1f, -13.83333f), new Vector3(17.33333f, 0.1f, -14.83333f), Color.blue);
        Debug.DrawLine(new Vector3(3.833333f, 0.1f, -13.83333f), new Vector3(17.33333f, 0.1f, -16f), Color.blue);
        Debug.DrawLine(new Vector3(17.33333f, 0.1f, -14.83333f), new Vector3(17.33333f, 0.1f, -16f), Color.red);

        Handles.Label(new Vector3(13.72222f, 0.1f, -14.16666f), "59");
        Debug.DrawLine(new Vector3(3.833333f, 0.1f, -13.83333f), new Vector3(20f, 0.1f, -13.83333f), Color.blue);
        Debug.DrawLine(new Vector3(3.833333f, 0.1f, -13.83333f), new Vector3(17.33333f, 0.1f, -14.83333f), Color.blue);
        Debug.DrawLine(new Vector3(20f, 0.1f, -13.83333f), new Vector3(17.33333f, 0.1f, -14.83333f), Color.red);

        Handles.Label(new Vector3(22.33333f, 0.1f, -19.83334f), "60");
        Debug.DrawLine(new Vector3(24.5f, 0.1f, -20.16667f), new Vector3(19.83333f, 0.1f, -20.16667f), Color.blue);
        Debug.DrawLine(new Vector3(24.5f, 0.1f, -20.16667f), new Vector3(22.66667f, 0.1f, -19.16667f), Color.blue);
        Debug.DrawLine(new Vector3(19.83333f, 0.1f, -20.16667f), new Vector3(22.66667f, 0.1f, -19.16667f), Color.red);

        Handles.Label(new Vector3(22.55556f, 0.1f, -14.5f), "61");
        Debug.DrawLine(new Vector3(22f, 0.1f, -15.83333f), new Vector3(21.16667f, 0.1f, -13.83333f), Color.red);
        Debug.DrawLine(new Vector3(22f, 0.1f, -15.83333f), new Vector3(24.5f, 0.1f, -13.83333f), Color.blue);
        Debug.DrawLine(new Vector3(21.16667f, 0.1f, -13.83333f), new Vector3(24.5f, 0.1f, -13.83333f), Color.blue);

        Handles.Label(new Vector3(23.05556f, 0.1f, -18.38889f), "62");
        Debug.DrawLine(new Vector3(24.5f, 0.1f, -20.16667f), new Vector3(22.66667f, 0.1f, -19.16667f), Color.blue);
        Debug.DrawLine(new Vector3(24.5f, 0.1f, -20.16667f), new Vector3(22f, 0.1f, -15.83333f), Color.blue);
        Debug.DrawLine(new Vector3(22.66667f, 0.1f, -19.16667f), new Vector3(22f, 0.1f, -15.83333f), Color.red);

        Handles.Label(new Vector3(23.66667f, 0.1f, -16.61111f), "63");
        Debug.DrawLine(new Vector3(24.5f, 0.1f, -20.16667f), new Vector3(22f, 0.1f, -15.83333f), Color.blue);
        Debug.DrawLine(new Vector3(24.5f, 0.1f, -20.16667f), new Vector3(24.5f, 0.1f, -13.83333f), Color.red);
        Debug.DrawLine(new Vector3(22f, 0.1f, -15.83333f), new Vector3(24.5f, 0.1f, -13.83333f), Color.blue);

        Handles.Label(new Vector3(1.944444f, 0.1f, -13.16666f), "64");
        Debug.DrawLine(new Vector3(1.5f, 0.1f, -13.83333f), new Vector3(2f, 0.1f, -12.83333f), Color.red);
        Debug.DrawLine(new Vector3(1.5f, 0.1f, -13.83333f), new Vector3(2.333333f, 0.1f, -12.83333f), Color.blue);
        Debug.DrawLine(new Vector3(2f, 0.1f, -12.83333f), new Vector3(2.333333f, 0.1f, -12.83333f), Color.red);

        Handles.Label(new Vector3(2.333333f, 0.1f, -13.5f), "65");
        Debug.DrawLine(new Vector3(1.5f, 0.1f, -13.83333f), new Vector3(2.333333f, 0.1f, -12.83333f), Color.blue);
        Debug.DrawLine(new Vector3(1.5f, 0.1f, -13.83333f), new Vector3(3.166667f, 0.1f, -13.83333f), Color.blue);
        Debug.DrawLine(new Vector3(2.333333f, 0.1f, -12.83333f), new Vector3(3.166667f, 0.1f, -13.83333f), Color.red);

        Handles.Label(new Vector3(6.111111f, 0.1f, -8f), "66");
        Debug.DrawLine(new Vector3(7.333333f, 0.1f, -9.5f), new Vector3(5.166667f, 0.1f, -7.666666f), Color.red);
        Debug.DrawLine(new Vector3(7.333333f, 0.1f, -9.5f), new Vector3(5.833333f, 0.1f, -6.833332f), Color.blue);
        Debug.DrawLine(new Vector3(5.166667f, 0.1f, -7.666666f), new Vector3(5.833333f, 0.1f, -6.833332f), Color.red);

        Handles.Label(new Vector3(9.444446f, 0.1f, -7.722221f), "67");
        Debug.DrawLine(new Vector3(7.333333f, 0.1f, -9.5f), new Vector3(5.833333f, 0.1f, -6.833332f), Color.blue);
        Debug.DrawLine(new Vector3(7.333333f, 0.1f, -9.5f), new Vector3(15.16667f, 0.1f, -6.833332f), Color.blue);
        Debug.DrawLine(new Vector3(5.833333f, 0.1f, -6.833332f), new Vector3(15.16667f, 0.1f, -6.833332f), Color.blue);

        Handles.Label(new Vector3(23.38889f, 0.1f, -11.5f), "68");
        Debug.DrawLine(new Vector3(24.5f, 0.1f, -6.833332f), new Vector3(24.5f, 0.1f, -13.83333f), Color.red);
        Debug.DrawLine(new Vector3(24.5f, 0.1f, -6.833332f), new Vector3(21.16667f, 0.1f, -13.83333f), Color.blue);
        Debug.DrawLine(new Vector3(24.5f, 0.1f, -13.83333f), new Vector3(21.16667f, 0.1f, -13.83333f), Color.blue);

        Handles.Label(new Vector3(17.27778f, 0.1f, -9.166665f), "69");
        Debug.DrawLine(new Vector3(15.16667f, 0.1f, -6.833332f), new Vector3(16.66667f, 0.1f, -6.833332f), Color.red);
        Debug.DrawLine(new Vector3(15.16667f, 0.1f, -6.833332f), new Vector3(20f, 0.1f, -13.83333f), Color.blue);
        Debug.DrawLine(new Vector3(16.66667f, 0.1f, -6.833332f), new Vector3(20f, 0.1f, -13.83333f), Color.blue);

        Handles.Label(new Vector3(13f, 0.1f, -11.5f), "70");
        Debug.DrawLine(new Vector3(15.16667f, 0.1f, -6.833332f), new Vector3(20f, 0.1f, -13.83333f), Color.blue);
        Debug.DrawLine(new Vector3(15.16667f, 0.1f, -6.833332f), new Vector3(3.833333f, 0.1f, -13.83333f), Color.blue);
        Debug.DrawLine(new Vector3(20f, 0.1f, -13.83333f), new Vector3(3.833333f, 0.1f, -13.83333f), Color.blue);

        Handles.Label(new Vector3(8.777779f, 0.1f, -10.27778f), "71");
        Debug.DrawLine(new Vector3(15.16667f, 0.1f, -6.833332f), new Vector3(3.833333f, 0.1f, -13.83333f), Color.blue);
        Debug.DrawLine(new Vector3(15.16667f, 0.1f, -6.833332f), new Vector3(7.333333f, 0.1f, -10.16667f), Color.blue);
        Debug.DrawLine(new Vector3(3.833333f, 0.1f, -13.83333f), new Vector3(7.333333f, 0.1f, -10.16667f), Color.red);

        Handles.Label(new Vector3(20.77778f, 0.1f, -9.166665f), "72");
        Debug.DrawLine(new Vector3(16.66667f, 0.1f, -6.833332f), new Vector3(24.5f, 0.1f, -6.833332f), Color.blue);
        Debug.DrawLine(new Vector3(16.66667f, 0.1f, -6.833332f), new Vector3(21.16667f, 0.1f, -13.83333f), Color.blue);
        Debug.DrawLine(new Vector3(24.5f, 0.1f, -6.833332f), new Vector3(21.16667f, 0.1f, -13.83333f), Color.blue);

        Handles.Label(new Vector3(19.27778f, 0.1f, -11.5f), "73");
        Debug.DrawLine(new Vector3(16.66667f, 0.1f, -6.833332f), new Vector3(21.16667f, 0.1f, -13.83333f), Color.blue);
        Debug.DrawLine(new Vector3(16.66667f, 0.1f, -6.833332f), new Vector3(20f, 0.1f, -13.83333f), Color.blue);
        Debug.DrawLine(new Vector3(21.16667f, 0.1f, -13.83333f), new Vector3(20f, 0.1f, -13.83333f), Color.red);

        Handles.Label(new Vector3(9.944446f, 0.1f, -8.833334f), "74");
        Debug.DrawLine(new Vector3(7.333333f, 0.1f, -10.16667f), new Vector3(7.333333f, 0.1f, -9.5f), Color.red);
        Debug.DrawLine(new Vector3(7.333333f, 0.1f, -10.16667f), new Vector3(15.16667f, 0.1f, -6.833332f), Color.blue);
        Debug.DrawLine(new Vector3(7.333333f, 0.1f, -9.5f), new Vector3(15.16667f, 0.1f, -6.833332f), Color.blue);

        Handles.Label(new Vector3(0.2222222f, 0.1f, -18.11111f), "75");
        Debug.DrawLine(new Vector3(0f, 0.1f, -24.33333f), new Vector3(0f, 0.1f, -15f), Color.blue);
        Debug.DrawLine(new Vector3(0f, 0.1f, -24.33333f), new Vector3(0.6666667f, 0.1f, -15f), Color.blue);
        Debug.DrawLine(new Vector3(0f, 0.1f, -15f), new Vector3(0.6666667f, 0.1f, -15f), Color.red);

        Handles.Label(new Vector3(14.38889f, 0.1f, -4.277777f), "76");
        Debug.DrawLine(new Vector3(13.5f, 0.1f, -3.166666f), new Vector3(14.83333f, 0.1f, -3.166666f), Color.red);
        Debug.DrawLine(new Vector3(13.5f, 0.1f, -3.166666f), new Vector3(14.83333f, 0.1f, -6.5f), Color.blue);
        Debug.DrawLine(new Vector3(14.83333f, 0.1f, -3.166666f), new Vector3(14.83333f, 0.1f, -6.5f), Color.red);

        Handles.Label(new Vector3(8.500001f, 0.1f, -3.277777f), "77");
        Debug.DrawLine(new Vector3(7.333333f, 0.1f, -3.5f), new Vector3(7f, 0.1f, -3.166666f), Color.red);
        Debug.DrawLine(new Vector3(7.333333f, 0.1f, -3.5f), new Vector3(11.16667f, 0.1f, -3.166666f), Color.blue);
        Debug.DrawLine(new Vector3(7f, 0.1f, -3.166666f), new Vector3(11.16667f, 0.1f, -3.166666f), Color.blue);

        Handles.Label(new Vector3(8.611112f, 0.1f, -3.666666f), "78");
        Debug.DrawLine(new Vector3(7.333333f, 0.1f, -4.333332f), new Vector3(7.333333f, 0.1f, -3.5f), Color.red);
        Debug.DrawLine(new Vector3(7.333333f, 0.1f, -4.333332f), new Vector3(11.16667f, 0.1f, -3.166666f), Color.blue);
        Debug.DrawLine(new Vector3(7.333333f, 0.1f, -3.5f), new Vector3(11.16667f, 0.1f, -3.166666f), Color.blue);

        Handles.Label(new Vector3(13.16667f, 0.1f, -4.277777f), "79");
        Debug.DrawLine(new Vector3(11.16667f, 0.1f, -3.166666f), new Vector3(13.5f, 0.1f, -3.166666f), Color.red);
        Debug.DrawLine(new Vector3(11.16667f, 0.1f, -3.166666f), new Vector3(14.83333f, 0.1f, -6.5f), Color.blue);
        Debug.DrawLine(new Vector3(13.5f, 0.1f, -3.166666f), new Vector3(14.83333f, 0.1f, -6.5f), Color.blue);

        Handles.Label(new Vector3(13.72222f, 0.1f, -5.5f), "80");
        Debug.DrawLine(new Vector3(11.16667f, 0.1f, -3.166666f), new Vector3(14.83333f, 0.1f, -6.5f), Color.blue);
        Debug.DrawLine(new Vector3(11.16667f, 0.1f, -3.166666f), new Vector3(15.16667f, 0.1f, -6.833332f), Color.blue);
        Debug.DrawLine(new Vector3(14.83333f, 0.1f, -6.5f), new Vector3(15.16667f, 0.1f, -6.833332f), Color.red);

        Handles.Label(new Vector3(10.72222f, 0.1f, -5.61111f), "81");
        Debug.DrawLine(new Vector3(11.16667f, 0.1f, -3.166666f), new Vector3(15.16667f, 0.1f, -6.833332f), Color.blue);
        Debug.DrawLine(new Vector3(11.16667f, 0.1f, -3.166666f), new Vector3(5.833333f, 0.1f, -6.833332f), Color.blue);
        Debug.DrawLine(new Vector3(15.16667f, 0.1f, -6.833332f), new Vector3(5.833333f, 0.1f, -6.833332f), Color.blue);

        Handles.Label(new Vector3(8.111112f, 0.1f, -4.777777f), "82");
        Debug.DrawLine(new Vector3(11.16667f, 0.1f, -3.166666f), new Vector3(5.833333f, 0.1f, -6.833332f), Color.blue);
        Debug.DrawLine(new Vector3(11.16667f, 0.1f, -3.166666f), new Vector3(7.333333f, 0.1f, -4.333332f), Color.blue);
        Debug.DrawLine(new Vector3(5.833333f, 0.1f, -6.833332f), new Vector3(7.333333f, 0.1f, -4.333332f), Color.red);

        Handles.Label(new Vector3(19.38889f, 0.1f, -6.722221f), "83");
        Debug.DrawLine(new Vector3(24.5f, 0.1f, -6.833332f), new Vector3(16.66667f, 0.1f, -6.833332f), Color.blue);
        Debug.DrawLine(new Vector3(24.5f, 0.1f, -6.833332f), new Vector3(17f, 0.1f, -6.5f), Color.blue);
        Debug.DrawLine(new Vector3(16.66667f, 0.1f, -6.833332f), new Vector3(17f, 0.1f, -6.5f), Color.red);

        Handles.Label(new Vector3(22f, 0.1f, -4.444444f), "84");
        Debug.DrawLine(new Vector3(24.5f, 0.1f, 0f), new Vector3(24.5f, 0.1f, -6.833332f), Color.red);
        Debug.DrawLine(new Vector3(24.5f, 0.1f, 0f), new Vector3(17f, 0.1f, -6.5f), Color.blue);
        Debug.DrawLine(new Vector3(24.5f, 0.1f, -6.833332f), new Vector3(17f, 0.1f, -6.5f), Color.blue);

        Handles.Label(new Vector3(19.5f, 0.1f, -2.166667f), "85");
        Debug.DrawLine(new Vector3(24.5f, 0.1f, 0f), new Vector3(17f, 0.1f, -6.5f), Color.blue);
        Debug.DrawLine(new Vector3(24.5f, 0.1f, 0f), new Vector3(17f, 0.1f, 0f), Color.blue);
        Debug.DrawLine(new Vector3(17f, 0.1f, -6.5f), new Vector3(17f, 0.1f, 0f), Color.red);

        Handles.Label(new Vector3(9.666667f, 0.1f, -2.999999f), "86");
        Debug.DrawLine(new Vector3(10.83333f, 0.1f, -2.666666f), new Vector3(11.16667f, 0.1f, -3.166666f), Color.red);
        Debug.DrawLine(new Vector3(10.83333f, 0.1f, -2.666666f), new Vector3(7f, 0.1f, -3.166666f), Color.blue);
        Debug.DrawLine(new Vector3(11.16667f, 0.1f, -3.166666f), new Vector3(7f, 0.1f, -3.166666f), Color.blue);

        Handles.Label(new Vector3(8.055554f, 0.1f, -2.833333f), "87");
        Debug.DrawLine(new Vector3(10.83333f, 0.1f, -2.666666f), new Vector3(7f, 0.1f, -3.166666f), Color.blue);
        Debug.DrawLine(new Vector3(10.83333f, 0.1f, -2.666666f), new Vector3(6.333333f, 0.1f, -2.666666f), Color.blue);
        Debug.DrawLine(new Vector3(7f, 0.1f, -3.166666f), new Vector3(6.333333f, 0.1f, -2.666666f), Color.red);

        Handles.Label(new Vector3(2.555556f, 0.1f, -14.22222f), "88");
        Debug.DrawLine(new Vector3(0.6666667f, 0.1f, -15f), new Vector3(3.166667f, 0.1f, -13.83333f), Color.blue);
        Debug.DrawLine(new Vector3(0.6666667f, 0.1f, -15f), new Vector3(3.833333f, 0.1f, -13.83333f), Color.blue);
        Debug.DrawLine(new Vector3(3.166667f, 0.1f, -13.83333f), new Vector3(3.833333f, 0.1f, -13.83333f), Color.red);

        Handles.Label(new Vector3(7.722223f, 0.1f, -16.33333f), "89");
        Debug.DrawLine(new Vector3(0.6666667f, 0.1f, -15f), new Vector3(3.833333f, 0.1f, -13.83333f), Color.blue);
        Debug.DrawLine(new Vector3(0.6666667f, 0.1f, -15f), new Vector3(18.66667f, 0.1f, -20.16667f), Color.blue);
        Debug.DrawLine(new Vector3(3.833333f, 0.1f, -13.83333f), new Vector3(18.66667f, 0.1f, -20.16667f), Color.blue);

        Handles.Label(new Vector3(14.61111f, 0.1f, -19.83333f), "90");
        Debug.DrawLine(new Vector3(0.6666667f, 0.1f, -15f), new Vector3(18.66667f, 0.1f, -20.16667f), Color.blue);
        Debug.DrawLine(new Vector3(0.6666667f, 0.1f, -15f), new Vector3(24.5f, 0.1f, -24.33333f), Color.blue);
        Debug.DrawLine(new Vector3(18.66667f, 0.1f, -20.16667f), new Vector3(24.5f, 0.1f, -24.33333f), Color.blue);

        Handles.Label(new Vector3(8.388889f, 0.1f, -21.22222f), "91");
        Debug.DrawLine(new Vector3(0.6666667f, 0.1f, -15f), new Vector3(24.5f, 0.1f, -24.33333f), Color.blue);
        Debug.DrawLine(new Vector3(0.6666667f, 0.1f, -15f), new Vector3(0f, 0.1f, -24.33333f), Color.blue);
        Debug.DrawLine(new Vector3(24.5f, 0.1f, -24.33333f), new Vector3(0f, 0.1f, -24.33333f), Color.red);

        Handles.Label(new Vector3(5.444446f, 0.1f, -0.8888887f), "92");
        Debug.DrawLine(new Vector3(0f, 0.1f, 0f), new Vector3(10.66667f, 0.1f, 0f), Color.blue);
        Debug.DrawLine(new Vector3(0f, 0.1f, 0f), new Vector3(5.666667f, 0.1f, -2.666666f), Color.blue);
        Debug.DrawLine(new Vector3(10.66667f, 0.1f, 0f), new Vector3(5.666667f, 0.1f, -2.666666f), Color.blue);

        Handles.Label(new Vector3(1.888889f, 0.1f, -4.777779f), "93");
        Debug.DrawLine(new Vector3(0f, 0.1f, 0f), new Vector3(5.666667f, 0.1f, -2.666666f), Color.blue);
        Debug.DrawLine(new Vector3(0f, 0.1f, 0f), new Vector3(0f, 0.1f, -11.66667f), Color.blue);
        Debug.DrawLine(new Vector3(5.666667f, 0.1f, -2.666666f), new Vector3(0f, 0.1f, -11.66667f), Color.red);

        Handles.Label(new Vector3(-13.33333f, 0.1f, -2.666667f), "94");
        Debug.DrawLine(new Vector3(-24.33333f, 0.1f, 0f), new Vector3(-5.333332f, 0.1f, 0f), Color.blue);
        Debug.DrawLine(new Vector3(-24.33333f, 0.1f, 0f), new Vector3(-10.33333f, 0.1f, -8f), Color.blue);
        Debug.DrawLine(new Vector3(-5.333332f, 0.1f, 0f), new Vector3(-10.33333f, 0.1f, -8f), Color.red);

        Handles.Label(new Vector3(-19.66666f, 0.1f, -5.833333f), "95");
        Debug.DrawLine(new Vector3(-24.33333f, 0.1f, 0f), new Vector3(-10.33333f, 0.1f, -8f), Color.blue);
        Debug.DrawLine(new Vector3(-24.33333f, 0.1f, 0f), new Vector3(-24.33333f, 0.1f, -9.5f), Color.red);
        Debug.DrawLine(new Vector3(-10.33333f, 0.1f, -8f), new Vector3(-24.33333f, 0.1f, -9.5f), Color.blue);

        Handles.Label(new Vector3(-16.77778f, 0.1f, -12.94444f), "96");
        Debug.DrawLine(new Vector3(-24.33333f, 0.1f, -9.5f), new Vector3(-13.16667f, 0.1f, -14.5f), Color.blue);
        Debug.DrawLine(new Vector3(-24.33333f, 0.1f, -9.5f), new Vector3(-12.83333f, 0.1f, -14.83333f), Color.blue);
        Debug.DrawLine(new Vector3(-13.16667f, 0.1f, -14.5f), new Vector3(-12.83333f, 0.1f, -14.83333f), Color.red);

        Handles.Label(new Vector3(-20.5f, 0.1f, -16.22222f), "97");
        Debug.DrawLine(new Vector3(-24.33333f, 0.1f, -9.5f), new Vector3(-12.83333f, 0.1f, -14.83333f), Color.blue);
        Debug.DrawLine(new Vector3(-24.33333f, 0.1f, -9.5f), new Vector3(-24.33333f, 0.1f, -24.33333f), Color.red);
        Debug.DrawLine(new Vector3(-12.83333f, 0.1f, -14.83333f), new Vector3(-24.33333f, 0.1f, -24.33333f), Color.blue);

        Handles.Label(new Vector3(-4.277777f, 0.1f, -18.05555f), "98");
        Debug.DrawLine(new Vector3(-12.83333f, 0.1f, -14.83333f), new Vector3(0f, 0.1f, -15f), Color.red);
        Debug.DrawLine(new Vector3(-12.83333f, 0.1f, -14.83333f), new Vector3(0f, 0.1f, -24.33333f), Color.blue);
        Debug.DrawLine(new Vector3(0f, 0.1f, -15f), new Vector3(0f, 0.1f, -24.33333f), Color.blue);

        Handles.Label(new Vector3(-12.38889f, 0.1f, -21.16666f), "99");
        Debug.DrawLine(new Vector3(-12.83333f, 0.1f, -14.83333f), new Vector3(0f, 0.1f, -24.33333f), Color.blue);
        Debug.DrawLine(new Vector3(-12.83333f, 0.1f, -14.83333f), new Vector3(-24.33333f, 0.1f, -24.33333f), Color.blue);
        Debug.DrawLine(new Vector3(0f, 0.1f, -24.33333f), new Vector3(-24.33333f, 0.1f, -24.33333f), Color.red);

        Handles.Label(new Vector3(-10.16667f, 0.1f, -10.55556f), "100");
        Debug.DrawLine(new Vector3(-9.166666f, 0.1f, -9.5f), new Vector3(-8.5f, 0.1f, -9.5f), Color.red);
        Debug.DrawLine(new Vector3(-9.166666f, 0.1f, -9.5f), new Vector3(-12.83333f, 0.1f, -12.66667f), Color.blue);
        Debug.DrawLine(new Vector3(-8.5f, 0.1f, -9.5f), new Vector3(-12.83333f, 0.1f, -12.66667f), Color.blue);

        Handles.Label(new Vector3(-15.44444f, 0.1f, -10.55556f), "101");
        Debug.DrawLine(new Vector3(-24.33333f, 0.1f, -9.5f), new Vector3(-9.166666f, 0.1f, -9.5f), Color.blue);
        Debug.DrawLine(new Vector3(-24.33333f, 0.1f, -9.5f), new Vector3(-12.83333f, 0.1f, -12.66667f), Color.blue);
        Debug.DrawLine(new Vector3(-9.166666f, 0.1f, -9.5f), new Vector3(-12.83333f, 0.1f, -12.66667f), Color.blue);

        Handles.Label(new Vector3(-16.77778f, 0.1f, -11.72222f), "102");
        Debug.DrawLine(new Vector3(-24.33333f, 0.1f, -9.5f), new Vector3(-12.83333f, 0.1f, -12.66667f), Color.blue);
        Debug.DrawLine(new Vector3(-24.33333f, 0.1f, -9.5f), new Vector3(-13.16667f, 0.1f, -13f), Color.blue);
        Debug.DrawLine(new Vector3(-12.83333f, 0.1f, -12.66667f), new Vector3(-13.16667f, 0.1f, -13f), Color.red);

        Handles.Label(new Vector3(-16.88889f, 0.1f, -12.33333f), "103");
        Debug.DrawLine(new Vector3(-24.33333f, 0.1f, -9.5f), new Vector3(-13.16667f, 0.1f, -13f), Color.blue);
        Debug.DrawLine(new Vector3(-24.33333f, 0.1f, -9.5f), new Vector3(-13.16667f, 0.1f, -14.5f), Color.blue);
        Debug.DrawLine(new Vector3(-13.16667f, 0.1f, -13f), new Vector3(-13.16667f, 0.1f, -14.5f), Color.red);

        Handles.Label(new Vector3(-14.61111f, 0.1f, -9.222222f), "104");
        Debug.DrawLine(new Vector3(-10.33333f, 0.1f, -8.666666f), new Vector3(-9.166666f, 0.1f, -9.5f), Color.red);
        Debug.DrawLine(new Vector3(-10.33333f, 0.1f, -8.666666f), new Vector3(-24.33333f, 0.1f, -9.5f), Color.blue);
        Debug.DrawLine(new Vector3(-9.166666f, 0.1f, -9.5f), new Vector3(-24.33333f, 0.1f, -9.5f), Color.blue);

        Handles.Label(new Vector3(-15f, 0.1f, -8.722222f), "105");
        Debug.DrawLine(new Vector3(-10.33333f, 0.1f, -8f), new Vector3(-10.33333f, 0.1f, -8.666666f), Color.red);
        Debug.DrawLine(new Vector3(-10.33333f, 0.1f, -8f), new Vector3(-24.33333f, 0.1f, -9.5f), Color.blue);
        Debug.DrawLine(new Vector3(-10.33333f, 0.1f, -8.666666f), new Vector3(-24.33333f, 0.1f, -9.5f), Color.blue);

        Handles.Label(new Vector3(-7.277777f, 0.1f, -11.61111f), "106");
        Debug.DrawLine(new Vector3(-0.5f, 0.1f, -12.66667f), new Vector3(-12.83333f, 0.1f, -12.66667f), Color.red);
        Debug.DrawLine(new Vector3(-0.5f, 0.1f, -12.66667f), new Vector3(-8.5f, 0.1f, -9.5f), Color.blue);
        Debug.DrawLine(new Vector3(-12.83333f, 0.1f, -12.66667f), new Vector3(-8.5f, 0.1f, -9.5f), Color.blue);

        Handles.Label(new Vector3(-3f, 0.1f, -11.27778f), "107");
        Debug.DrawLine(new Vector3(0f, 0.1f, -11.66667f), new Vector3(-0.5f, 0.1f, -12.66667f), Color.red);
        Debug.DrawLine(new Vector3(0f, 0.1f, -11.66667f), new Vector3(-8.5f, 0.1f, -9.5f), Color.blue);
        Debug.DrawLine(new Vector3(-0.5f, 0.1f, -12.66667f), new Vector3(-8.5f, 0.1f, -9.5f), Color.blue);

        Handles.Label(new Vector3(-3.722222f, 0.1f, -7.055557f), "108");
        Debug.DrawLine(new Vector3(0f, 0.1f, -11.66667f), new Vector3(-8.5f, 0.1f, -9.5f), Color.blue);
        Debug.DrawLine(new Vector3(0f, 0.1f, -11.66667f), new Vector3(-2.666666f, 0.1f, 0f), Color.blue);
        Debug.DrawLine(new Vector3(-8.5f, 0.1f, -9.5f), new Vector3(-2.666666f, 0.1f, 0f), Color.red);

        Handles.Label(new Vector3(-0.8888887f, 0.1f, -3.88889f), "109");
        Debug.DrawLine(new Vector3(0f, 0.1f, -11.66667f), new Vector3(-2.666666f, 0.1f, 0f), Color.blue);
        Debug.DrawLine(new Vector3(0f, 0.1f, -11.66667f), new Vector3(0f, 0.1f, 0f), Color.blue);
        Debug.DrawLine(new Vector3(-2.666666f, 0.1f, 0f), new Vector3(0f, 0.1f, 0f), Color.blue);


    }

    internal Client_Cast[] GetCastMessages()
    {
        var tmp = _sendMessages.ToArray();
        _sendMessages.Clear();
        return tmp;
    }
}
