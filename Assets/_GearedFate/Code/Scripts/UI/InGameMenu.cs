using System;
using PrimeTween;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BTG
{
    public class InGameMenu : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _GearOne;

        [SerializeField]
        private RectTransform _GearTwo;

        [SerializeField]
        private RectTransform _mainMenuPanel;

        [SerializeField]
        private RectTransform _mainMenuPipe;

        [SerializeField]
        private RectTransform _settingPipe;

        [SerializeField]
        private InputAction _toggleMenuAction;

        [SerializeField]
        private GameObject _panelPause;

        private bool _menuOpened;

        protected void Start()
        {
            _toggleMenuAction.Enable();
            _toggleMenuAction.performed += ToggleMenu;
        }

        private void ToggleMenu(InputAction.CallbackContext obj)
        {
            Time.timeScale = 0f;
            _menuOpened = !_menuOpened;

            if (_menuOpened)
            {
                _panelPause.SetActive(true);

                Vector3 tempPos = _mainMenuPanel.position;
                _mainMenuPanel.anchorMax = new Vector2(1, 1);
                _mainMenuPanel.anchorMin = new Vector2(0, 0);
                _mainMenuPanel.position = tempPos;

                int windowHeight = Screen.height;

                 //TODO maybe change anchor before
                Sequence.Create(1, CycleMode.Restart, 0f, true)
                    .Group(Tween.UIAnchoredPositionY(_mainMenuPanel, windowHeight/2, 0.3f))
                    .Chain(Tween.UIAnchoredPositionY(_mainMenuPanel, windowHeight/2 +5, 0.2f))
                    .Chain(Tween.UIAnchoredPositionY(_mainMenuPanel, windowHeight/2, 0.2f))
                    .Chain(Tween.UIAnchoredPositionY(_mainMenuPanel, windowHeight/4, 0.2f))
                    .Chain(Tween.UIAnchoredPositionY(_mainMenuPanel, windowHeight/4+5, 0.2f))
                    .Chain(Tween.UIAnchoredPositionY(_mainMenuPanel, windowHeight/4, 0.2f))
                    .Chain(Tween.UIAnchoredPositionY(_mainMenuPanel, 0, 0.3f));

                Sequence.Create(1, CycleMode.Restart, 0f, true)
                    .Group(Tween.UIAnchoredPosition(_GearTwo, new Vector2(60, 60), 1f))
                    .Group(Tween.Rotation(_GearTwo.transform, _GearTwo.rotation.eulerAngles + new Vector3(0, 0, 120f), 0.3f));

                Sequence.Create(1, CycleMode.Restart, 0f, true)
                    .Group(Tween.UIAnchoredPosition(_GearOne, new Vector2(0, 65), 1f))
                    .Group(Tween.Rotation(_GearOne.transform, _GearOne.rotation.eulerAngles + new Vector3(0, 0, 120f), 0.3f));

                Tween.UIAnchoredPositionX(_mainMenuPipe, 500, 1.5f, Ease.Default, 1, CycleMode.Restart, 0f, 0f, true);

                Tween.UIAnchoredPositionX(_settingPipe, 500, 1.5f, Ease.Default, 1, CycleMode.Restart, 0f, 0f, true);

            }
            else
            {
                Time.timeScale = 1f;
                _panelPause.SetActive(false);

                Vector3 tempPos = _mainMenuPanel.position;
                _mainMenuPanel.anchorMax = new Vector2(1, 2);
                _mainMenuPanel.anchorMin = new Vector2(0, 1);
                _mainMenuPanel.position = tempPos;

                Tween.UIAnchoredPositionY(_mainMenuPanel, 0, 1f);
                Sequence.Create(1, CycleMode.Restart, 0f, true)
                    .Group(Tween.UIAnchoredPosition(_GearTwo, new Vector2(-200, -200), 1f))
                    .Group(Tween.Rotation(_GearTwo.transform, Quaternion.Euler(0,0,0), 1f, Ease.Default, 1, CycleMode.Incremental));

                Sequence.Create(1, CycleMode.Restart, 0f, true)
                    .Group(Tween.UIAnchoredPosition(_GearOne, new Vector2(200, 45), 1f))
                    .Group(Tween.Rotation(_GearOne.transform, Quaternion.Euler(0,0,0), 1f, Ease.Default, 1, CycleMode.Incremental));

                Tween.UIAnchoredPositionX(_mainMenuPipe, -100, 1f);

                Tween.UIAnchoredPositionX(_settingPipe, -100, 1f);
            }
        }

    }
}
