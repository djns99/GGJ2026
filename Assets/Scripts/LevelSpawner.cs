using UnityEngine;
using System.Collections.Generic;


public class LevelSpawner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public List<GameObject> spawnables;
    public Transform chaserTransform;
    public Camera cameraObject;
    public int spawnBuffer = 1000;
    public int despawnBuffer = 1000;

    public Vector3 defaultSpawnPoint = new Vector3(0, 0, 0);

    private LinkedList<GameObject> spawnedObjects = new LinkedList<GameObject>();

    

    
    
    void Start()
    {
        Debug.Assert(spawnables.Count > 0);
    }


    GameObject pickObjectToSpawn(GameObject previous)
    {
        var item = Random.Range(0, spawnables.Count);
        return spawnables[item];
    }

    // Update is called once per frame
    void Update()
    {
        // Despawn objects that are consumed
        var despawnThreshold = chaserTransform.position.x - despawnBuffer;
        while (spawnedObjects.Count > 0 && spawnedObjects.First.Value.GetComponent<Transform>().position.x < despawnThreshold)
        {
            var top = spawnedObjects.First.Value;
            spawnedObjects.RemoveFirst();
            Destroy(top);
        }

        // Spawn objects if needed
        var spawnThreshold = Camera.main.transform.position.x + Camera.main.orthographicSize * Screen.width / Screen.height / 2 + spawnBuffer;

        // Spawn only one object per frame
        if (spawnedObjects.Count == 0 || spawnedObjects.Last.Value.GetComponent<Transform>().position.x < spawnThreshold)
        { 
            var last = spawnedObjects.Count > 0 ? spawnedObjects.Last.Value : null;
            var item = Instantiate(pickObjectToSpawn(last));
            Vector3 position = defaultSpawnPoint;
            if (last != null) {
                position.x = last.GetComponent<Collider2D>().bounds.max.x + item.GetComponent<Collider2D>().bounds.extents.x;
            }
            
            item.transform.position = position;
            spawnedObjects.AddLast(item);
        }
    }
}
