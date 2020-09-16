using System;
using UnityEngine;
using UnityEngine.UI;
using DuloGames.UI.Tweens;
using Assets.Scripts.Manager;

namespace DuloGames.UI
{
    class Castbar_Player : MonoBehaviour
    {
        [SerializeField] private UICastBar castBar;

        [SerializeField] private PlayerController player;

        private void Start()
        {
            player.StartCastEvent += Player_StartCastEvent;
            player.InterruptCastEvent += Player_InterruptCastEvent;
        }

        private void Player_StartCastEvent(object sender, EventArgs e)
        {
            if (player.castingSpell.Name != "" && !this.castBar.IsCasting)
            {
                UISpellInfo spellInfo = new UISpellInfo(player.castingSpell);

                UnityThread.executeInUpdate(() => castBar.StartCasting(spellInfo, spellInfo.CastTime, Time.time + spellInfo.CastTime));
            }
        }
        private void Player_InterruptCastEvent(object sender, EventArgs e)
        {
            UnityThread.executeInUpdate(() => this.castBar.Interrupt());
        }
    }
}
