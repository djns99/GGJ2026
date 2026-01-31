
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChaserBehaviour : MonoBehaviour
{

    public float chaserSpeed = 5f;
    private bool isColliding = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!isColliding)
        {
            transform.position -= Vector3.left * chaserSpeed * Time.deltaTime;
        }
    }
    private void OnCollisionEnter2D(UnityEngine.Collision2D collision)
    {
        Debug.Log("COLLISION!");
        if (collision.gameObject.CompareTag("Player"))
        {
            isColliding = true;
            //Camera.main.GetComponent<ChaserBehaviour>().enabled = false;
            ReloadGame();
        }
    }

    private void ReloadGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
