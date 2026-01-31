using UnityEngine;

public class DropletScript : MonoBehaviour
{

    public float warningTime = 0.3f;

    private float lifetime = 0.0f;
    private bool dropped = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void Drop() {
        // TODO Play a drip sound
        GetComponent<Rigidbody2D>().simulated = true;
        dropped = true;
    }


    // Update is called once per frame
    void Update()
    {
        lifetime += Time.deltaTime;
        if (lifetime >= warningTime)
        {
            Drop();
        }

        if (!dropped)
        {
            var scale = gameObject.transform.localScale;
            scale.y = Mathf.Lerp(0, 1, lifetime / warningTime);
            gameObject.transform.localScale = scale;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // TODO Play a drip sound
        Destroy(gameObject);
    }
}
