//
// Copyright (c) BTG. All rights reserved.
//

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BTG
{
    /// <summary>
    /// PlayerHUD_UI 
    /// </summary>
    public class PlayerHUD_UI : MonoBehaviour
    {
        [Header("Player Bars")] [SerializeField]
        private Image _healthBar;

        [SerializeField] private Image _fuelBar;

        [Header("Boss Bars")] [SerializeField] private Image _BossBar1;
        [SerializeField] private Image _BossBar2;

        [Header("Ability Wheel")] [SerializeField]
        private Image _topCD;

        [SerializeField] private Image _downCD;
        [SerializeField] private Image _leftCD;
        [SerializeField] private Image _rightCD;
        [SerializeField] private Image _topSelected;
        [SerializeField] private Image _botSelected;
        [SerializeField] private Image _leftSelected;
        [SerializeField] private Image _rightSelected;
        [SerializeField] private Image _topSelectedCD;
        [SerializeField] private Image _botSelectedCD;
        [SerializeField] private Image _leftSelectedCD;
        [SerializeField] private Image _rightSelectedCD;
        [SerializeField] private TextMeshProUGUI _gearTossCDText;
        [SerializeField] private TextMeshProUGUI _heatWaveCDText;
        [SerializeField] private TextMeshProUGUI _slashCDText;
        [SerializeField] private TextMeshProUGUI _fireBlazeCDText;

        private Player player;

        private void Awake()
        {
            this.player = FindAnyObjectByType<Player>();
        }

        private void Update()
        {
            this.UpdateUI();
        }

        private void UpdateUI()
        {
            this._healthBar.fillAmount = this.player.CurrentHealth / this.player.Data.Health;
            this._fuelBar.fillAmount = this.player.CurrentAttackFuelAmount / this.player.Data.MaxAttackFuelAmount;

            // some abilities don't have a cooldown, so we'll use animation time remaining instead
            var currentAnim = this.player.Anim.GetCurrentAnimatorStateInfo(0);
            var currentAnimHash = currentAnim.shortNameHash;
            var animFracRemaining = 1 - currentAnim.normalizedTime;
            Image cdImage = null;
            Image cdSelected = null;
            TextMeshProUGUI cdText = null;
            var animatingCooldown = true;
            // Heat Wave
            if (currentAnimHash == Animator.StringToHash(nameof(Player.State.HeatWave)))
            {
                cdImage = this._topCD;
                cdSelected = this._topSelectedCD;
                cdText = this._heatWaveCDText;
            }
            // Slash
            else if (currentAnimHash == Animator.StringToHash("Slash") ||
                     currentAnimHash == Animator.StringToHash("Slash_1") ||
                     currentAnimHash == Animator.StringToHash("Slash_2"))
            {
                cdImage = this._downCD;
                cdSelected = this._botSelectedCD;
                cdText = this._slashCDText;
            }
            // FireBlaze
            else if (currentAnimHash == Animator.StringToHash("ChargeUp") ||
                     currentAnimHash == Animator.StringToHash(nameof(Player.State.FireBlaze)))
            {
                cdImage = this._leftCD;
                cdSelected = this._leftSelectedCD;
                cdText = this._fireBlazeCDText;
            }
            else
            {
                animatingCooldown = false;
                this._leftCD.fillAmount = 0;
                this._downCD.fillAmount = 0;
                this._topCD.fillAmount = 0;
                this._leftSelectedCD.fillAmount = 0;
                this._botSelectedCD.fillAmount = 0;
                this._topSelectedCD.fillAmount = 0;
                if (this._fireBlazeCDText != null)
                {
                    this._fireBlazeCDText.text = string.Empty;
                }

                if (this._slashCDText != null)
                {
                    this._slashCDText.text = string.Empty;
                }

                if (this._heatWaveCDText != null)
                {
                    this._heatWaveCDText.text = string.Empty;
                }
            }

            if (animatingCooldown)
            {
                if (cdImage.fillMethod == Image.FillMethod.Vertical ||
                    cdImage.fillMethod == Image.FillMethod.Horizontal)
                {
                    cdImage.fillAmount = animFracRemaining;
                }
                else
                {
                    cdImage.fillAmount =
                        animFracRemaining * 0.6f +
                        0.2f; // because of the shape of the images (at time of writing), the first and last 20% or so do nothing, so we scale it to be between 0.2 and 0.8
                }

                cdSelected.fillAmount = cdImage.fillAmount;
                if (cdText != null)
                {
                    cdText.text = this.FormatCooldown(animFracRemaining * currentAnim.length);
                }
            }

            // Gear Toss
            if (currentAnimHash != Animator.StringToHash("GearToss"))
            {
                animFracRemaining = 0f;
            }

            const float tossAnimLength = 0.75f;
            var gearTossState = this.player.states[Player.State.GearToss] as PlayerGearTossState;
            var totalCD = this.player.Data.GearShootCoolDown + tossAnimLength;
            var gearTossCDLeft = animFracRemaining == 0f
                ? gearTossState.LastUsedTime + this.player.Data.GearShootCoolDown - Time.time
                : this.player.Data.GearShootCoolDown + animFracRemaining * tossAnimLength;
            if (this._rightCD.fillMethod == Image.FillMethod.Vertical || this._rightCD.fillMethod == Image.FillMethod.Horizontal)
            {
                this._rightCD.fillAmount = gearTossCDLeft / totalCD;
            }
            else
            {
                this._rightCD.fillAmount = gearTossCDLeft / totalCD * 0.6f + 0.2f; // see above
            }

            this._rightSelectedCD.fillAmount = this._rightCD.fillAmount;
            if (this._gearTossCDText != null)
            {
                this._gearTossCDText.text = this.FormatCooldown(gearTossCDLeft);
            }

            // Switched Selected Images
            this.SelectAbility(this.player.CurrentAbility == Player.State.HeatWave, this._topSelected, this._topSelectedCD);
            this.SelectAbility(this.player.CurrentAbility == Player.State.Slash, this._botSelected, this._botSelectedCD);
            this.SelectAbility(this.player.CurrentAbility == Player.State.FireBlaze, this._leftSelected, this._leftSelectedCD);
            this.SelectAbility(this.player.CurrentAbility == Player.State.GearToss, this._rightSelected, this._rightSelectedCD);
        }

        private void SelectAbility(bool action, Image image1, Image image2)
        {
            if (action)
            {
                this.SetSelectedInactive();
                image1.gameObject.SetActive(true);
                image2.gameObject.SetActive(true);
            }
        }

        private void SetSelectedInactive()
        {
            this._topSelected.gameObject.SetActive(false);
            this._botSelected.gameObject.SetActive(false);
            this._leftSelected.gameObject.SetActive(false);
            this._rightSelected.gameObject.SetActive(false);
            this._topSelectedCD.gameObject.SetActive(false);
            this._botSelectedCD.gameObject.SetActive(false);
            this._leftSelectedCD.gameObject.SetActive(false);
            this._rightSelectedCD.gameObject.SetActive(false);
        }

        private string FormatCooldown(float cooldown)
        {
            if (cooldown <= 0)
            {
                return "";
            }

            if (cooldown < 0.9f)
                // display one decimal after the zero
            {
                return "0." + Mathf.CeilToInt(cooldown * 10f);
            }

            return Mathf.CeilToInt(cooldown).ToString();
        }
    }
}
