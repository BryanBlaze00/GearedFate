//
// Copyright (c) BTG. All rights reserved.
//

using UnityEngine;

namespace BTG
{
    /// <summary>
    /// Door
    /// </summary>
    public class Door : MonoBehaviour
    {
        [SerializeField] private GameObject closedChild;
        [SerializeField] private GameObject openChild;

        private void Start()
        {
            this.closedChild.SetActive(true);
            this.openChild.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out Player _))
            {
                this.closedChild.SetActive(false);
                this.openChild.SetActive(true);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.TryGetComponent(out Player _))
            {
                this.closedChild.SetActive(true);
                this.openChild.SetActive(false);
            }
        }
    }
}
