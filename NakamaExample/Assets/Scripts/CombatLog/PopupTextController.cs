using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupTextController : MonoBehaviour
{
    private static GameObject popupText;
    private static GameObject canvas;
    
    public static void Init()
    {
        if (!popupText)
            popupText = Resources.Load("Popup2DTextParent") as GameObject;
    }

    public static void CreatePopupText(string text, Vector3 location, bool crit)
    {
        UnityThread.executeInUpdate(() => {
            Debug.Log("CreatePopupText" + location);
            GameObject instance = Instantiate(popupText);
            //Vector2 pos = Camera.main.WorldToScreenPoint(location + new Vector3(0,2,0));//(new Vector2(location.x + Random.Range(-.5f, .5f), location.y + Random.Range(-.5f, .5f)));

            instance.transform.position = location + new Vector3(0,2,0);
            instance.GetComponentInChildren<TextMesh>().text = text;
            if(crit)
            {
                instance.GetComponentInChildren<TextMesh>().color = Color.red;
                instance.GetComponentInChildren<Animator>().speed = 0.5f;
                instance.transform.Find("Popup2DText").transform.localScale = new Vector3(2,2,2);
            }
            
            var info = instance.GetComponentInChildren<Animator>().GetCurrentAnimatorClipInfo(0);
            Destroy(instance, info[0].clip.length*.9f);
        });
    }
}
