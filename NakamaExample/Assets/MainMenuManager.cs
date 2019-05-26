using Assets.Scripts.Manager;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuManager : MonoBehaviour
{
    public GameObject Panel;
    public GameObject Waiting;
    // Start is called before the first frame update
    void Start()
    {

    }

    int waitCounter = 0;
    void FixedUpdate()
    {
        if (NakamaManager.Instance.IsConnected)
        {
            Panel.gameObject.SetActive(true);
            Waiting.gameObject.SetActive(false);
        }
        else
        {
            Panel.gameObject.SetActive(false);
            Waiting.gameObject.SetActive(true);
            waitCounter++;

            if(waitCounter % 100 == 0)
            {
                Task.Run(() => NakamaManager.Instance.ConnectSocketAsync());
            }
        }
    }
}
