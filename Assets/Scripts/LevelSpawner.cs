using UnityEngine;
using System.Collections.Generic;
using System;

using SectionTuple = System.Tuple<UnityEngine.GameObject, UnityEngine.GameObject, UnityEngine.GameObject>;
using Unity.VisualScripting;
public class LevelSpawner : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject parentType;
    public List<GameObject> floorTypes;
    public List<GameObject> obstacleTypes;
    public Transform chaserTransform;
    public Camera cameraObject;
    public int spawnBuffer = 1000;
    public int despawnBuffer = 1000;

    public Vector3 defaultSpawnPoint = new Vector3(0, 0, 0);

    
    private LinkedList<SectionTuple> spawnedObjects = new LinkedList<SectionTuple>();

    

    
    
    void Start()
    {
        Debug.Assert(floorTypes.Count > 0);
        Debug.Assert(obstacleTypes.Count > 0);
    }

    GameObject pickRandomItem(List<GameObject> items)
    {
        var item = UnityEngine.Random.Range(0, items.Count);
        return items[item];
    }

    GameObject pickObjectToSpawn(GameObject previous)
    {
        // TODO Filter out incompatable combinations
        return pickRandomItem(floorTypes);
    }


    Bounds getAllBounds(GameObject item)
    {
        var childrenBounds = item.GetComponentsInChildren<Collider2D>();
        Bounds bounds = item.GetComponent<Collider2D>() ? item.GetComponent<Collider2D>().bounds : childrenBounds[0].bounds;
        foreach (var child in childrenBounds)
        {
            bounds.Encapsulate(child.bounds);
        }
        return bounds;
    }

    GameObject attachObstacle(GameObject parent, GameObject currentFloor, SectionTuple lastSection)
    {
        // Skip obstacle on the first item, or if the previous obstacle was wide
        if(lastSection == null || (lastSection.Item3 != null && lastSection.Item3.tag == "wide"))
        {
            return null; // Previous obstacle is extra wide
        }
        // TODO filter out incompatable combinations
        var obstacleType = pickRandomItem(obstacleTypes);
        var obstacle = Instantiate(obstacleType, parent.transform);
        obstacle.transform.localPosition = Vector3.zero;
        // Obstacles are hard coded to be at left bottom aligned
        //var obstacleBounds = getAllBounds(obstacle); // Get the obstacle's bounds
        //Debug.Log("Obstacle bounds " + obstacleBounds);
        //var position = new Vector3(obstacleBounds.extents.x, obstacleBounds.extents.y, 0);
        //obstacle.transform.localPosition = position;
        return obstacle;
    }

    GameObject attachFloor(GameObject parent, SectionTuple last)
    {
        var floorType = pickRandomItem(floorTypes);
        var floor = Instantiate(floorType, parent.transform);
        floor.transform.localPosition = Vector3.zero;
        var floorBounds = getAllBounds(floor);
        floor.transform.localPosition = new Vector3(floorBounds.extents.x, -floorBounds.extents.y, 0);
        Debug.Log("Parent position " + parent.transform.localPosition);
        Debug.Log("Floor local position" + floor.transform.localPosition);
        Debug.Log("Floor global position" + floor.transform.position);
        var newFloorBounds = getAllBounds(floor);
        Debug.Log("Collider bounds max " + newFloorBounds.max);
        Debug.Log("Collider bounds extents " + newFloorBounds.extents);
        return floor;
    }

    // Update is called once per frame
    void Update()
    {
        Camera cam = Camera.main;
        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;

        // Despawn objects that are consumed
        var despawnThreshold = chaserTransform.position.x - despawnBuffer;
        while (spawnedObjects.Count > 0 && spawnedObjects.First.Value.Item1.GetComponent<Transform>().position.x < despawnThreshold)
        {
            var top = spawnedObjects.First.Value;
            spawnedObjects.RemoveFirst();
            Destroy(top.Item1);
        }

        // Spawn objects if needed
        var spawnThreshold = Camera.main.transform.position.x + width / 2 + spawnBuffer;

        var bottomPosition = Camera.main.transform.position.y - height / 2;


        // Spawn only one object per frame
        if (spawnedObjects.Count == 0 || spawnedObjects.Last.Value.Item1.GetComponent<Transform>().position.x < spawnThreshold)
        { 
            var last = spawnedObjects.Count > 0 ? spawnedObjects.Last.Value : null;
            var parent = Instantiate(parentType);
 
            Vector3 position = new Vector3(defaultSpawnPoint.x, bottomPosition + defaultSpawnPoint.y, defaultSpawnPoint.z);
            if (last != null) {
                var lastFloorBounds = getAllBounds(last.Item2);
                position.x = last.Item1.transform.position.x + lastFloorBounds.extents.x * 2;
                Debug.Log("Last end position " + lastFloorBounds);
            }
            
            parent.transform.position = position;
            Debug.Log("Next position " + position);

            var floor = attachFloor(parent, last);
            var obstacle = attachObstacle(parent, floor, last);

            spawnedObjects.AddLast(new SectionTuple(parent, floor, obstacle));
        }
    }
}
