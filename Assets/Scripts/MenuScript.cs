using Palmmedia.ReportGenerator.Core.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    public TMP_InputField corners;
    public TMP_InputField width;
    public TMP_InputField length;
    public GameObject errorText1;
    public GameObject errorText2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void StartRace()
    {
        SceneManager.LoadScene("Track01");
    }
    public void GenerateTrack()
    {
        if (int.Parse(corners.text) < 2)
        {
            errorText1.gameObject.SetActive(true);
            return;
        }
        if(int.Parse(width.text) < 100 || int.Parse(length.text) < 100) 
        { 
            errorText2.gameObject.SetActive(true);
            return;
        }
        PlayerPrefs.SetInt("TrackWidth", int.Parse(width.text));
        PlayerPrefs.SetInt("TrackLength", int.Parse(length.text));
        PlayerPrefs.SetInt("noOfCorners", int.Parse(corners.text));
        SceneManager.LoadScene("GeneratedTrack");
    }
}
