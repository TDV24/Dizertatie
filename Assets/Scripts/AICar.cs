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
    public float acceleration = 10.0f;
    public float rotationSpeed = 8.0f;
    public float maxSpeed = 4.0f;
    public float speed = 0;
    Rigidbody rb;
    // Start is called before the first frame update
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        GameObject points = GameObject.Find("Waypoints");
        List<GameObject> list = new List<GameObject>();
        for (int i = 0; i < points.transform.childCount; i++)
        {
            list.Add(points.transform.GetChild(i).gameObject);
        }
        waypoints = list.ToArray();
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }
    private void OnEnable()
    {
        GameObject points = GameObject.Find("Waypoints");
        List<GameObject> list = new List<GameObject>();
        for (int i = 0; i < points.transform.childCount; i++)
        {
            list.Add(points.transform.GetChild(i).gameObject);
        }
        waypoints = list.ToArray();
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        if(speed < maxSpeed)
        {
            speed += acceleration * Time.deltaTime;
        }
        transform.rotation = Quaternion.Slerp(transform.rotation,
            Quaternion.LookRotation(waypoints[index].transform.position - transform.position),
            rotationSpeed * Time.deltaTime);
        transform.position += transform.forward * speed * Time.deltaTime;
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
