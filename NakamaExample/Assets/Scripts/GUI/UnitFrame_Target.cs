using System;
using UnityEngine;
using UnityEngine.UI;
using DuloGames.UI.Tweens;
using Assets.Scripts.Manager;

namespace DuloGames.UI
{

    public class UnitFrame_Target : MonoBehaviour
    {
        public enum TextVariant
        {
            Percent,
            Value,
            ValueMax
        }

        public UIProgressBar hp_bar;
        public UIProgressBar power_bar;
        public TweenEasing Easing = TweenEasing.InOutQuint;
        public Text m_Text;
        public Text m_HpText;
        public Text m_PowerText;

        public PlayerController Player;

        // Tween controls
        [NonSerialized] private readonly TweenRunner<FloatTween> m_FloatTweenRunner;

        // Start is called before the first frame update
        void Start()
        {
            this.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            if (Player.Target != null)
            {
                if(!this.gameObject.transform.GetChild(0).gameObject.activeSelf)
                    this.gameObject.transform.GetChild(0).gameObject.SetActive(true);

                SetFillAmount(hp_bar, Player.Target.CurrentHealth / Player.Target.MaxHealth);
                SetFillAmount(power_bar, Player.Target.CurrentPower / Player.Target.MaxPower);
                
                m_Text.text = PlayerManager.Instance.UserNames[Player.Target.name];
            }
            else
                this.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }

        protected void SetFillAmount(UIProgressBar bar, float amount)
        {
            if (bar == null)
                return;

            bar.fillAmount = amount;

            if (this.m_Text != null)
            {
                this.m_HpText.text = Math.Round(Player.Target.CurrentHealth / Player.Target.MaxHealth * 100, 2).ToString() + "%";
                this.m_PowerText.text = Math.Round(Player.Target.CurrentPower / Player.Target.MaxPower * 100, 2).ToString() + "%";
            }
        }
    }
}
