using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NakamaMinimalGame.PublicMatchState.PublicMatchState.Types;

public class CombatLog : MonoBehaviour
{
    public static List<CombatLogEntry> CombatLogList = new List<CombatLogEntry>();
    public Vector2 scrollPosition = Vector2.zero;

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
        var copy = CombatLogList.ToArray();
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
