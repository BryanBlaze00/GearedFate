namespace BTG
{
    using UnityEngine;
    using UnityEngine.AI;
    using UnityEngine.Serialization;

    public class TestCode : MonoBehaviour, IDamagable
    {
        [FormerlySerializedAs("target")]
        [SerializeField]
        private Transform _target;

        private NavMeshAgent _agent;

        public void TakeDamage(float damage)
        {
            Debug.Log("Ouch!! it hurt about " + damage + "from: " + name);
        }

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.updateRotation = false;
            _agent.updateUpAxis = false;
        }

        private void Update()
        {
            _agent.SetDestination(_target.position);
        }

        private void TestPhysicsDetections()
        {
            var col = Physics2D.OverlapCircle(transform.position, 1, LayerMask.GetMask("Ground"));
            if (col != null)
            {
                // Debug.Log(col.name);
            }

            var hit = Physics2D.Raycast(transform.position, Vector2.down, 2, LayerMask.GetMask("Ground"));
            Debug.DrawRay(transform.position, Vector2.down * 2);
            Debug.Log(hit.collider);
            if (hit)
            {
                Debug.Log(hit.collider.name);
            }
        }
    }
}
