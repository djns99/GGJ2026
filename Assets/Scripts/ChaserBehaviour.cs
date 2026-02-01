
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class ChaserBehaviour : MonoBehaviour
{

    public float chaserStartSpeed = 5f;
    public float chaserDelayTime = 5f;
    public float totalGameTime = 120f;
    private float elapsedTime = 0f;
    public GameObject player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        Debug.Log("Elapsed Time: " + elapsedTime);
        if (elapsedTime >= chaserDelayTime)
        {
            Debug.Log("Chaser Moving");

            // TODO Get a more intelligent percentage
            var playerController = player.GetComponent<PlayerController>();
            float speed = Mathf.Lerp(chaserStartSpeed, playerController.CurrentMaxMoveSpeed, player.gameObject.transform.position.x / playerController.targetDistance);

            transform.position -= Vector3.left * speed * Time.deltaTime;
        }
    }
}
