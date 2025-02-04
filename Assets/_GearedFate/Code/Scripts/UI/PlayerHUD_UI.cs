//
// Copyright (c) BTG. All rights reserved.
//

using System;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
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
            player = FindAnyObjectByType<Player>();
        }

        private void Update()
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            _healthBar.fillAmount = player.CurrentHealth / player.Data.Health;
            _fuelBar.fillAmount = player.CurrentAttackFuelAmount / player.Data.MaxAttackFuelAmount;

            // some abilities don't have a cooldown, so we'll use animation time remaining instead
            var currentAnim = player.Anim.GetCurrentAnimatorStateInfo(0);
            var currentAnimHash = currentAnim.shortNameHash;
            var animFracRemaining = 1 - currentAnim.normalizedTime;
            Image cdImage = null;
            Image cdSelected = null;
            TextMeshProUGUI cdText = null;
            var animatingCooldown = true;
            // Heat Wave
            if (currentAnimHash == Animator.StringToHash(nameof(Player.State.HeatWave)))
            {
                cdImage = _topCD;
                cdSelected = _topSelectedCD;
                cdText = _heatWaveCDText;
            }
            // Slash
            else if (currentAnimHash == Animator.StringToHash("Slash") ||
                     currentAnimHash == Animator.StringToHash("Slash_1") ||
                     currentAnimHash == Animator.StringToHash("Slash_2"))
            {
                cdImage = _downCD;
                cdSelected = _botSelectedCD;
                cdText = _slashCDText;
            }
            // FireBlaze
            else if (currentAnimHash == Animator.StringToHash("ChargeUp") ||
                     currentAnimHash == Animator.StringToHash(nameof(Player.State.FireBlaze)))
            {
                cdImage = _leftCD;
                cdSelected = _leftSelectedCD;
                cdText = _fireBlazeCDText;
            }
            else
            {
                animatingCooldown = false;
                _leftCD.fillAmount = 0;
                _downCD.fillAmount = 0;
                _topCD.fillAmount = 0;
                _leftSelectedCD.fillAmount = 0;
                _botSelectedCD.fillAmount = 0;
                _topSelectedCD.fillAmount = 0;
                if (_fireBlazeCDText != null)
                    _fireBlazeCDText.text = string.Empty;
                if (_slashCDText != null)
                    _slashCDText.text = string.Empty;
                if (_heatWaveCDText != null)
                    _heatWaveCDText.text = string.Empty;
            }

            if (animatingCooldown)
            {
                if (cdImage.fillMethod == Image.FillMethod.Vertical ||
                    cdImage.fillMethod == Image.FillMethod.Horizontal)
                    cdImage.fillAmount = animFracRemaining;
                else
                    cdImage.fillAmount =
                        animFracRemaining * 0.6f +
                        0.2f; // because of the shape of the images (at time of writing), the first and last 20% or so do nothing, so we scale it to be between 0.2 and 0.8
                cdSelected.fillAmount = cdImage.fillAmount;
                if (cdText != null)
                    cdText.text = FormatCooldown(animFracRemaining * currentAnim.length);
            }

            // Gear Toss
            if (currentAnimHash != Animator.StringToHash("GearToss"))
                animFracRemaining = 0f;
            const float tossAnimLength = 0.75f;
            var gearTossState = player.states[Player.State.GearToss] as PlayerGearTossState;
            var totalCD = player.Data.GearShootCoolDown + tossAnimLength;
            var gearTossCDLeft = animFracRemaining == 0f
                ? gearTossState.LastUsedTime + player.Data.GearShootCoolDown - Time.time
                : player.Data.GearShootCoolDown + animFracRemaining * tossAnimLength;
            if (_rightCD.fillMethod == Image.FillMethod.Vertical || _rightCD.fillMethod == Image.FillMethod.Horizontal)
                _rightCD.fillAmount = gearTossCDLeft / totalCD;
            else
                _rightCD.fillAmount = gearTossCDLeft / totalCD * 0.6f + 0.2f; // see above
            _rightSelectedCD.fillAmount = _rightCD.fillAmount;
            if (_gearTossCDText != null)
                _gearTossCDText.text = FormatCooldown(gearTossCDLeft);

            // Switched Selected Images
            SelectAbility(player.CurrentAbility == Player.State.HeatWave, _topSelected, _topSelectedCD);
            SelectAbility(player.CurrentAbility == Player.State.Slash, _botSelected, _botSelectedCD);
            SelectAbility(player.CurrentAbility == Player.State.FireBlaze, _leftSelected, _leftSelectedCD);
            SelectAbility(player.CurrentAbility == Player.State.GearToss, _rightSelected, _rightSelectedCD);
        }

        private void SelectAbility(bool action, Image image1, Image image2)
        {
            if (action)
            {
                SetSelectedInactive();
                image1.gameObject.SetActive(true);
                image2.gameObject.SetActive(true);
            }
        }

        private void SetSelectedInactive()
        {
            _topSelected.gameObject.SetActive(false);
            _botSelected.gameObject.SetActive(false);
            _leftSelected.gameObject.SetActive(false);
            _rightSelected.gameObject.SetActive(false);
            _topSelectedCD.gameObject.SetActive(false);
            _botSelectedCD.gameObject.SetActive(false);
            _leftSelectedCD.gameObject.SetActive(false);
            _rightSelectedCD.gameObject.SetActive(false);
        }

        private string FormatCooldown(float cooldown)
        {
            if (cooldown <= 0)
                return "";
            if (cooldown < 0.9f)
                // display one decimal after the zero
                return "0." + Mathf.CeilToInt(cooldown * 10f);

            return Mathf.CeilToInt(cooldown).ToString();
        }
    }
}
