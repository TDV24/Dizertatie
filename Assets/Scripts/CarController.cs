using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCar : MonoBehaviour
{
    public static PlayerCar instance;
    public float speed = 0;
    public float maxSpeed = 0.18f;
    public float acceleration = 0.02f;
    Vector3 moveDirection;
    CharacterController characterController;
    // Start is called before the first frame update
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        moveDirection = transform.TransformDirection(moveDirection);
        if (Input.GetKey(KeyCode.W))
        {
            if (speed <= maxSpeed)
            {
                speed += acceleration * Time.deltaTime;
            }
        }
        if (Input.GetKey(KeyCode.S))
        {
            if (speed > 0.01)
            {
                speed -= acceleration * Time.deltaTime * 2;
            }
        }
        if (Input.GetKey(KeyCode.A))
        {
            gameObject.transform.Rotate(0, -270.0f * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            gameObject.transform.Rotate(0, 270.0f * Time.deltaTime, 0);
        }
        moveDirection *= speed;
        moveDirection.y -= (50.8f * Time.deltaTime);
        characterController.Move(moveDirection * Time.deltaTime);

    }
}
