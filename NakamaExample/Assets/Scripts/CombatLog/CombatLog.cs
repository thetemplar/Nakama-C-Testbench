using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NakamaMinimalGame.PublicMatchState.PublicMatchState.Types;

public class CombatLog: IEnumerable
{
    private static List<CombatLogEntry> _combatLogList = new List<CombatLogEntry>();
    
    public class FloatingDamageEvent : EventArgs
    {
        public float Value;
        public string Target;
        public bool Critical;
    }

    public delegate void FloatingDamageEventHandler(string source, FloatingDamageEvent e);
    public event FloatingDamageEventHandler OnNewDamage;

    IEnumerator IEnumerable.GetEnumerator()
    {
       return (IEnumerator) GetEnumerator();
    }

    public IEnumerator GetEnumerator()
    {
        return _combatLogList.GetEnumerator();
    }

    public void Add(CombatLogEntry entry)
    {
        _combatLogList.Add(entry);
        if((entry.SourceId == Assets.Scripts.Manager.NakamaManager.Instance.Session.UserId || entry.DestinationId == Assets.Scripts.Manager.NakamaManager.Instance.Session.UserId) && entry.TypeCase == CombatLogEntry.TypeOneofCase.Damage)
        {
            if(entry.Damage.Critical > 0)
                OnNewDamage?.Invoke(entry.SourceId, new FloatingDamageEvent{Target = entry.DestinationId, Value = entry.Damage.Amount + entry.Damage.Critical, Critical = true});
            else
                OnNewDamage?.Invoke(entry.SourceId, new FloatingDamageEvent{Target = entry.DestinationId, Value = entry.Damage.Amount, Critical = false});
        }
    }

    public void AddRange(IEnumerable<CombatLogEntry> range)
    {
        foreach(var entry in range)
        {
            Add(entry);
        }
    }

    public CombatLogEntry[] ToArray()
    {
        return _combatLogList.ToArray();
    }
}
