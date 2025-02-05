namespace BTG
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.Serialization;
    using UnityEngine.UI;

    public class PlayerUI : MonoBehaviour
    {
        private Player _player;

        [FormerlySerializedAs("healthBar")]
        [SerializeField]
        private Slider _healthBar;

        [FormerlySerializedAs("fuelBar")]
        [SerializeField]
        private Slider _fuelBar;

        [FormerlySerializedAs("gearTossIcon")]
        [SerializeField]
        private Slider _gearTossIcon;

        [FormerlySerializedAs("gearTossCDText")]
        [SerializeField]
        private TextMeshProUGUI _gearTossCdText;

        private void Awake()
        {
            if (_player == null)
            {
                Debug.LogWarning($"{nameof(_player)} wasn't assigned on {nameof(PlayerUI)}");
                _player = FindAnyObjectByType<Player>();
            }
        }

        private void Update()
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            _healthBar.value = _player.CurrentHealth / _player.Data.Health;
            _fuelBar.value = _player.CurrentAttackFuelAmount / _player.Data.MaxAttackFuelAmount;
            var gearTossState = _player[Player.State.GearToss] as PlayerGearTossState;
            var gearTossCdLeft = gearTossState.LastUsedTime + _player.Data.GearShootCoolDown - Time.time;
            _gearTossIcon.value = gearTossCdLeft / _player.Data.GearShootCoolDown;
            _gearTossCdText.text = FormatCooldown(gearTossCdLeft);
        }

        private string FormatCooldown(float cooldown)
        {
            if (cooldown <= 0)
            {
                return string.Empty;
            }

            // display one decimal after the zero
            if (cooldown < 0.9f)
            {
                return "0." + Mathf.CeilToInt(cooldown * 10f);
            }

            return Mathf.CeilToInt(cooldown).ToString();
        }
    }
}
