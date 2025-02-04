using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace BTG
{
    public class BossHealthListener : MonoBehaviour
    {

        [SerializeField]
        private Image _firstBossBar;

        [SerializeField]
        private Image _secondBossBar;

        private IBoss _boss;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            this._boss = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<IBoss>().FirstOrDefault();
#if UNITY_EDITOR
         if (this._boss == null)
                Debug.LogError($"No {nameof(IBoss)} object found in scene");
#endif
      }

        private void Update()
        {
            if (this._boss == null)
                return;
            float normalizedLife = this._boss.CurrentHealth / this._boss.MaxHealth;
            if (normalizedLife > 0.5f)
            {
                this._firstBossBar.fillAmount = 1f;
                this._secondBossBar.fillAmount = 2 * normalizedLife - 1;
            }
            else
            {
                this._firstBossBar.fillAmount = 2 * normalizedLife;
                this._secondBossBar.fillAmount = 0f;
            }
        }
    }
}
