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
                            UnityThread.executeInUpdate(() => this.m_Chat.ReceiveChatMessage(3, "<color=#bb6ad9>[" + e.Timestamp + "]: " + PlayerManager.Instance.UserNames[e.SourceId] + " damages " + PlayerManager.Instance.UserNames[e.DestinationId] + " for " + ((e.Damage.Critical > 0) ? e.Damage.Critical : e.Damage.Amount) + " in the Area of " + (e.SourceSpellEffectIdCase == PublicMatchState.Types.CombatLogEntry.SourceSpellEffectIdOneofCase.SourceSpellId ? GameManager.Instance.GameDB.Spells[e.SourceSpellId].Name : GameManager.Instance.GameDB.Effects[e.SourceEffectId].Name) + "</color>"));
                            break;
                        case PublicMatchState.Types.CombatLogEntry.TypeOneofCase.Aura:
                            UnityThread.executeInUpdate(() => this.m_Chat.ReceiveChatMessage(3, "<color=#6ad2d9>[" + e.Timestamp + "]: " + PlayerManager.Instance.UserNames[e.SourceId] + " damages " + PlayerManager.Instance.UserNames[e.DestinationId] + " for " + ((e.Damage.Critical > 0) ? e.Damage.Critical : e.Damage.Amount) + " from the Aura of " + (e.SourceSpellEffectIdCase == PublicMatchState.Types.CombatLogEntry.SourceSpellEffectIdOneofCase.SourceSpellId ? GameManager.Instance.GameDB.Spells[e.SourceSpellId].Name : GameManager.Instance.GameDB.Effects[e.SourceEffectId].Name) + "</color>"));
                            break;
                        case PublicMatchState.Types.CombatLogEntry.TypeOneofCase.Damage:
                            UnityThread.executeInUpdate(() => this.m_Chat.ReceiveChatMessage(3, "<color=#d96a6a>[" + e.Timestamp + "]: " + PlayerManager.Instance.UserNames[e.SourceId] + " damages " + PlayerManager.Instance.UserNames[e.DestinationId] + " for " + ((e.Damage.Critical > 0) ? e.Damage.Critical : e.Damage.Amount) + " with " + (e.SourceSpellEffectIdCase == PublicMatchState.Types.CombatLogEntry.SourceSpellEffectIdOneofCase.SourceSpellId ? GameManager.Instance.GameDB.Spells[e.SourceSpellId].Name : GameManager.Instance.GameDB.Effects[e.SourceEffectId].Name) + "</color>"));
                            break;
                        case PublicMatchState.Types.CombatLogEntry.TypeOneofCase.Heal:
                            UnityThread.executeInUpdate(() => this.m_Chat.ReceiveChatMessage(3, "<color=#6ad975>[" + e.Timestamp + "]: " + PlayerManager.Instance.UserNames[e.SourceId] + " heals " + PlayerManager.Instance.UserNames[e.DestinationId] + " for " + ((e.Heal.Critical > 0) ? e.Heal.Critical : e.Heal.Amount) + " with " + (e.SourceSpellEffectIdCase == PublicMatchState.Types.CombatLogEntry.SourceSpellEffectIdOneofCase.SourceSpellId ? GameManager.Instance.GameDB.Spells[e.SourceSpellId].Name : GameManager.Instance.GameDB.Effects[e.SourceEffectId].Name) + "</color>"));
                            break;
                        case PublicMatchState.Types.CombatLogEntry.TypeOneofCase.Cast:
                            switch (e.Cast.Event)
                            {
                                case PublicMatchState.Types.CombatLogEntry.Types.CombatLogEntry_Cast.Types.CombatLogEntry_Cast_Event.Start:
                                    UnityThread.executeInUpdate(() => this.m_Chat.ReceiveChatMessage(3, "<color=#d9d56a>[" + e.Timestamp + "]: " + PlayerManager.Instance.UserNames[e.SourceId] + " starts to casts " + (e.SourceSpellEffectIdCase == PublicMatchState.Types.CombatLogEntry.SourceSpellEffectIdOneofCase.SourceSpellId ? GameManager.Instance.GameDB.Spells[e.SourceSpellId].Name : GameManager.Instance.GameDB.Effects[e.SourceEffectId].Name) + "</color>"));
                                    break;
                                case PublicMatchState.Types.CombatLogEntry.Types.CombatLogEntry_Cast.Types.CombatLogEntry_Cast_Event.Interrupted:
                                    UnityThread.executeInUpdate(() => this.m_Chat.ReceiveChatMessage(3, "<color=#d9d56a>[" + e.Timestamp + "]: " + PlayerManager.Instance.UserNames[e.SourceId] + " was interrupted to casts " + (e.SourceSpellEffectIdCase == PublicMatchState.Types.CombatLogEntry.SourceSpellEffectIdOneofCase.SourceSpellId ? GameManager.Instance.GameDB.Spells[e.SourceSpellId].Name : GameManager.Instance.GameDB.Effects[e.SourceEffectId].Name) + "</color>"));
                                    break;
                                case PublicMatchState.Types.CombatLogEntry.Types.CombatLogEntry_Cast.Types.CombatLogEntry_Cast_Event.Success:
                                    UnityThread.executeInUpdate(() => this.m_Chat.ReceiveChatMessage(3, "<color=#d9d56a>[" + e.Timestamp + "]: " + PlayerManager.Instance.UserNames[e.SourceId] + " succeeded to casts " + (e.SourceSpellEffectIdCase == PublicMatchState.Types.CombatLogEntry.SourceSpellEffectIdOneofCase.SourceSpellId ? GameManager.Instance.GameDB.Spells[e.SourceSpellId].Name : GameManager.Instance.GameDB.Effects[e.SourceEffectId].Name) + "</color>"));
                                    break;
                                case PublicMatchState.Types.CombatLogEntry.Types.CombatLogEntry_Cast.Types.CombatLogEntry_Cast_Event.Failed:
                                    UnityThread.executeInUpdate(() => this.m_Chat.ReceiveChatMessage(3, "<color=#d9d56a>[" + e.Timestamp + "]: " + PlayerManager.Instance.UserNames[e.SourceId] + " failed to casts " + (e.SourceSpellEffectIdCase == PublicMatchState.Types.CombatLogEntry.SourceSpellEffectIdOneofCase.SourceSpellId ? GameManager.Instance.GameDB.Spells[e.SourceSpellId].Name : GameManager.Instance.GameDB.Effects[e.SourceEffectId].Name) + ": " + e.Cast.FailedMessage + "</color>"));
                                    break;
                            }
                            break;
                        case PublicMatchState.Types.CombatLogEntry.TypeOneofCase.Interrupted:
                            UnityThread.executeInUpdate(() => this.m_Chat.ReceiveChatMessage(3, "<color=#d9d56a>[" + e.Timestamp + "]: " + PlayerManager.Instance.UserNames[e.SourceId] + " was interrupted </color>"));
                            break;
                        case PublicMatchState.Types.CombatLogEntry.TypeOneofCase.MissedType:
                            UnityThread.executeInUpdate(() => this.m_Chat.ReceiveChatMessage(3, "<color=#d9d56a>[" + e.Timestamp + "]: " + PlayerManager.Instance.UserNames[e.SourceId] + " missed " + PlayerManager.Instance.UserNames[e.DestinationId] + " with " + (e.SourceSpellEffectIdCase == PublicMatchState.Types.CombatLogEntry.SourceSpellEffectIdOneofCase.SourceSpellId ? GameManager.Instance.GameDB.Spells[e.SourceSpellId].Name : GameManager.Instance.GameDB.Effects[e.SourceEffectId].Name) + "</color>"));
                            break;
                        case PublicMatchState.Types.CombatLogEntry.TypeOneofCase.SystemMessage:
                        case PublicMatchState.Types.CombatLogEntry.TypeOneofCase.None:
                        default:
                            UnityThread.executeInUpdate(() => this.m_Chat.ReceiveChatMessage(1, "<color=#f5f542><b>[SERVER]:</b> " + e.SystemMessage + "</color>"));
                            break;
                    }
                }
            }
        }

        public void OnSendMessage(int tabId, string text)
        {
            if (this.m_Chat != null)
            {
                UnityThread.executeInUpdate(() => this.m_Chat.ReceiveChatMessage(tabId, "<color=#" + CommonColorBuffer.ColorToString(this.m_PlayerColor) + "><b>" + this.m_PlayerName + "</b></color> <color=#59524bff>said:</color> " + text));
            }
        }
    }
}
