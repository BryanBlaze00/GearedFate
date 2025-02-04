//
// Copyright (c) BTG. All rights reserved.
//

using UnityEngine;
using UnityUtils;

namespace BTG
{
    /// <summary>
    /// Elevator class to manage elevator movement and interactions.
    /// </summary>
    public class Elevator : PersistentSingleton<Elevator>
    {
        [SerializeField] private Transform _parentTransform;
        [SerializeField] private GameObject _exitLevelTrigger;

        private Animator _animator;

        protected override void Awake()
        {
            base.Awake();

            _animator = GetComponent<Animator>();
            _animator.SetBool("isFlying", false);

            if (_exitLevelTrigger != null && _exitLevelTrigger.activeSelf) _exitLevelTrigger.SetActive(false);
        }

        private void Start()
        {
            transform.SetParent(_parentTransform);

            CutSceneCheckActivate();
        }

        public void ActivateElevator()
        {
            _animator.SetBool("isFlying", true);
            _exitLevelTrigger.SetActive(true);
        }

        public void DeactivateElevator()
        {
            _animator.SetBool("isFlying", false);
            _exitLevelTrigger.SetActive(false);
        }

        public void ActivateElevatorAnim()
        {
            _animator.SetBool("isFlying", true);
        }

        public void DeactivateElevatorAnim()
        {
            _animator.SetBool("isFlying", false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Player _) && _animator.GetBool("isFlying"))
                GameManager.Instance.LoadNextLevel();
        }

        private void CutSceneCheckActivate()
        {
            // Check if the current scene is a cutscene scene
            var cutsceneScenes = GameManager.Instance.GetCutsceneScenes();
            foreach (var scene in cutsceneScenes)
                if (scene == GameManager.Instance.GetCurrentScene())
                    ActivateElevatorAnim();
        }
    }
}
