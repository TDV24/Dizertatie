using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.AI;

public class AICar : MonoBehaviour
{
    public static AICar instance;
    int index;
    public GameObject[] waypoints;
    public float rotationSpeed = 8.0f;
    public float maxSpeed = 4.0f;
    // Start is called before the first frame update
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation,
            Quaternion.LookRotation(waypoints[index].transform.position - transform.position),
            rotationSpeed * Time.deltaTime);
        transform.position += transform.forward * maxSpeed * Time.deltaTime;
        if (Vector3.Distance(waypoints[index].transform.position, transform.position) < 2)
        {
            if(index >= waypoints.Length - 1)
            {
                index = 0;
            }
            else
            {
                index++;
            }
        }
    }
}
