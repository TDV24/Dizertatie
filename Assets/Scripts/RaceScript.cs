using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public Dictionary<GameObject, CarRaceData> raceData = new Dictionary<GameObject, CarRaceData>();
    public GameObject[] Waypoints;
    public GameObject pointsobj;
    public bool raceended = false;
    int totallaps;
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
        GameObject points = GameObject.Find("Waypoints");
        List<GameObject> wlist = new List<GameObject>();
        for (int i = 0; i < points.transform.childCount; i++)
        {
            wlist.Add(points.transform.GetChild(i).gameObject);
        }
        Waypoints = list.ToArray();
        if (PlayerPrefs.GetInt("GameMode") == 1)
        {
            totallaps = 3;
            StopCoroutine(StartRaceRoutine());
            ShuffleCars(cars);
            for(int i = 0; i < cars.Length; i++) 
            {
                GameObject car = Instantiate(cars[i],
                    new Vector3(gridslots[i].transform.position.x, 0.7f, gridslots[i].transform.position.z), gridslots[i].transform.rotation);
                spawnedcars.Add(car); 
                raceData[car] = new CarRaceData();
            }
            playercar = GameObject.Find(PlayerPrefs.GetString("RaceDriver") + "(Clone)");
            StartCoroutine(StartRaceRoutine());
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject car in spawnedcars) 
        {
            if (raceData[car].lap > 0)
                UpdateCarProgress(car);
        }
        if (raceData[playercar].lap == 0)
        {
            laptext.text = "Laps: 1/" + totallaps.ToString();
        }
        else
        {
            laptext.text = "Laps: " + raceData[playercar].lap.ToString() + "/" +totallaps.ToString();
            laptime += Time.deltaTime;
                
        }
        if(raceData[playercar].lap > totallaps && !raceended)
        {
            raceended = true;
            Endpanel.SetActive(true);
            Time.timeScale = 0.0f;
            positionCount();
            if(PlayerPrefs.GetInt("GameMode") == 1 || PlayerPrefs.GetInt("GameMode") == 3 || PlayerPrefs.GetInt("GameMode") == 5)
                pointsobj.SetActive(true);
        }
        laptimetext.text = "Lap Time: " + FormatTime(laptime);
        if (besttime == Mathf.Infinity)
            bestlaptext.text = "Best Time: " + FormatTime(0f);
        else
            bestlaptext.text = "Best Time: " + FormatTime(besttime);
    }

    public void increaseLaps(GameObject car)
    {
        if (car == playercar && raceData[playercar].lap > 0)
        {
            Timing = false;
            if (laptime < besttime)
            {
                besttime = laptime;
            }
            Timing = true;
            laptime = 0f;
        }
        raceData[car].lap++;
    }
    void positionCount()
    {
        var ordered = raceData.OrderByDescending(kv => kv.Value.lap)
                              .ThenByDescending(kv => kv.Value.currentWaypointIndex)
                              .ThenByDescending(kv => kv.Value.progressBetweenWaypoints)
                              .Select(kv => kv.Key)
                              .ToList();
        for(int i = 0; i < ordered.Count; i++) 
        {
            GameObject car = ordered[i];
            string name = car.name;
            if(name == playercar.name)
            {
                positiontext.text += $"<color=orange>{i + 1}. {name.Substring(0, name.Length - 7)}</color>\n";
            }
            else
            {
                positiontext.text += $"{i + 1}. {name.Substring(0, name.Length - 7)}\n";
            }
        }
    }
    void UpdateCarProgress(GameObject car)
    {
        var data = raceData[car];
        Vector3 carPos = car.transform.position;
        int currentIndex = data.currentWaypointIndex;
        int nextIndex = (currentIndex + 1) % Waypoints.Length;
        Vector3 currentWP = Waypoints[currentIndex].transform.position;
        Vector3 nextWP = Waypoints[nextIndex].transform.position;
        float segmentLength = Vector3.Distance(currentWP, nextWP);
        float distToNextWP = Vector3.Distance(carPos, nextWP);
        float progress = Mathf.Clamp01(1f - distToNextWP / segmentLength);

        if (distToNextWP < 2f) 
        {
            data.currentWaypointIndex = nextIndex;
        }

        data.progressBetweenWaypoints = progress;
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
    public class CarRaceData
    {
        public int lap = 0;
        public int currentWaypointIndex = 0;
        public float progressBetweenWaypoints = 0f;
    }
}
