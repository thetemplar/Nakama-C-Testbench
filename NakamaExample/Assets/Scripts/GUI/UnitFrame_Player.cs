using System;
using UnityEngine;
using UnityEngine.UI;
using DuloGames.UI.Tweens;

namespace DuloGames.UI
{

    public class UnitFrame_Player : MonoBehaviour
    {
        public enum TextVariant
        {
            Percent,
            Value,
            ValueMax
        }

        public UIProgressBar hp_bar;
        public UIProgressBar power_bar;
        public float Duration = 5f;
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

        }

        // Update is called once per frame
        void Update()
        {
            SetFillAmount(hp_bar, Player.CurrentHealth / Player.MaxHealth);
            SetFillAmount(power_bar, Player.CurrentPower / Player.MaxPower);
        }

        protected void SetFillAmount(UIProgressBar bar, float amount)
        {
            if (bar == null)
                return;

            bar.fillAmount = amount;

            if (this.m_Text != null)
            {
                this.m_HpText.text = Player.Target.CurrentHealth + "/" + Player.Target.MaxHealth;
                this.m_PowerText.text = Player.Target.CurrentPower + "/" + Player.Target.MaxPower;
            }
        }
    }
}
