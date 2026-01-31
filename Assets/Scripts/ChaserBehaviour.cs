
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class ChaserBehaviour : MonoBehaviour
{

    public float chaserSpeed = 5f;
    public float chaserDelayTime = 5f;
    private bool isColliding = false;
    private float elapsedTime = 0f;
    GameObject player = GameObject.FindWithTag("Player");
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        Debug.Log("Elapsed Time: " + elapsedTime);
        if (!isColliding && elapsedTime >= chaserDelayTime)
        {
            Debug.Log("Chaser Moving");
            transform.position -= Vector3.left * chaserSpeed * Time.deltaTime;
        }
    }
    private void OnCollisionEnter2D(UnityEngine.Collision2D collision)
    {
        Debug.Log("COLLISION!");
        if (collision.gameObject.CompareTag("Player"))
        {
            isColliding = true;
            ReloadGame();
        }
    }

    private void ReloadGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
