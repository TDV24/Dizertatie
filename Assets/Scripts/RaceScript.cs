using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RaceScript : MonoBehaviour
{
    public TextMeshProUGUI laptext;
    public TextMeshProUGUI positiontext;
    public GameObject panel;
    int car1laps = 0;
    int car2laps = 0;
    int car4laps = 0;
    int car11laps = 0;
    int car16laps = 0;
    int car23laps = 0;
    int car44laps = 0;
    int car55laps = 0;
    int car63laps = 0;
    int car81laps = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(car16laps == 0)
        {
            laptext.text = "Laps: 1/2";
        }
        else
        {
            laptext.text = "Laps: " + car16laps.ToString() + "/2";
        }
        if(car16laps == 3)
        {
            panel.SetActive(true);
            Time.timeScale = 0.0f;
            positionCount();

        }
    }

    public void increaseLaps(string name)
    {
        if(name == "Car#1")
            car1laps++;
        if (name == "Car#2")
            car2laps++;
        if (name == "Car#4")
            car4laps++;
        if (name == "Car#11")
            car11laps++;
        if (name == "Car#16")
            car16laps++;
        if (name == "Car#23")
            car23laps++;
        if (name == "Car#44")
            car44laps++;
        if (name == "Car#55")
            car55laps++;
        if (name == "Car#63")
            car63laps++;
        if (name == "Car#81")
            car81laps++;
    }
    void positionCount()
    {
        int countPos = 1;
        if (car1laps == 3)
            countPos++;
        if (car2laps == 3)
            countPos++;
        if (car4laps == 3)
            countPos++;
        if (car11laps == 3)
            countPos++;
        if (car23laps == 3)
            countPos++;
        if (car44laps == 3)
            countPos++;
        if (car55laps == 3)
            countPos++;
        if (car63laps == 3)
            countPos++;
        if (car81laps == 3)
            countPos++;
        if (countPos == 1)
            positiontext.text = "Final position: 1st";
        else if (countPos == 2)
            positiontext.text = "Final position: 2nd";
        else if (countPos == 3)
            positiontext.text = "Final position: 3rd";
        else
            positiontext.text = "Final position: " + countPos.ToString() + "th";
    }
    public void ExitRace()
    {
        SceneManager.LoadScene("Menu");
    }
}
