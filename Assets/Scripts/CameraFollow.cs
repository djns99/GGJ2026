using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform followTransform;
    public BoxCollider2D mapBounds;

    private float xMin, xMax, yMin, yMax;
    private float camY, camX;
    private float camOrthsize;
    private float cameraRatio;
    private Camera mainCamera;
    private Vector3 smoothPos;

    public float smoothSpeed = 0.1f;
    private Vector3 velocity = Vector3.zero;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //xMin = mapBounds.bounds.min.x;
        //xMin = mapBounds.bounds.max.x;
        //yMin = mapBounds.bounds.min.y;
        //yMax = mapBounds.bounds.max.y;

        //mainCamera = GetComponent<Camera>();
        //camOrthsize = mainCamera.orthographicSize;
        //cameraRatio = (xMax / camOrthsize) / 2.0f;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPos = new Vector3(followTransform.position.x, followTransform.position.y, this.transform.position.z);
        //this.transform.position = Vector3.Lerp(this.transform.position, targetPos, smoothSpeed);
        this.transform.position = Vector3.SmoothDamp(this.transform.position, targetPos, ref velocity, smoothSpeed);
        //camY = Mathf.Clamp(followTransform.position.y, yMin + camOrthsize, yMax - camOrthsize);
        //camX = Mathf.Clamp(followTransform.position.x, xMin + cameraRatio, xMax - cameraRatio);
        //smoothPos = Vector3.Lerp(this.transform.position, new Vector3(camX, camY, this.transform.position.z), smoothSpeed);
        //this.transform.position = smoothPos;
    }
}
