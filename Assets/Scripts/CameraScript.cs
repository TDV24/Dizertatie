using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    GameObject playercar;
    Vector3 offset = new Vector3(0f, 1.5f, -4f);
    int cameratype = 0;

    // Start is called before the first frame update
    void Start()
    {
        if(PlayerPrefs.GetInt("GameMode") == 1)
        {
            playercar = GameObject.Find(PlayerPrefs.GetString("RaceDriver"));
            playercar.transform.Find("Camera").gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (cameratype == 0) 
            {
                playercar.transform.Find("Camera").gameObject.transform.Translate(0f, -0.9f, 3.5f);
                cameratype = 1;
            }            
            else
            {
                playercar.transform.Find("Camera").gameObject.transform.Translate(0f, 0.9f, -3.5f);
                cameratype = 0;
            }
        }
    }
}
