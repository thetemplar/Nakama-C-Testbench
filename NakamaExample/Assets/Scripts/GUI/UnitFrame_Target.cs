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

        [SerializeField] private PlayerController player;
        private PlayerController target;

        public GridLayoutGroup Buffs;
        public GameObject Buff;

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
            //no target or other target
            if ((player.Target == null && target != null) || player.Target != target)
            {
                try
                {
                    target.GotAura -= Player_GotAura;
                    target.LostAura -= Player_LostAura;
                }
                catch { }

                target = null;

                UnityThread.executeInUpdate(() =>
                {
                    foreach (Transform child in Buffs.transform)
                        Destroy(child.gameObject);
                });
            }

            //got new target in focus
            if (player.Target != null && target == null)
            {
                player.Target.GotAura += Player_GotAura;
                player.Target.LostAura += Player_LostAura;

                target = player.Target;

                UnityThread.executeInUpdate(() =>
                {
                    foreach (var aura in target.Auras)
                    {
                        GameObject go = Buff;
                        long id = (long)aura.EffectId;
                        var sprite = IconStore.Instance.Spellicon[(int)GameManager.Instance.GameDB.Effects[id].IconID];
                        go.GetComponent<Image>().sprite = sprite;
                        go.name = "effect_" + id.ToString();
                        var InfoObject = Instantiate(go, Buffs.transform, false);
                    }
                });
            }

            //hpbar
            if (target != null)
            {
                if(!this.gameObject.transform.GetChild(0).gameObject.activeSelf)
                    this.gameObject.transform.GetChild(0).gameObject.SetActive(true);

                SetFillAmount(hp_bar, player.Target.CurrentHealth / player.Target.MaxHealth);
                SetFillAmount(power_bar, player.Target.CurrentPower / player.Target.MaxPower);

                if(PlayerManager.Instance.UserNames.ContainsKey(player.Target.name))
                    m_Text.text = PlayerManager.Instance.UserNames[player.Target.name];
            }

            //hide target-frame
            if (target == null)
                this.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }

        private void Player_LostAura(object sender, EventArgs e)
        {
            UnityThread.executeInUpdate(() =>
            {
                long id = (long)sender;
                var go = Buffs.transform.Find("effect_" + id.ToString() + "(Clone)");
                Destroy(go.gameObject);
            });
        }

        private void Player_GotAura(object sender, EventArgs e)
        {
            UnityThread.executeInUpdate(() =>
            {
                GameObject go = Buff;
                long id = (long)sender;
                var sprite = IconStore.Instance.Spellicon[(int)GameManager.Instance.GameDB.Effects[id].IconID];
                go.GetComponent<Image>().sprite = sprite;
                go.name = "effect_" + id.ToString();
                var InfoObject = Instantiate(go, Buffs.transform, false);
            });
        }

        protected void SetFillAmount(UIProgressBar bar, float amount)
        {
            if (bar == null)
                return;

            bar.fillAmount = amount;

            if (this.m_Text != null)
            {
                this.m_HpText.text = Math.Round(player.Target.CurrentHealth / player.Target.MaxHealth * 100, 2).ToString() + "%";
                this.m_PowerText.text = Math.Round(player.Target.CurrentPower / player.Target.MaxPower * 100, 2).ToString() + "%";
            }
        }
    }
}
