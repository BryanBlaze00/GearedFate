// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using UnityEngine;
    using UnityEngine.Events;

    /// <summary>
    /// Classic Door.
    /// </summary>
    public class Door : MonoBehaviour
    {
        [SerializeField]
        private bool _isLocked = false;

        [SerializeField]
        private TriggerDetector _enterTrigger;

        [SerializeField]
        private TriggerDetector _exitTrigger;

        [SerializeField]
        private GameObject _closedChild;

        [SerializeField]
        private GameObject _openChild;

        private bool _isOpen = false;

        private UnityEvent onTriggerEnter2D;

        /// <summary>
        /// Lock the door.
        /// </summary>
        public void DoorLock()
        {
            _isLocked = true;
            if (_isOpen)
            {
                CloseDoor();
            }
        }

        /// <summary>
        /// Unlock the door.
        /// </summary>
        public void DoorUnlock()
        {
            _isLocked = false;
            if (!_isOpen)
            {
                OpenDoor();
            }
        }

        private void OnEnable()
        {
            _enterTrigger.onTriggerEnter2D.AddListener(WhenTriggerEnter);
            _exitTrigger.onTriggerExit2D.AddListener(WhenTriggerExit);
        }

        private void Start()
        {
            _closedChild.SetActive(true);
            _openChild.SetActive(false);
        }

        private void OnDisable()
        {
            _enterTrigger.onTriggerEnter2D.RemoveAllListeners();
            _exitTrigger.onTriggerExit2D.RemoveAllListeners();
        }

        private void WhenTriggerEnter(Collider2D other)
        {
            if (other.TryGetComponent(out Player _) && !_isLocked)
            {
                OpenDoor();
            }
        }

        private void WhenTriggerExit(Collider2D other)
        {
            if (other.TryGetComponent(out Player _) && !_isLocked)
            {
                CloseDoor();
                _isLocked = true;
            }
        }

        private void OpenDoor()
        {
            _closedChild.SetActive(false);
            _openChild.SetActive(true);
            _isOpen = true;
        }

        private void CloseDoor()
        {
            _closedChild.SetActive(true);
            _openChild.SetActive(false);
            _isOpen = false;
        }
    }
}
