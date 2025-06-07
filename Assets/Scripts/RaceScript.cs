using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RaceScript : MonoBehaviour
{
    public TextMeshProUGUI laptext;
    public TextMeshProUGUI laptimetext;
    public TextMeshProUGUI bestlaptext;
    public TextMeshProUGUI positiontext;
    float laptime;
    float besttime = Mathf.Infinity;
    bool Timing;
    public GameObject Endpanel;
    public GameObject Pausepanel;
    public GameObject Redlights;
    public AudioSource audioSource;
    public AudioClip redbeep;
    public int remainingtime = 7;
    public GameObject[] cars;
    public GameObject[] gridslots;
    GameObject playercar;
    List<GameObject> spawnedcars = new List<GameObject>();
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
        Time.timeScale = 1.0f;
        GameObject startline = GameObject.Find("StartingLine");
        Transform grid = startline.transform.Find("GridSlots");
        List<GameObject> list = new List<GameObject>();
        for(int i = 0; i < grid.childCount; i++)
        {
            list.Add(grid.transform.GetChild(i).gameObject);
        }
        gridslots = list.ToArray();
        if(PlayerPrefs.GetInt("GameMode") == 1)
        {
            StopCoroutine(StartRaceRoutine());
            ShuffleCars(cars);
            for(int i = 0; i < cars.Length; i++) 
            {
                GameObject car = Instantiate(cars[i],
                    new Vector3(gridslots[i].transform.position.x, 0.7f, gridslots[i].transform.position.z), gridslots[i].transform.rotation);
                spawnedcars.Add(car);
            }
            playercar = GameObject.Find(PlayerPrefs.GetString("RaceDriver") + "(Clone)");
            StartCoroutine(StartRaceRoutine());
        }
        
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
            laptext.text = "Laps: " + car16laps.ToString() + "/3";
            laptime += Time.deltaTime;
        }
        if(car16laps == 4)
        {
            Endpanel.SetActive(true);
            Time.timeScale = 0.0f;
            positionCount();

        }
        laptimetext.text = "Lap Time: " + FormatTime(laptime);
        if (besttime == Mathf.Infinity)
            bestlaptext.text = "Best Time: " + FormatTime(0f);
        else
            bestlaptext.text = "Best Time: " + FormatTime(besttime);
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
        {
            if(car16laps > 0)
            {
                Timing = false;
                if(laptime < besttime)
                {
                    besttime = laptime;
                }
            }
            car16laps++;
            Timing = true;
            laptime = 0f;
        }
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
        if (name == playercar.name)
        {
            if (car16laps > 0)
            {
                Timing = false;
                if (laptime < besttime)
                {
                    besttime = laptime;
                }
            }
            car16laps++;
            Timing = true;
            laptime = 0f;
        }
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
        foreach (GameObject car in spawnedcars)
        {
            Destroy(car);
        }
        spawnedcars.Clear();
        SceneManager.LoadScene("Menu");
    }
    public void Pause()
    {
        Time.timeScale = 0.0f;
        Pausepanel.SetActive(true);
    }
    public void Resume()
    {
        Time.timeScale = 1.0f;
        Pausepanel.SetActive(false);
    }
    IEnumerator StartRaceRoutine()
    {
        List<GameObject> lights = new List<GameObject>();
        foreach (Transform light in Redlights.GetComponentsInChildren<Transform>(true)) 
        {
            lights.Add(light.gameObject);
        }
        while(remainingtime > 0)
        {
            if(remainingtime == 5)
            {
                lights[1].SetActive(true);
                audioSource.PlayOneShot(redbeep);
            }
            if (remainingtime == 4)
            {
                lights[2].SetActive(true);
                audioSource.PlayOneShot(redbeep);
            }
            if (remainingtime == 3)
            {
                lights[3].SetActive(true);
                audioSource.PlayOneShot(redbeep);
            }
            if (remainingtime == 2)
            {
                lights[4].SetActive(true);
                audioSource.PlayOneShot(redbeep);
            }
            if (remainingtime == 1)
            {
                lights[5].SetActive(true);
                audioSource.PlayOneShot(redbeep);
            }
            yield return new WaitForSeconds(1f);
            remainingtime--;
        }
        lights[1].SetActive(false);
        lights[2].SetActive(false);
        lights[3].SetActive(false);
        lights[4].SetActive(false);
        lights[5].SetActive(false);
        yield return new WaitForSeconds(1f);
        foreach (GameObject car in spawnedcars)
        {
            if (car.name == playercar.name)
                car.GetComponent<CarController>().enabled = true;
            else
                car.GetComponent<AICar>().enabled = true;
        }
    }
    string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        int milliseconds = Mathf.FloorToInt((time * 100) % 100);
        return string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);
    }
    void ShuffleCars(GameObject[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            int randomIndex = Random.Range(i, array.Length);
            GameObject temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }
}
