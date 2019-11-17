using UnityEngine;
using System;

namespace DuloGames.UI
{
	[Serializable]
	public class UISpellInfo
	{
		public int ID;
		public string Name;
		public Sprite Icon;
		public string Description;
		public float Range;
		public float Cooldown;
		public float CastTime;
		public float PowerCost;
	
		[BitMask(typeof(UISpellInfo_Flags))]
		public UISpellInfo_Flags Flags;

        public UISpellInfo()
        {
        }

        public UISpellInfo(GameDB_Lib.GameDB_Spell spell)
        {
            Icon = IconStore.Instance.Spellicon[(int)spell.IconID - 1];
            Name = spell.Name;
            Description = spell.Description;
            Cooldown = spell.Cooldown;
            Range = spell.Range;
            CastTime = spell.CastTime;
            PowerCost = spell.BaseCost;
            ID = (int)spell.Id;
        }
    }
}
