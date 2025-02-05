// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using UnityEngine;

    /// <summary>
    /// ExitLevel is a script that will trigger to load the next level. Or the credits
    /// </summary>
    public class ExitLevel : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("MovableCollider") &&
                GameManager.Instance.GetCurrentScene() == "BossLevel 5")
            {
                GameManager.Instance.LoadCredits();
            }
            else if (other.gameObject.CompareTag("MovableCollider"))
            {
                GameManager.Instance.LoadNextLevel();
            }
        }
    }
}
