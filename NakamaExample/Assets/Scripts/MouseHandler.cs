using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHandler : MonoBehaviour
{
    public PlayerController Player;
    private bool placeSpellCursor = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (placeSpellCursor)
                placeSpellCursor = false;
            else
                Player.Target = null;
        }
        if (Input.GetMouseButton(0))
        {
            if (placeSpellCursor)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    //CastSpell(placeSpellCursor_Spell, placeSpellCursor_Button, new Vector2(hit.point.x, hit.point.z));
                }
                placeSpellCursor = false;
            }
            else
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    GameObject go = hit.transform.root.gameObject;
                    if (go.layer == 8)
                    {
                        Player.Target = go.GetComponent<PlayerController>();
                    }
                }
            }
        }
        else if (Input.GetMouseButton(1))
        {
            if (placeSpellCursor)
            {
                //UnityThread.executeInUpdate(() => placeSpellCursor_Button.GetComponent<Image>().color = Color.white);
                placeSpellCursor = false;
            }
        }
    }
}
