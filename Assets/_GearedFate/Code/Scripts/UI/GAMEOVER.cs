// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// GAMEOVER
    /// </summary>
    public class GameOver : MonoBehaviour
    {
        [Header("Game Over Settings")]
        [Header("Button Settings")]
        [SerializeField]
        private Button _menuButton;

        [SerializeField]
        private Button _retryButton;

        [Header("Avatar Settings")]
        [SerializeField]
        private GameObject _avatar;

        [Header("Audio Clip")]
        [SerializeField]
        private AudioClip _gameoverSFX;

        private UISpriteAnimation _avatarAnim;

        private void Awake()
        {
            _avatarAnim = _avatar.GetComponent<UISpriteAnimation>();
            _retryButton.onClick.AddListener(Retry);
            _menuButton.onClick.AddListener(GoMain);
        }

        private void Start()
        {
            _avatarAnim.PlayOnce(false);
            AudioManager.Instance.StopMusic();
            AudioManager.Instance.PlaySFX(_gameoverSFX);
        }

        private void GoMain()
        {
            GameManager.Instance.LoadMainMenu();
        }

        private void Retry()
        {
            GameManager.Instance.RestartLevel();
        }
    }
}
