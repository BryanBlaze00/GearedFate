using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BTG
{
    public class PlayerUI : MonoBehaviour
    {
        public Player player;

        [SerializeField] private Slider healthBar;
        [SerializeField] private Slider fuelBar;
        [SerializeField] private Slider gearTossIcon;
        [SerializeField] private TextMeshProUGUI gearTossCDText;

        private void Awake()
        {
            if (this.player == null)
            {
                Debug.LogWarning($"{nameof(this.player)} wasn't assigned on {nameof(PlayerUI)}");
                this.player = FindAnyObjectByType<Player>();
            }
        }

        private void Update()
        {
            this.UpdateUI();
        }

        private void UpdateUI()
        {
            this.healthBar.value = this.player.CurrentHealth / this.player.Data.Health;
            this.fuelBar.value = this.player.CurrentAttackFuelAmount / this.player.Data.MaxAttackFuelAmount;
            var gearTossState = this.player.states[Player.State.GearToss] as PlayerGearTossState;
            var GearTossCDLeft = gearTossState.LastUsedTime + this.player.Data.GearShootCoolDown - Time.time;
            this.gearTossIcon.value = GearTossCDLeft / this.player.Data.GearShootCoolDown;
            this.gearTossCDText.text = this.FormatCooldown(GearTossCDLeft);
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
