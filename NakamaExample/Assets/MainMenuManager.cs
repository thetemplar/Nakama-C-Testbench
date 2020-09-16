using Assets.Scripts.Manager;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public GameObject Panel;
    public GameObject Waiting;
    // Start is called before the first frame update
    void Start()
    {
        var refresh = GameObject.Find("Start_Join");
        var bt = refresh.GetComponent<Button>();
        bt.onClick.AddListener(delegate { NakamaManager.Instance.LoadMatch(); });
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

            if(waitCounter % 50 == 0)
            {
                Task.Run(() => NakamaManager.Instance.ConnectSocketAsync());
            }
        }
    }
}
