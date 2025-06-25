using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
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
    public AudioSource audioSource1;
    public AudioClip redbeep;
    public AudioClip clutch;
    public int remainingtime = 7;
    public GameObject[] cars;
    public GameObject[] gridslots;
    GameObject playercar;
    List<GameObject> spawnedcars = new List<GameObject>();
    public Dictionary<GameObject, CarRaceData> raceData = new Dictionary<GameObject, CarRaceData>();
    public GameObject[] Waypoints;
    public GameObject pointsobj;
    public TextMeshProUGUI timesobj;
    public bool raceended = false;
    int totallaps;
    int[] pointsShared = new int[10] {25, 18, 15, 12, 10, 8, 6, 4, 2, 1 };
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
        if (PlayerPrefs.GetInt("GameMode") == 2)
        {
            totallaps = 1;
            StopCoroutine(StartRaceRoutine());
            for(int i = 0; i < cars.Length; i++)
            {
                if (cars[i].name == PlayerPrefs.GetString("GPDriver"))
                {
                    GameObject car = Instantiate(cars[i],
                    new Vector3(gridslots[0].transform.position.x, 0.7f, gridslots[0].transform.position.z), gridslots[0].transform.rotation);
                    spawnedcars.Add(car);
                    raceData[car] = new CarRaceData();
                }
            }
            playercar = GameObject.Find(PlayerPrefs.GetString("GPDriver") + "(Clone)");
            StartCoroutine(StartRaceRoutine());
        }
        if (PlayerPrefs.GetInt("GameMode") == 3)
        {
            totallaps = 3;
            StopCoroutine(StartRaceRoutine());
            for (int i = 0; i < DataScript.QualifyingGPOrder.Count; i++)
            {
                GameObject car = Instantiate(DataScript.QualifyingGPOrder[i],
                    new Vector3(gridslots[i].transform.position.x, 0.7f, gridslots[i].transform.position.z), gridslots[i].transform.rotation);
                spawnedcars.Add(car);
                raceData[car] = new CarRaceData();
            }
            playercar = GameObject.Find(PlayerPrefs.GetString("GPDriver") + "(Clone)");
            StartCoroutine(StartRaceRoutine());
        }
        if (PlayerPrefs.GetInt("GameMode") == 4)
        {
            totallaps = 1;
            StopCoroutine(StartRaceRoutine());
            for (int i = 0; i < cars.Length; i++)
            {
                if (cars[i].name == PlayerPrefs.GetString("SeasonDriver"))
                {
                    GameObject car = Instantiate(cars[i],
                    new Vector3(gridslots[0].transform.position.x, 0.7f, gridslots[0].transform.position.z), gridslots[0].transform.rotation);
                    spawnedcars.Add(car);
                    raceData[car] = new CarRaceData();
                }
            }
            playercar = GameObject.Find(PlayerPrefs.GetString("SeasonDriver") + "(Clone)");
            StartCoroutine(StartRaceRoutine());
        }
        if (PlayerPrefs.GetInt("GameMode") == 5)
        {
            totallaps = 3;
            StopCoroutine(StartRaceRoutine());
            for (int i = 0; i < DataScript.QualifyingChampionshipOrder.Count; i++)
            {
                GameObject car = Instantiate(DataScript.QualifyingChampionshipOrder[i],
                    new Vector3(gridslots[i].transform.position.x, 0.7f, gridslots[i].transform.position.z), gridslots[i].transform.rotation);
                spawnedcars.Add(car);
                raceData[car] = new CarRaceData();
            }
            playercar = GameObject.Find(PlayerPrefs.GetString("SeasonDriver") + "(Clone)");
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
            if (PlayerPrefs.GetInt("GameMode") == 1 || PlayerPrefs.GetInt("GameMode") == 3 || PlayerPrefs.GetInt("GameMode") == 5)
            {
                positionCount();
                pointsobj.SetActive(true);
                if(PlayerPrefs.GetInt("GameMode") == 3)
                    PlayerPrefs.SetInt("GPQualy", 0);
                if(PlayerPrefs.GetInt("GameMode") == 5)
                {
                    PlayerPrefs.SetInt("SeasonQualy", 0);
                    PlayerPrefs.SetInt("CurrentRound", PlayerPrefs.GetInt("CurrentRound") + 1);
                }     
            }
            if(PlayerPrefs.GetInt("GameMode") == 2 || PlayerPrefs.GetInt("GameMode") == 4)
            {
                timesobj.gameObject.SetActive(true);
                SimulateQualifying();
                if (PlayerPrefs.GetInt("GameMode") == 2)
                    PlayerPrefs.SetInt("GPQualy", 1);
                if (PlayerPrefs.GetInt("GameMode") == 4)
                    PlayerPrefs.SetInt("SeasonQualy", 1);
            }
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
        if (raceData[car].lap > totallaps)
        {
            raceData[car].finished = true;
            raceData[car].finishtime = Time.time;
        }
    }
    void positionCount()
    {
        var ordered = raceData.OrderBy(kv => kv.Value.finished ? 0 : 1)
                              .ThenBy(kv => kv.Value.finished ? kv.Value.finishtime : -kv.Value.totalprogress)
                              .Select(kv => kv.Key)
                              .ToList();
        for(int i = 0; i < ordered.Count; i++) 
        {
            GameObject car = ordered[i];
            string name = car.name;
            if(name == playercar.name)
            {
                positiontext.text += $"<color=orange>{i + 1}. {name.Substring(0, name.Length - 7)}</color>\n";
                if(PlayerPrefs.GetInt("GameMode") == 5)
                {
                    PlayerPrefs.SetInt("Money", PlayerPrefs.GetInt("Money") + 4000 - i * 100);
                }
            }
            else
            {
                positiontext.text += $"{i + 1}. {name.Substring(0, name.Length - 7)}\n";
            }
        }
        if(PlayerPrefs.GetInt("GameMode") == 5)
        {
            for (int i = 0; i < pointsShared.Length; i++)
            {
                PlayerPrefs.SetInt(ordered[i].name.Substring(0, ordered[i].name.Length - 7) + "Points", 
                    PlayerPrefs.GetInt(ordered[i].name.Substring(0, ordered[i].name.Length - 7) + "Points") + pointsShared[i]);
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
        data.totalprogress = data.lap * Waypoints.Length + data.currentWaypointIndex + data.progressBetweenWaypoints;
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
                audioSource1.PlayOneShot(clutch);
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
    void SimulateQualifying()
    {
        AICar playerStats = playercar.GetComponent<AICar>();
        float playerHandling = Mathf.Clamp(playerStats.rotationSpeed, 4f, 12f);
        float playerMaxSpeed = Mathf.Clamp(playerStats.maxSpeed, 30f, 44f);
        float playerHandlingFactor = Mathf.InverseLerp(4f, 12f, playerHandling);
        float playerSpeedFactor = Mathf.InverseLerp(30f, 44f, playerMaxSpeed);
        float playerSkill = playerHandlingFactor * 0.65f + playerSpeedFactor * 0.35f;
        Dictionary<GameObject, float> carTimes = new Dictionary<GameObject, float>();
        List<(GameObject car, float skill)> carSkills = new List<(GameObject, float)>();
        if (PlayerPrefs.GetInt("GameMode") == 2)
        {
            DataScript.QualifyingGPOrder.Clear();
            foreach (var car in cars)
            {
                if (car.name == PlayerPrefs.GetString("GPDriver"))
                {
                    carTimes.Add(car, besttime);
                    continue;
                }

                AICar stats = car.GetComponent<AICar>();
                float handling = Mathf.Clamp(stats.rotationSpeed, 4f, 12f);
                float maxspeed = Mathf.Clamp(stats.maxSpeed, 30f, 44f);

                float handlingFactor = Mathf.InverseLerp(4f, 12f, handling);
                float speedFactor = Mathf.InverseLerp(30f, 44f, maxspeed);
                float aiSkill = handlingFactor * 0.65f + speedFactor * 0.35f;
                float skillDifference = playerSkill - aiSkill;
                float performance = 1f + (skillDifference * 0.3f);
                float variation = Random.Range(0.90f, 1.1f);
                float simulatedTime = besttime * performance * variation;
                carTimes.Add(car, simulatedTime);
            }
            timesobj.text += "Time\n";
            var ordered = carTimes.OrderBy(kv => kv.Value).ToList();
            foreach (var place in ordered)
                DataScript.QualifyingGPOrder.Add(place.Key);
            for (int i = 0; i < ordered.Count; i++)
            {
                GameObject car = ordered[i].Key;
                string name = car.name;
                if (name == PlayerPrefs.GetString("GPDriver"))
                {
                    positiontext.text += $"<color=orange>{i + 1}. {name}</color>\n";
                    timesobj.text += $"<color=orange>{FormatTime(ordered[i].Value)}</color>\n";
                }
                else
                {
                    positiontext.text += $"{i + 1}. {name}\n";
                    timesobj.text += FormatTime(ordered[i].Value) + "\n";
                }
            }
        }
        if (PlayerPrefs.GetInt("GameMode") == 4)
        {
            DataScript.QualifyingChampionshipOrder.Clear();
            foreach (var car in cars)
            {
                if (car.name == PlayerPrefs.GetString("SeasonDriver"))
                {
                    carTimes.Add(car, besttime);
                    continue;
                }

                AICar stats = car.GetComponent<AICar>();
                float handling = Mathf.Clamp(stats.rotationSpeed, 4f, 12f);
                float maxspeed = Mathf.Clamp(stats.maxSpeed, 30f, 44f);

                float handlingFactor = Mathf.InverseLerp(4f, 12f, handling);
                float speedFactor = Mathf.InverseLerp(30f, 44f, maxspeed);
                float aiSkill = handlingFactor * 0.65f + speedFactor * 0.35f;
                float skillDifference = playerSkill - aiSkill;
                float performance = 1f + (skillDifference * 0.3f);
                float variation = Random.Range(0.90f, 1.1f);
                float simulatedTime = besttime * performance * variation;
                carTimes.Add(car, simulatedTime);
            }
            timesobj.text += "Time\n";
            var ordered = carTimes.OrderBy(kv => kv.Value).ToList();
            foreach (var place in ordered)
                DataScript.QualifyingChampionshipOrder.Add(place.Key);
            for (int i = 0; i < ordered.Count; i++)
            {
                GameObject car = ordered[i].Key;
                string name = car.name;
                if (name == PlayerPrefs.GetString("SeasonDriver"))
                {
                    positiontext.text += $"<color=orange>{i + 1}. {name}</color>\n";
                    timesobj.text += $"<color=orange>{FormatTime(ordered[i].Value)}</color>\n";
                }
                else
                {
                    positiontext.text += $"{i + 1}. {name}\n";
                    timesobj.text += FormatTime(ordered[i].Value) + "\n";
                }
            }
        }
    }
    public class CarRaceData
    {
        public int lap = 0;
        public int currentWaypointIndex = 0;
        public float progressBetweenWaypoints = 0f;
        public float totalprogress = 0f;
        public bool finished = false;
        public float finishtime = 0f;
    }
}
