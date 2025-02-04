using UnityEngine;

public class SpinMe : MonoBehaviour
{
    [SerializeField] private float speed = 1f;

    private void Update()
    {
        this.transform.Rotate(Vector3.forward, this.speed * Time.deltaTime);
    }
}
