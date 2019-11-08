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
        public TextVariant m_TextVariant = TextVariant.Percent;
        public int m_TextValue = 100;
        public string m_TextValueFormat = "0";

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
                if (this.m_TextVariant == TextVariant.Percent)
                {
                    this.m_Text.text = Mathf.RoundToInt(amount * 100f).ToString() + "%";
                }
                else if (this.m_TextVariant == TextVariant.Value)
                {
                    this.m_Text.text = ((float)this.m_TextValue * amount).ToString(this.m_TextValueFormat);
                }
                else if (this.m_TextVariant == TextVariant.ValueMax)
                {
                    this.m_Text.text = ((float)this.m_TextValue * amount).ToString(this.m_TextValueFormat) + "/" + this.m_TextValue;
                }
            }
        }
    }
}
