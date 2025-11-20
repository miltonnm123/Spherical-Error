using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float distance = 4f;
    public float minDistance = 0.5f;
    public float height = 2f;
    public float rotationSpeed = 100f;
    public LayerMask collisionLayers;

    private float yaw;
    private float pitch;    

    public Vector3 offset;

    public float smoothSpeed = 0.1f;

    void LateUpdate()
    {
        if (!target) return;

        yaw += Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        pitch -= Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

        pitch = Mathf.Clamp(pitch, -20f, 60f);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

        Vector3 desiredPosition = target.position + rotation * new Vector3(0, height, -distance);

        //Vector3 offset = rotation * new Vector3(0, height, -distance); DELETE IF NEW CAMERA WORKS

        // Collision Check
        RaycastHit hit;
        Vector3 direction = desiredPosition - target.position;

        if(Physics.SphereCast(target.position, 0.3f, direction.normalized, out hit, distance, collisionLayers))
        {
            float hitDistance = Mathf.Max(hit.distance - 0.3f, minDistance);
            desiredPosition = target.position + direction.normalized * hitDistance;
        }

        transform.position = desiredPosition;

        transform.LookAt(target.position + Vector3.up * height * 0.5f);


        /*
        Vector3 desiredPosition = player.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
        */
    }
}
