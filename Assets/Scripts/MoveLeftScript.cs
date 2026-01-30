using UnityEngine;

public class MoveLeftScript : MonoBehaviour
{

    public Vector3 move;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += move * Time.deltaTime;
    }
}
