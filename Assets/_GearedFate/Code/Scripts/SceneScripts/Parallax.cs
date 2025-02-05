using UnityEngine;
using UnityEngine.Serialization;

/**
 * This class is responsible for parallaxing the background.
 */
public class Parallax : MonoBehaviour
{
    [FormerlySerializedAs("parallaxOffset")]
    [SerializeField]
    private float _parallaxOffset = -0.30f;

    private Camera _cam;

    private Vector2 _startPosition;

    private Vector2 Travel => (Vector2)_cam.transform.position - _startPosition;

    private void Awake()
    {
        _cam = Camera.main;
    }

    private void Start()
    {
        _startPosition = transform.position;
    }

    private void FixedUpdate()
    {
        transform.position = _startPosition + (Travel * _parallaxOffset);
    }
}
