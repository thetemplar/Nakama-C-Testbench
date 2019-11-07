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
            foreach(var e in state.Combatlog)
            {
                if (this.m_Chat != null)
                {
                    switch (e.TypeCase) {
                        case PublicMatchState.Types.CombatLogEntry.TypeOneofCase.Area:
                        case PublicMatchState.Types.CombatLogEntry.TypeOneofCase.Aura:
                        case PublicMatchState.Types.CombatLogEntry.TypeOneofCase.Damage:
                        case PublicMatchState.Types.CombatLogEntry.TypeOneofCase.Heal:
                            this.m_Chat.ReceiveChatMessage(0, "<color=#dfe5f0><b>[SERVER]:</b> " + e.Timestamp + "</color>");
                            break;
                        case PublicMatchState.Types.CombatLogEntry.TypeOneofCase.Cast:
                        case PublicMatchState.Types.CombatLogEntry.TypeOneofCase.Interrupted:
                        case PublicMatchState.Types.CombatLogEntry.TypeOneofCase.MissedType:
                        case PublicMatchState.Types.CombatLogEntry.TypeOneofCase.SystemMessage:
                        case PublicMatchState.Types.CombatLogEntry.TypeOneofCase.None:
                        default:
                            this.m_Chat.ReceiveChatMessage(0, "<color=#f5f542><b>[SERVER]:</b> " + e.SystemMessage + "</color>");
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
