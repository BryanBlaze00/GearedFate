//
// Copyright (c) BTG. All rights reserved.
//

using UnityEngine;
using System.Collections.Generic;
using MEC;
using Unity.VisualScripting;
using System.Collections;

namespace BTG
{
   /// <summary>
   /// Projectile
   /// </summary>

   public class Projectile : MonoBehaviour
   {
		[field: SerializeField] public ProjectileData Data { get; private set; }

		private CoroutineHandle coroutineHandle;
		private Rigidbody2D rb;

        private int _unaffectedLayer = -1;

		/// <summary>
		/// Set a layer of game objects that won't be affected by those projectiles.
		/// Mostly useful so that throwers don't get damages from their own projectiles.
		/// </summary>
		public void SetUnaffectedLayer(int unaffectedLayer)
		{
			_unaffectedLayer = unaffectedLayer;
		}

		private void OnEnable()
		{
			coroutineHandle = Timing.RunCoroutine(_Disable().CancelWith(gameObject)); 
			if(rb == null)
				rb = GetComponent<Rigidbody2D>();
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			// Do nothing if the collided game object is the on the unnafected layer or if it can't take damages
			// TODO : Some objects should stop projectiles, like walls, maybe have a IStopProjectileInterface to deal with that.
			if (collision.gameObject.layer == _unaffectedLayer || !collision.TryGetComponent(out IDamagable damageable))
			{
				return;
			}
			Debug.Log("Projectile hit");
			damageable.TakeDamage(Data.Damage);
            gameObject.SetActive(false);
            Timing.KillCoroutines(coroutineHandle);
		}

		private void OnCollisionEnter2D(Collision2D collision)
		{
			OnTriggerEnter2D(collision.collider);
		}

        IEnumerator<float> _Disable()
		{
         yield return Timing.WaitForSeconds(5f);
			gameObject.SetActive(false);
		}
	}
}
