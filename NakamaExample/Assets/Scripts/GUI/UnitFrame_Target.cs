using System;
using UnityEngine;
using UnityEngine.UI;
using DuloGames.UI.Tweens;

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
        public TextVariant m_TextVariant = TextVariant.Percent;
        public int m_TextValue = 100;
        public string m_TextValueFormat = "0";

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
                m_Text.text = Player.Target.name;
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
