using UnityEngine;

/**
 * This class is responsible for parallaxing the background.
 */
public class Parallax : MonoBehaviour
{
    [SerializeField] private float parallaxOffset = -0.30f;
    private Camera cam;
    private Vector2 startPosition;
    private Vector2 travel => (Vector2)cam.transform.position - startPosition;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Start() {
        startPosition = transform.position;
    }

    private void FixedUpdate()
    {
        transform.position = startPosition + travel * parallaxOffset;
    }
}