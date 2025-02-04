//
// Copyright (c) BTG. All rights reserved.
//

using System.Collections;
using UnityEngine;

namespace BTG
{
    /// <summary>
    /// GSDyingState
    /// </summary>
    public class GSDyingState : GSBaseState
    {
        public GSDyingState(FiniteStateMachine<GearboundSentinel.State> fsm, GearboundSentinel.State state,
            GearboundSentinel gs) : base(fsm, state, gs)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter(); // TODO: What happens when he dies?
            gearboundSentinel.StartCoroutine(DieAndStayDead());
            gearboundSentinel.StopAllCoroutines();
            gearboundSentinel.GetComponentsInChildren<Collider2D>().ForEach(x => x.enabled = false);
            Object.Destroy(gearboundSentinel);
            GameManager.Instance
                .StartCoroutine(
                    DelayedElevator()); // start it on GameManager because gearboundSentinel's coroutines will end right after destroy
        }

        private IEnumerator DelayedElevator()
        {
            yield return new WaitForSeconds(2.5f); // wait for animation to finish
            Elevator.Instance.ActivateElevator();
            Debug.Log("GS Activated elevator");
        }


        private IEnumerator DieAndStayDead()
        {
            gearboundSentinel.Animator.speed = 1f;
            gearboundSentinel.transform.position += new Vector3(0f, -1.05f); // death animation is higher
            gearboundSentinel.Shadow.SetActive(false);
            var pos = gearboundSentinel.transform.position;
            gearboundSentinel.Animator.Play("Death");
            yield return null; // this might break if something sets its animation differently right after death?
            Debug.Log("This code shouldn't run because all coroutines should be stopped.");
            var animationLength = gearboundSentinel.Animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
            var played = 0f;
            while (true) // prevents other stuff from changing the animation
            {
                played += Time.deltaTime;
                var progress = played / animationLength;
                gearboundSentinel.Animator.Play("Death", -1, Mathf.Min(1f, progress));
                gearboundSentinel.transform.position = pos;
                yield return null;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void OnFrameUpdate()
        {
            base.OnFrameUpdate();
        }

        public override void OnPhysicsUpdate()
        {
            base.OnPhysicsUpdate();
        }
    }
}
