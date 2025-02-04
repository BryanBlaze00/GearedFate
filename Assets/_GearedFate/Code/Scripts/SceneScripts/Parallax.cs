using UnityEngine;

/**
 * This class is responsible for parallaxing the background.
 */
public class Parallax : MonoBehaviour
{
    [SerializeField] private float parallaxOffset = -0.30f;
    private Camera cam;
    private Vector2 startPosition;
    private Vector2 travel => (Vector2)this.cam.transform.position - this.startPosition;

    private void Awake()
    {
        this.cam = Camera.main;
    }

    private void Start()
    {
        this.startPosition = this.transform.position;
    }

    private void FixedUpdate()
    {
        this.transform.position = this.startPosition + this.travel * this.parallaxOffset;
    }
}
