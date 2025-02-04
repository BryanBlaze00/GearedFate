//
// Copyright (c) BTG. All rights reserved.
//

using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace BTG
{
    /// <summary>
    /// GAMEOVER 
    /// </summary>
    public class GameOver : MonoBehaviour
    {
        [Header("Game Over Settings")] [Header("Button Settings")] [SerializeField]
        private Button _retryButton;

        [SerializeField] private Button _menuButton;

        [Header("Avatar Settings")] [SerializeField]
        private GameObject _avatar;


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
