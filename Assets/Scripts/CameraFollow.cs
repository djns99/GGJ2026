using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform followTransform;

    public float smoothSpeed = 0.1f;
    private Vector3 velocity = Vector3.zero;


    // Update is called once per frame
    void LateUpdate()
    {
        if (followTransform != null)
        {
            Vector3 targetPos = new Vector3(followTransform.position.x, followTransform.position.y, transform.position.z);

            // SmoothDamp is usually smoother than Lerp for cameras
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothSpeed);
        }
    }
}
