using UnityEngine;

public class QuadParallax : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [Range(0f, 1f)] public float parallaxFactor = 0.5f;
    public float yOffset = 0f;

    private MeshRenderer meshRenderer;
    private Material mat;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        mat = meshRenderer.material;

        // Scale the Quad to fit the camera view perfectly
        float camHeight = 2f * cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;
        transform.localScale = new Vector3(camWidth, camHeight, 1f);
    }

    void LateUpdate()
    {
        if (cam == null) return;

        // 1. Keep the Quad centered on the camera
        float bottomPosition = cam.transform.position.y;
        transform.position = new Vector3(
            cam.transform.position.x,
            bottomPosition + yOffset,
            transform.position.z
        );

        // 2. Scroll the texture based on Camera X position
        // We divide by localScale.x to keep the scroll speed consistent with world movement
        float offset = (cam.transform.position.x * parallaxFactor) / transform.localScale.x;
        mat.mainTextureOffset = new Vector2(offset, 0);
    }
}