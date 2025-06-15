using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    GameObject playercar;
    Vector3 offset = new Vector3(0f, 1.5f, -4f);
    int cameratype = 0;
    bool cameraFound = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!cameraFound)
        {
            if (PlayerPrefs.GetInt("GameMode") == 1)
            {
                if (GameObject.Find(PlayerPrefs.GetString("RaceDriver") + "(Clone)") != null)
                {
                    playercar = GameObject.Find(PlayerPrefs.GetString("RaceDriver") + "(Clone)");
                    playercar.transform.Find("Camera").gameObject.SetActive(true);
                    cameraFound = true;
                }
            }
            if (PlayerPrefs.GetInt("GameMode") == 2 || PlayerPrefs.GetInt("GameMode") == 3)
            {
                if (GameObject.Find(PlayerPrefs.GetString("GPDriver") + "(Clone)") != null)
                {
                    playercar = GameObject.Find(PlayerPrefs.GetString("GPDriver") + "(Clone)");
                    playercar.transform.Find("Camera").gameObject.SetActive(true);
                    cameraFound = true;
                }
            }
            if (PlayerPrefs.GetInt("GameMode") == 4 || PlayerPrefs.GetInt("GameMode") == 5)
            {
                if (GameObject.Find(PlayerPrefs.GetString("SeasonDriver") + "(Clone)") != null)
                {
                    playercar = GameObject.Find(PlayerPrefs.GetString("SeasonDriver") + "(Clone)");
                    playercar.transform.Find("Camera").gameObject.SetActive(true);
                    cameraFound = true;
                }
            }
        }
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
