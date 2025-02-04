using UnityEngine;
using UnityEngine.AI;

namespace BTG
{
    public class GCRunAwayState : GCBaseState
    {
        public GCRunAwayState(FiniteStateMachine<GreatCreator.GreatCreatorState> fsm, GreatCreator enemy,
            int animId) : base(fsm,
            enemy, animId)
        {
        }

        public override void OnEnter()
        {
            this.PlayAnimation();
        }

        public override void OnExit()
        {
        }

        public override void OnFrameUpdate()
        {
            this.GreatCreator.SetAnimationMoveParameters(this.GreatCreator.Agent.velocity);
            if (this.GreatCreator.DistanceToTarget > this.GreatCreator.SafeDistance)
            {
                this.fsm.SwitchState(this.GreatCreator.States[GreatCreator.GreatCreatorState.Idle]);
            }
            else
            {
                if (this.IsReadyToSpawn() && this.GreatCreator.Stage == 0)
                {
                    this.fsm.SwitchState(this.GreatCreator.States[GreatCreator.GreatCreatorState.Swarm]);
                    return;
                }

                if (this.GoingToCenter)
                {
                    return;
                }

                var canMoveAway = this.CanMoveAwayFromPlayer(out var AwayPosition);

                if (canMoveAway)
                {
                    this.GreatCreator.Agent.SetDestination(AwayPosition);
                }
                else if (!canMoveAway && this.GreatCreator.Stage == 0)
                {
                    this.fsm.SwitchState(this.GreatCreator.States[GreatCreator.GreatCreatorState.Dash]);
                }
                else if (!canMoveAway && this.GreatCreator.Stage > 0)
                {
                    this.GreatCreator.StartCoroutine(this.GoToCenter());
                }
            }
        }


        public override void OnPhysicsUpdate()
        {
        }

        public void SetAnimation(int animId)
        {
            this.SetAnimationId(animId);
        }

        private bool CanMoveAwayFromPlayer(out Vector2 targetPosition)
        {
            // Calculate the target position
            targetPosition = (Vector2)this.GreatCreator.Target.position + this.GreatCreator.DirectionToTarget * this.GreatCreator.SafeDistance;

            // Find a valid NavMesh position close to the target
            if (NavMesh.SamplePosition(targetPosition, out var hit, 3, NavMesh.AllAreas))
            {
                return true;
            }
            else
            {
                Debug.LogWarning("Failed to find a valid NavMesh position away from the point.");
                return false;
            }
        }

        private void MoveOnSidePlayer()
        {
            var side = Vector2.Perpendicular(this.GreatCreator.DirectionToTarget);
            Vector3 targetPosition = (Vector2)this.GreatCreator.Target.position - this.GreatCreator.DirectionToTarget * this.GreatCreator.SafeDistance + side;

            // Find a valid NavMesh position close to the target
            if (NavMesh.SamplePosition(targetPosition, out var hit, 3, NavMesh.AllAreas))
            {
                this.GreatCreator.Agent.SetDestination(hit.position);
            }

            targetPosition = (Vector2)this.GreatCreator.Target.position - this.GreatCreator.DirectionToTarget * this.GreatCreator.SafeDistance - side;

            // Find a valid NavMesh position close to the target
            if (NavMesh.SamplePosition(targetPosition, out hit, 3, NavMesh.AllAreas))
            {
                this.GreatCreator.Agent.SetDestination(hit.position);
            }
        }
    }
}
