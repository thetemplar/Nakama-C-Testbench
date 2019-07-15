using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using static NakamaMinimalGame.PublicMatchState.PublicMatchState.Types;

public class CombatLogGUI : MonoBehaviour
{
    public static CombatLog CombatLog = new CombatLog();
    public Vector2 scrollPosition = Vector2.zero;

    void Start()
    {
        CombatLog.OnNewDamage += Log_OnNewDamage;
        PopupTextController.Init();
    }

    private void Log_OnNewDamage(string source, CombatLog.FloatingDamageEvent e)
    {
        var o = Assets.Scripts.Manager.PlayerManager.Instance.GetGameObjectPosition(e.Target);
        Debug.Log("calling CreatePopupText" + o);
        PopupTextController.CreatePopupText((Math.Round(e.Value * 100)/100).ToString(), Assets.Scripts.Manager.PlayerManager.Instance.GetGameObjectPosition(e.Target), e.Critical);
    }

    void OnGUI()
    {
        GUIStyle myStyle = new GUIStyle();
        myStyle.fontSize = 10;

        // combatLog
        GUILayout.BeginArea(new Rect(10,  10, 1000, 1000));
        GUILayout.Box("", GUILayout.Width(525), GUILayout.Height(150));
        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(20, 10, 980, 980));
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(525), GUILayout.Height(150));

        GUILayout.BeginVertical();
        var copy = CombatLog.ToArray();
        for(int i = copy.Length - 1; i >= 0; i--)
        {
            var entry = copy[i];
            switch (entry.TypeCase)
            {
                case CombatLogEntry.TypeOneofCase.Damage:
                    GUILayout.Label("<color=white><size=10>" + entry.Timestamp + ": " + entry.SourceId + " damages " + entry.DestinationId + " with " + entry.SourceSpellId + "/" + entry.SourceEffectId + " for " + entry.Damage.Amount + "</size></color>", GUILayout.Width(500));
                    GUILayout.Space(-10);
                    break;
                case CombatLogEntry.TypeOneofCase.Cast:
                    GUILayout.Label("<color=blue><size=10>" + entry.Timestamp + ": " + entry.SourceId + " casts " + entry.SourceSpellId + "/" + entry.SourceEffectId + " against " + entry.DestinationId + ": " + entry.Cast.Event + " " + entry.Cast.FailedMessage + "</size></color>", GUILayout.Width(500));
                    GUILayout.Space(-10);
                    break;
                case CombatLogEntry.TypeOneofCase.Aura:
                    GUILayout.Label("<color=yellow><size=10>" + entry.Timestamp + ": " + entry.SourceId + " " + entry.Aura.Event + " " + entry.SourceSpellId + "/" + entry.SourceEffectId + " on " + entry.DestinationId + "</size></color>", GUILayout.Width(500));
                    GUILayout.Space(-10);
                    break;
                case CombatLogEntry.TypeOneofCase.SystemMessage:
                    GUILayout.Label("<color=red><size=10>" + entry.SystemMessage + "</size></color>", GUILayout.Width(500));
                    GUILayout.Space(-10);
                    break;
            }
        }

        GUILayout.EndVertical();
        GUILayout.EndScrollView();

        GUILayout.EndArea();
    }
}
