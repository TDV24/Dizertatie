using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CarController : MonoBehaviour
{
    Vector3 MovementInput;
    public float acceleration = 12f;
    public float steering = 1f;
    public float maxSpeed = 10f;
    [SerializeField]private float speed = 0f;

    private Rigidbody rb;
    private Quaternion turnRotation;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }
    private void OnEnable()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }
    void Update()
    {
        // Accelerate/Brake
        if (Input.GetKey(KeyCode.W))
        {
            if(speed < maxSpeed)
                speed += acceleration * Time.deltaTime;
        }
        else 
        {
            if (speed > 0)
                speed -= acceleration * Time.deltaTime;
        }
        if(Input.GetKeyDown(KeyCode.S))
        {
            if(speed > 0)
                speed -= acceleration * Time.deltaTime;
        }   
        // Get input and move
        MovementInput = new Vector3(0f, 0f, 1f);
        Vector3 moveVector = transform.TransformDirection(MovementInput) * speed;
        rb.velocity = new Vector3(moveVector.x, rb.velocity.y, moveVector.z);

        if(Mathf.Abs(speed) > 0.1f)
        {
            float turn = steering * Time.fixedDeltaTime;
            turnRotation = Quaternion.Euler(0f, 0f, 0f);
            if (Input.GetKey(KeyCode.A))
            {
                turnRotation = Quaternion.Euler(0f, -turn, 0f);
            }
            if(Input.GetKey(KeyCode.D)) 
            {
                turnRotation = Quaternion.Euler(0f, turn, 0f);
            }
            rb.MoveRotation(rb.rotation * turnRotation);
        }
    }
}
