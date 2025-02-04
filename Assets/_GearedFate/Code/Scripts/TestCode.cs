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
            this.agent = this.GetComponent<NavMeshAgent>();
            this.agent.updateRotation = false;
            this.agent.updateUpAxis = false;
        }

        private void Update()
        {
            this.agent.SetDestination(this.target.position);
        }

        private void TestPhysicsDetections()
        {
            var col = Physics2D.OverlapCircle(this.transform.position, 1, LayerMask.GetMask("Ground"));
            if (col != null)
            {
                //Debug.Log(col.name);
            }

            var hit = Physics2D.Raycast(this.transform.position, Vector2.down, 2, LayerMask.GetMask("Ground"));
            Debug.DrawRay(this.transform.position, Vector2.down * 2);
            Debug.Log(hit.collider);
            if (hit)
            {
                Debug.Log(hit.collider.name);
            }
        }

        private void OnDrawGizmos()
        {
            //Handles.DrawWireArc(transform.position, transform.forward, transform.position + Vector3.right * radius, 360, radius);
        }

        public void TakeDamage(float damage)
        {
            Debug.Log("Ouch!! it hurt about " + damage + "from: " + this.name);
        }
    }
}
