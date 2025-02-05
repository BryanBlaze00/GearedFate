namespace BTG
{
    using UnityEngine;
    using UnityEngine.AI;

    public class GCRunAwayState : GCBaseState
    {
        public GCRunAwayState(FiniteStateMachine<GreatCreator.GreatCreatorState> fsm, GreatCreator enemy, int animId)
            : base(fsm, enemy, animId)
        {
        }

        public override void OnEnter()
        {
            PlayAnimation();
        }

        public override void OnExit()
        {
        }

        public override void OnFrameUpdate()
        {
            GreatCreator.SetAnimationMoveParameters(GreatCreator.Agent.velocity);
            if (GreatCreator.DistanceToTarget > GreatCreator.SafeDistance)
            {
                Fsm.SwitchState(GreatCreator[GreatCreator.GreatCreatorState.Idle]);
            }
            else
            {
                if (IsReadyToSpawn() && GreatCreator.Stage == 0)
                {
                    Fsm.SwitchState(GreatCreator[GreatCreator.GreatCreatorState.Swarm]);
                    return;
                }

                if (GoingToCenter)
                {
                    return;
                }

                var canMoveAway = CanMoveAwayFromPlayer(out var awayPosition);

                if (canMoveAway)
                {
                    GreatCreator.Agent.SetDestination(awayPosition);
                }
                else if (!canMoveAway && GreatCreator.Stage == 0)
                {
                    Fsm.SwitchState(GreatCreator[GreatCreator.GreatCreatorState.Dash]);
                }
                else if (!canMoveAway && GreatCreator.Stage > 0)
                {
                    GreatCreator.StartCoroutine(GoToCenter());
                }
            }
        }

        public override void OnPhysicsUpdate()
        {
        }

        public void SetAnimation(int animId)
        {
            SetAnimationId(animId);
        }

        private bool CanMoveAwayFromPlayer(out Vector2 targetPosition)
        {
            // Calculate the target position
            targetPosition = (Vector2)GreatCreator.Target.position + (GreatCreator.DirectionToTarget * GreatCreator.SafeDistance);

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
            var side = Vector2.Perpendicular(GreatCreator.DirectionToTarget);
            Vector3 targetPosition = (Vector2)GreatCreator.Target.position - (GreatCreator.DirectionToTarget * GreatCreator.SafeDistance) + side;

            // Find a valid NavMesh position close to the target
            if (NavMesh.SamplePosition(targetPosition, out var hit, 3, NavMesh.AllAreas))
            {
                GreatCreator.Agent.SetDestination(hit.position);
            }

            targetPosition = (Vector2)GreatCreator.Target.position - (GreatCreator.DirectionToTarget * GreatCreator.SafeDistance) - side;

            // Find a valid NavMesh position close to the target
            if (NavMesh.SamplePosition(targetPosition, out hit, 3, NavMesh.AllAreas))
            {
                GreatCreator.Agent.SetDestination(hit.position);
            }
        }
    }
}
