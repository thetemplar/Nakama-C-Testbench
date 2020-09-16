using Assets.Scripts.NakamaManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconStore : Singleton<IconStore>
{
    public List<Sprite> Spellicon = new List<Sprite>();
    public Sprite MeeleAutoattack;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
