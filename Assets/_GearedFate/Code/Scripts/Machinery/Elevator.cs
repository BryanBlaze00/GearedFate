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

            this._animator = this.GetComponent<Animator>();
            this._animator.SetBool("isFlying", false);

            if (this._exitLevelTrigger != null && this._exitLevelTrigger.activeSelf)
            {
                this._exitLevelTrigger.SetActive(false);
            }
        }

        private void Start()
        {
            this.transform.SetParent(this._parentTransform);

            this.CutSceneCheckActivate();
        }

        public void ActivateElevator()
        {
            this._animator.SetBool("isFlying", true);
            this._exitLevelTrigger.SetActive(true);
        }

        public void DeactivateElevator()
        {
            this._animator.SetBool("isFlying", false);
            this._exitLevelTrigger.SetActive(false);
        }

        public void ActivateElevatorAnim()
        {
            this._animator.SetBool("isFlying", true);
        }

        public void DeactivateElevatorAnim()
        {
            this._animator.SetBool("isFlying", false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Player _) && this._animator.GetBool("isFlying"))
            {
                GameManager.Instance.LoadNextLevel();
            }
        }

        private void CutSceneCheckActivate()
        {
            // Check if the current scene is a cutscene scene
            var cutsceneScenes = GameManager.Instance.GetCutsceneScenes();
            foreach (var scene in cutsceneScenes)
            {
                if (scene == GameManager.Instance.GetCurrentScene())
                {
                    this.ActivateElevatorAnim();
                }
            }
        }
    }
}
