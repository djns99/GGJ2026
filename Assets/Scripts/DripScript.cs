using System;
using UnityEngine;

public class DripScript : MonoBehaviour
{
    public GameObject dripType;
    public float dripRate = 3.0f;
    public float dripVariance = 0.2f;

    private float timeSinceDrip = 0.0f;
    private float meanDripTime = 0.0f;
    private float nextDripTime = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        meanDripTime = 1f / dripRate;
    }

    public static double NextGaussianDouble()
    {
        double u, v, S;

        do
        {
            u = 2.0 * UnityEngine.Random.value - 1.0;
            v = 2.0 * UnityEngine.Random.value - 1.0;
            S = u * u + v * v;
        }
        while (S >= 1.0);

        double fac = Math.Sqrt(-2.0 * Math.Log(S) / S);
        return u * fac;
    }


    // Update is called once per frame
    void Update()
    {
        timeSinceDrip += Time.deltaTime;

        if (timeSinceDrip >= nextDripTime)
        {
            var parentBounds = gameObject.GetComponent<Collider2D>().bounds;
            var range = parentBounds.extents.x;
            float pos = UnityEngine.Random.Range(-range, range);
            var drip = Instantiate(dripType);

            drip.GetComponent<DropletScript>().source = gameObject;
            drip.transform.position = gameObject.transform.position + new Vector3(pos, -0.5f, 1f);

            timeSinceDrip = 0.0f;
            nextDripTime = (float)NextGaussianDouble() * dripVariance + meanDripTime;
        }
    }
}
