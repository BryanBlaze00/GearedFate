namespace BTG
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class PlayerUI : MonoBehaviour
    {
        public Player player;

        [SerializeField]
        private Slider healthBar;
        [SerializeField]
        private Slider fuelBar;
        [SerializeField]
        private Slider gearTossIcon;
        [SerializeField]
        private TextMeshProUGUI gearTossCDText;

        private void Awake()
        {
            if (player == null)
            {
                Debug.LogWarning($"{nameof(player)} wasn't assigned on {nameof(PlayerUI)}");
                player = FindAnyObjectByType<Player>();
            }
        }

        private void Update()
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            healthBar.value = player.CurrentHealth / player.Data.Health;
            fuelBar.value = player.CurrentAttackFuelAmount / player.Data.MaxAttackFuelAmount;
            var gearTossState = player.states[Player.State.GearToss] as PlayerGearTossState;
            var GearTossCDLeft = gearTossState.LastUsedTime + player.Data.GearShootCoolDown - Time.time;
            gearTossIcon.value = GearTossCDLeft / player.Data.GearShootCoolDown;
            gearTossCDText.text = FormatCooldown(GearTossCDLeft);
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
