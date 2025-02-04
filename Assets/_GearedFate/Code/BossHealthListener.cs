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
        void Start()
        {
            _boss = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<IBoss>().FirstOrDefault();
#if UNITY_EDITOR
         if (_boss == null)
                Debug.LogError($"No {nameof(IBoss)} object found in scene");
#endif
      }
        void Update()
        {
            if (_boss == null)
                return;
            float normalizedLife = _boss.CurrentHealth / _boss.MaxHealth;
            if (normalizedLife > 0.5f)
            {
                _firstBossBar.fillAmount = 1f;
                _secondBossBar.fillAmount = 2 * normalizedLife - 1;
            }
            else
            {
                _firstBossBar.fillAmount = 2 * normalizedLife;
                _secondBossBar.fillAmount = 0f;
            }
        }
    }
}
