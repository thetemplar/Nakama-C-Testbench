using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class GameDB : MonoBehaviour
{
    bool IsInitialized;
    
    void FixedUpdate()
    {
        if(Assets.Scripts.Manager.NakamaManager.Instance.IsConnected && !IsInitialized)
        {
            Task.Run(async ()  => {
                var dbString = await Assets.Scripts.Manager.NakamaManager.Instance.GetGameDatabase();

                Debug.Log(dbString);
                IsInitialized = true;
            });
        }
    }
}
