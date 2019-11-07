using Assets.Scripts.Manager;
using UnityEngine;
using static DuloGames.UI.UISpellSlot;

namespace DuloGames.UI
{
    public class ActionBar : MonoBehaviour
    {
        // Start is called before the first frame update
        public UISpellSlot[] _spellslot = new UISpellSlot[8];
        public IconStore IconStore;

        void Start()
        {
        }

        private bool _initiatedButtonBar;
        // Update is called once per frame
        void Update()
        {
            if (!_initiatedButtonBar && PlayerManager.Instance.Spawned)
            {
                _spellslot[0].Assign(new UISpellInfo
                {
                    Icon = IconStore.MeeleAutoattack,
                    Name = "Autoattack",
                    Description = "Autoattack"
                });
                _spellslot[0].onClick.AddListener(OnSpellClick);

                for (int i = 1; i < GameManager.Instance.GameDB.Classes[PlayerManager.Instance.ClassName].Spells.Length && i < 7; i++)
                {
                    GameDB_Lib.GameDB_Spell spell = GameManager.Instance.GameDB.Classes[PlayerManager.Instance.ClassName].Spells[i-1];
                    _spellslot[i].Assign(new UISpellInfo
                    {
                        Icon = IconStore.Spellicon[(int)spell.IconID-1],
                        Name = spell.Name,
                        Description = spell.Description,
                        Cooldown = spell.Cooldown,
                        Range = spell.Range,
                        CastTime = spell.CastTime,
                        PowerCost = spell.BaseCost
                    });
                    _spellslot[i].onClick.AddListener(OnSpellClick);
                }



                _initiatedButtonBar = true;
            }
        }

        public void OnSpellClick(UISpellSlot slot)
        {
            Debug.Log("clicked" + slot.GetSpellInfo().Name);
        }
    }
}