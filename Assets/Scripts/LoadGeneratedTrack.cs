using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;
using static GenerateTrack;

public class LoadGeneratedTrack : MonoBehaviour
{
    public GameObject startingLine;
    public GameObject road;
    public GameObject tribune;
    public GameObject tree;
    public GameObject canvas;
    public GameObject Waypoints;
    // Start is called before the first frame update
    void Start()
    {
        LoadScene();
        canvas.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void LoadScene()
    {
        string filename = PlayerPrefs.GetString("FileName");
        string folderPath = Path.Combine(Application.persistentDataPath, "GeneratedTracks");
        string path = Path.Combine(folderPath, filename + ".json");
        string json = File.ReadAllText(path);
        SceneData data = JsonUtility.FromJson<SceneData>(json);
        for (int i = 0; i < data.pos.Count; i++)
        {
            GameObject wp = new GameObject("Waypoint");
            wp.transform.position = data.pos[i];
            wp.transform.parent = Waypoints.transform;
        }
        if (data.startingLine != null)
        {
            GameObject obj = Instantiate(startingLine, data.startingLine.position, data.startingLine.rotation);
            obj.transform.localScale = data.startingLine.scale;
            obj.name = "StartingLine";
        }
        foreach (TransformData t in data.roads)
        {
            GameObject obj = Instantiate(road, t.position, t.rotation);
            obj.transform.localScale = t.scale;
        }
        foreach (TransformData t in data.tribunes)
        {
            GameObject obj = Instantiate(tribune, t.position, t.rotation);
            obj.transform.localScale = t.scale;
        }
        foreach (Vector3 pos in data.trees)
        {
            Instantiate(tree, pos, Quaternion.identity);
        }
    }
}
