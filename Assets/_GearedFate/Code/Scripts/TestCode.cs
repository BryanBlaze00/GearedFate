using UnityEngine;
using UnityEngine.AI;

namespace BTG
{
    public class TestCode : MonoBehaviour, IDamagable
    {
        [SerializeField] private Transform target;
        private NavMeshAgent agent;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.updateRotation = false;
            agent.updateUpAxis = false;
        }

        private void Update()
        {
            agent.SetDestination(target.position);
        }

        private void TestPhysicsDetections()
        {
            var col = Physics2D.OverlapCircle(transform.position, 1, LayerMask.GetMask("Ground"));
            if (col != null)
            {
                //Debug.Log(col.name);
            }

            var hit = Physics2D.Raycast(transform.position, Vector2.down, 2, LayerMask.GetMask("Ground"));
            Debug.DrawRay(transform.position, Vector2.down * 2);
            Debug.Log(hit.collider);
            if (hit) Debug.Log(hit.collider.name);
        }

        private void OnDrawGizmos()
        {
            //Handles.DrawWireArc(transform.position, transform.forward, transform.position + Vector3.right * radius, 360, radius);
        }

        public void TakeDamage(float damage)
        {
            Debug.Log("Ouch!! it hurt about " + damage + "from: " + name);
        }
    }
}
