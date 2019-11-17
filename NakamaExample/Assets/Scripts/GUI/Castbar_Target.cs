using System;
using UnityEngine;
using UnityEngine.UI;
using DuloGames.UI.Tweens;
using Assets.Scripts.Manager;

namespace DuloGames.UI
{
    class Castbar_Target : MonoBehaviour
    {
        [SerializeField] private UICastBar castBar;

        [SerializeField] private PlayerController player;
        private PlayerController target;

        private void Update()
        {
            //no target or other target
            if ((player.Target == null && target != null ) || player.Target != target)
            {
                Debug.Log("Lost Target");
                try
                {
                    target.StartCastEvent -= Target_StartCastEvent;
                    target.InterruptCastEvent -= Target_InterruptCastEvent;
                }
                catch { }

                target = null;
            }

            //got new target in focus
            if (player.Target != null && target == null)
            {
                Debug.Log("new target");
                player.Target.StartCastEvent += Target_StartCastEvent;
                player.Target.InterruptCastEvent += Target_InterruptCastEvent;

                target = player.Target;
            }
        }

        private void Target_StartCastEvent(object sender, EventArgs e)
        {
            if (target.castingSpell.Name != "" && !this.castBar.IsCasting)
            {
                UISpellInfo spellInfo = new UISpellInfo(target.castingSpell);

                UnityThread.executeInUpdate(() => castBar.StartCasting(spellInfo, spellInfo.CastTime, Time.time + spellInfo.CastTime));
            }
        }
        private void Target_InterruptCastEvent(object sender, EventArgs e)
        {
            UnityThread.executeInUpdate(() => this.castBar.Interrupt());
        }
    }
}
