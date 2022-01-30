using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float speed = 5f;
    private Vector2 motion;

    public float zoomSensitivity = 20f;

    private Vector3 startPosition;
    private float startZoom;

    private void Start()
    {
        startPosition = transform.position;

        startZoom = this.GetComponent<Camera>().orthographicSize;
    }


    // Update is called once per frame
    void Update()
    {
        motion = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        float zoom = Input.GetAxis("Mouse ScrollWheel") * zoomSensitivity;

        transform.Translate(motion * speed * Time.deltaTime);

        Camera cam = this.GetComponent<Camera>();

        if(cam.orthographicSize - zoom <= 2 ||
           cam.orthographicSize - zoom > 20)
        {
            zoom = 0f;
        }

        cam.orthographicSize -= zoom;

        if(Input.GetKeyDown(KeyCode.Space))
        {
            // Reset position.
            transform.position = startPosition;
            cam.orthographicSize = startZoom;
        }
    }
}
