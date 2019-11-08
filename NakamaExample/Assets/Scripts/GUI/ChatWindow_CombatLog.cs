using System;
using Assets.Scripts.Manager;
using NakamaMinimalGame.PublicMatchState;
using UnityEngine;

namespace DuloGames.UI
{
    public class ChatWindow_CombatLog : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField] private Demo_Chat m_Chat;
        [SerializeField] private string m_PlayerName = "Player";
        [SerializeField] private Color m_PlayerColor = Color.white;
#pragma warning restore 0649

        public CombatLog CombatLog = new CombatLog();

        private void Start()
        {
            GameManager.Instance.OnNewWorldUpdate += OnNewWorldUpdate;
            //PopupTextController.CreatePopupText((Math.Round(e.Value * 100)/100).ToString(), Assets.Scripts.Manager.PlayerManager.Instance.GetGameObjectPosition(e.Target), e.Critical);
            
        }
        private void OnNewWorldUpdate(PublicMatchState state, float diffTime)
        {
            
            foreach (var e in state.Combatlog)
            {
                if (this.m_Chat != null)
                {
                    switch (e.TypeCase) {
                        case PublicMatchState.Types.CombatLogEntry.TypeOneofCase.Area:
                            this.m_Chat.ReceiveChatMessage(3, "<color=#dfe5f0>[" + e.Timestamp + "]: " + e.Source + " damages " + e.DestinationId + " for " + ((e.Damage.Critical > 0) ? e.Damage.Critical : e.Damage.Amount) + " in the Area of " + (e.SourceSpellEffectIdCase == PublicMatchState.Types.CombatLogEntry.SourceSpellEffectIdOneofCase.SourceSpellId ? GameManager.Instance.GameDB.Spells[e.SourceSpellId].Name : GameManager.Instance.GameDB.Effects[e.SourceEffectId].Name) + "</color>");
                            break;
                        case PublicMatchState.Types.CombatLogEntry.TypeOneofCase.Aura:
                            this.m_Chat.ReceiveChatMessage(3, "<color=#dfe5f0>[" + e.Timestamp + "]: " + e.Source + " damages " + e.DestinationId + " for " + ((e.Damage.Critical > 0) ? e.Damage.Critical : e.Damage.Amount) + " from the Aura of " + (e.SourceSpellEffectIdCase == PublicMatchState.Types.CombatLogEntry.SourceSpellEffectIdOneofCase.SourceSpellId ? GameManager.Instance.GameDB.Spells[e.SourceSpellId].Name : GameManager.Instance.GameDB.Effects[e.SourceEffectId].Name) + "</color>");
                            break;
                        case PublicMatchState.Types.CombatLogEntry.TypeOneofCase.Damage:
                            this.m_Chat.ReceiveChatMessage(3, "<color=#dfe5f0>[" + e.Timestamp + "]: " + e.Source + " damages " + e.DestinationId + " for " + ((e.Damage.Critical > 0) ? e.Damage.Critical : e.Damage.Amount) + " with " + (e.SourceSpellEffectIdCase == PublicMatchState.Types.CombatLogEntry.SourceSpellEffectIdOneofCase.SourceSpellId ? GameManager.Instance.GameDB.Spells[e.SourceSpellId].Name : GameManager.Instance.GameDB.Effects[e.SourceEffectId].Name) + "</color>");
                            break;
                        case PublicMatchState.Types.CombatLogEntry.TypeOneofCase.Heal:
                            this.m_Chat.ReceiveChatMessage(3, "<color=#dfe5f0>[" + e.Timestamp + "]: " + e.Source + " heals " + e.DestinationId + " for " + ((e.Damage.Critical > 0) ? e.Damage.Critical : e.Damage.Amount) + " with " + (e.SourceSpellEffectIdCase == PublicMatchState.Types.CombatLogEntry.SourceSpellEffectIdOneofCase.SourceSpellId ? GameManager.Instance.GameDB.Spells[e.SourceSpellId].Name : GameManager.Instance.GameDB.Effects[e.SourceEffectId].Name) + "</color>");
                            break;
                        case PublicMatchState.Types.CombatLogEntry.TypeOneofCase.Cast:
                            break;
                        case PublicMatchState.Types.CombatLogEntry.TypeOneofCase.Interrupted:
                            break;
                        case PublicMatchState.Types.CombatLogEntry.TypeOneofCase.MissedType:
                            this.m_Chat.ReceiveChatMessage(3, "<color=#dfe5f0>[" + e.Timestamp + "]: " + e.Source + " missed " + e.DestinationId + " with " + (e.SourceSpellEffectIdCase == PublicMatchState.Types.CombatLogEntry.SourceSpellEffectIdOneofCase.SourceSpellId ? GameManager.Instance.GameDB.Spells[e.SourceSpellId].Name : GameManager.Instance.GameDB.Effects[e.SourceEffectId].Name) + "</color>");
                            break;
                        case PublicMatchState.Types.CombatLogEntry.TypeOneofCase.SystemMessage:
                        case PublicMatchState.Types.CombatLogEntry.TypeOneofCase.None:
                        default:
                            this.m_Chat.ReceiveChatMessage(1, "<color=#f5f542><b>[SERVER]:</b> " + e.SystemMessage + "</color>");
                            break;
                    }
                }
            }
        }

        public void OnSendMessage(int tabId, string text)
        {
            if (this.m_Chat != null)
            {
                this.m_Chat.ReceiveChatMessage(tabId, "<color=#" + CommonColorBuffer.ColorToString(this.m_PlayerColor) + "><b>" + this.m_PlayerName + "</b></color> <color=#59524bff>said:</color> " + text);
            }
        }
    }
}
