using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace BTG
{
    /// <summary>
    /// Script for the Player, used to attract scraps around, upon reaching, scraps apply their effects.
    /// </summary>
    public class ScrapMagnet : MonoBehaviour
    {
        // Reference to the player's transform
        [SerializeField] private Player _player;

        [SerializeField] private float reachTime = 1.0f;

        // Time it takes for the scrap to reach the player
        [SerializeField] private float _proximityThreshold = 0.5f;

        private IEnumerator MoveToPlayer(Scrap scrap)
        {
            float t = 0;

            // Move the object toward the player until it's close enough
            while (Vector3.Distance(scrap.transform.position, this._player.transform.position) > this._proximityThreshold)
            {
                t += Time.deltaTime / this.reachTime;
                scrap.transform.position = Vector3.Lerp(scrap.transform.position,
                    this._player.transform.position,
                    Mathf.SmoothStep(0.0f, 1.0f, t));
                yield return null; // Wait for the next frame
            }

            // Handle arrival logic
            scrap.ApplyEffect(this._player);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Trigger the magnet effect if the scrap enters the Players trigger zone
            if (other.TryGetComponent(out Scrap scrap)) this.StartCoroutine(this.MoveToPlayer(scrap));
        }
    }
}
