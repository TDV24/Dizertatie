using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GenerateTrack : MonoBehaviour
{
    public GameObject corner;
    public GameObject roadPrefab;
    public GameObject startingLinePrefab;
    public GameObject savePanel;
    public GameObject tribune;
    public GameObject tree;
    public TMP_InputField inputField;
    public TextMeshProUGUI error;
    public int treenumber = 20;
    int noOfCorners;
    int width;
    int length;
    int maxIndex = 0;
    float maxDistance = 0f;
    private Vector3[] pos;
    List<GameObject> tribunes = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        width = PlayerPrefs.GetInt("TrackWidth");
        length = PlayerPrefs.GetInt("TrackLength");
        noOfCorners = PlayerPrefs.GetInt("noOfCorners");
        pos = new Vector3[noOfCorners];
        float halfLength = length / 2f;
        float halfWidth = width / 2f;
        for (int i = 0; i < noOfCorners; i++)
        {
            float angle = (360f / noOfCorners) * i;
            float rad = Mathf.Deg2Rad * angle;
            float x = Mathf.Cos(rad) * halfLength;
            float z = Mathf.Sin(rad) * halfWidth;
            float radius = Mathf.Sqrt(x * x + z * z);
            float noise = Random.Range(-radius * 0.3f, radius * 0.3f);
            float scale = (radius + noise) / radius;
            x *= scale;
            z *= scale;
            pos[i] = new Vector3(
                500 + x,
                0.3f,
                500 + z
            );
            Instantiate(corner, pos[i], Quaternion.identity);
        }
        float minSegmentLength = 0.1f;
        pos = PointsSorting(pos);
        Quaternion lastrotation = Quaternion.identity;
        for (int i = 0; i < pos.Length - 1; i++)
        {
            float dist = Vector3.Distance(pos[i], pos[i + 1]);
            if (dist > maxDistance)
            {
                maxDistance = dist;
                maxIndex = i;
            }
        }
        Debug.Log(pos[maxIndex]);
        List<Vector3> rotatedPos = new List<Vector3>();
        for (int i = maxIndex + 1; i < maxIndex + 1 + pos.Length; i++)
        {
            rotatedPos.Add(pos[i % pos.Length]);
        }
        pos = rotatedPos.ToArray();
        Debug.Log(pos[0]);
        List<Vector3> loopedPoints = pos.ToList();
        List<Vector3> curvePoints = GenerateCatmullRomSpline(loopedPoints, 6);
        curvePoints = FilterClosePoints(curvePoints, 1.0f);
        Vector3 start = pos[pos.Length - 1];
        Vector3 end = pos[0];

        Vector3 direction = end - start;
        Vector3 midPoint = start + direction / 2f;
        startingLinePrefab.transform.localScale = new Vector3(startingLinePrefab.transform.localScale.x, 
            startingLinePrefab.transform.localScale.y, Vector3.Distance(start, end));
        GameObject startLine = Instantiate(startingLinePrefab, midPoint, Quaternion.LookRotation(direction));
        Vector3 startLinePos = startLine.transform.position;
        startLinePos.y = 0;
        startLine.transform.position = startLinePos;
        for (int i = 0; i < curvePoints.Count - 1; i++) 
        {
            start = curvePoints[i];
            end = curvePoints[i + 1];
            direction = end - start;
            float distance = direction.magnitude;
            if (distance < minSegmentLength) continue;
            midPoint = start + direction / 2;
            GameObject road = Instantiate(roadPrefab, midPoint, Quaternion.LookRotation(direction));
            Vector3 scale = road.transform.localScale;
            scale.z = distance;
            road.transform.localScale = new Vector3(road.transform.localScale.x, road.transform.localScale.y, distance);
            Vector3 roadpos = road.transform.position;
            roadpos.y = 0;
            road.transform.position = roadpos;
        }
        List<Vector3> allPlacedPositions = new List<Vector3>(curvePoints);
        allPlacedPositions.AddRange(pos);
        allPlacedPositions.Add(pos[maxIndex] + (pos[maxIndex + 1] - pos[maxIndex]) / 2f);
        PlaceTribune(allPlacedPositions, curvePoints);
        PlaceTrees(allPlacedPositions, curvePoints);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z)) 
        {
            SceneManager.LoadScene("Menu");
            savePanel.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            savePanel.SetActive(true);
        }
    }

    Vector3[] PointsSorting(Vector3[] positions)
    {
        List<Vector3> sorted = new List<Vector3>();
        List<Vector3> unsorted = new List<Vector3>(positions);

        Vector3 current = unsorted[0];
        sorted.Add(current);
        unsorted.RemoveAt(0);

        while (unsorted.Count > 0)
        {
            Vector3 nearest = unsorted.OrderBy(p => Vector3.Distance(current, p)).First();
            sorted.Add(nearest);
            current = nearest;
            unsorted.Remove(nearest);
        }
        sorted.Add(sorted[0]);
        return sorted.ToArray();
    }
    List<Vector3> GenerateCatmullRomSpline(List<Vector3> points, int resolution = 10)
    {
        List<Vector3> curvePoints = new List<Vector3>();

        int count = points.Count;
        for (int i = 0; i < count; i++)
        {
            Vector3 p0 = points[(i - 1 + count) % count];
            Vector3 p1 = points[i];
            Vector3 p2 = points[(i + 1) % count];
            Vector3 p3 = points[(i + 2) % count];

            for (int j = 0; j <= resolution; j++)
            {
                float t = j / (float)resolution;
                Vector3 point = GetCatmullRomPosition(t, p0, p1, p2, p3);
                curvePoints.Add(point);
            }
        }
        return curvePoints;
    }
    Vector3 GetCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        return 0.5f * (
            2 * p1 +
            (-p0 + p2) * t +
            (2 * p0 - 5 * p1 + 4 * p2 - p3) * t * t +
            (-p0 + 3 * p1 - 3 * p2 + p3) * t * t * t
        );
    }
    List<Vector3> FilterClosePoints(List<Vector3> points, float minDistance)
    {
        List<Vector3> filtered = new List<Vector3>();
        filtered.Add(points[0]);

        for (int i = 1; i < points.Count; i++)
        {
            if (Vector3.Distance(filtered[filtered.Count - 1], points[i]) > minDistance)
            {
                filtered.Add(points[i]);
            }
        }

        return filtered;
    }
    void PlaceTribune(List<Vector3> allobjects, List<Vector3> curvePoints)
    {
        float minX = curvePoints.Min(p => p.x);
        float maxX = curvePoints.Max(p => p.x);
        float minZ = curvePoints.Min(p => p.z);
        float maxZ = curvePoints.Max(p => p.z);

        float margin = 40f;
        float offsetRange = 30f;
        float minDistanceFromTrack = 15f;
        int maxTribunes = 4;
        int placed = 0;
        int tries = 0;

        Vector3[] basePositions = new Vector3[]
        {
        new Vector3((minX + maxX) / 2, 0, maxZ + margin),   // north
        new Vector3((minX + maxX) / 2, 0, minZ - margin),   // south
        new Vector3(minX - margin, 0, (minZ + maxZ) / 2),   // west
        new Vector3(maxX + margin, 0, (minZ + maxZ) / 2)    // east
        };

        Vector3[] faceDirections = new Vector3[]
        {
        Vector3.back,  // north faces south
        Vector3.forward,  // south faces north
        Vector3.right, // west faces east
        Vector3.left   // east faces west
        };

        for (int i = 0; i < 4 && placed < maxTribunes; i++)
        {
            float val = Random.value;
            for (int attempt = 0; attempt < 15; attempt++)
            {
                Vector3 offset = (i < 2)
                    ? new Vector3(Random.Range(-offsetRange, offsetRange), 0, 0) 
                    : new Vector3(0, 0, Random.Range(-offsetRange, offsetRange));

                Vector3 tryPos = basePositions[i] + offset;

                if (!IsTooCloseToTrack(tryPos, curvePoints, minDistanceFromTrack) && (val < 0.7f))
                {
                    Quaternion rot = Quaternion.LookRotation(faceDirections[i]);
                    Instantiate(tribune, tryPos + Vector3.up, rot);
                    allobjects.Add(tryPos + Vector3.up);
                    tribunes.Add(tribune);
                    placed++;
                    break;
                }
            }
        }
    }
    void PlaceTrees(List<Vector3> allObjects, List<Vector3> CurvePoints, float spacing = 5f)
    {
        int attempts = 0;
        int placed = 0;
        float minX = CurvePoints.Min(p => p.x) - 50;
        float maxX = CurvePoints.Max(p => p.x) + 50;
        float minZ = CurvePoints.Min(p => p.z) - 50;
        float maxZ = CurvePoints.Max(p => p.z) + 50;

        while (placed < treenumber && attempts < treenumber * 10)
        {
            Vector3 pos = new Vector3(
                Random.Range(minX, maxX),
                1,
                Random.Range(minZ, maxZ)
            );
            bool tooCloseToObjects = allObjects.Any(o => Vector3.Distance(o, pos) < spacing);
            bool tooClose = IsTooCloseToTrack(pos, CurvePoints, 15f);
            bool insideTribune = tribunes.Any(t =>
            {
                var rend = t.GetComponent<Renderer>();
                if (rend == null) return false;
                return rend.bounds.Contains(pos);
            });
            if (!tooClose && !tooCloseToObjects && !insideTribune)
            {
                Instantiate(tree, pos, Quaternion.identity);
                allObjects.Add(pos); 
                placed++;
            }
            attempts++;
        }
    }
    bool IsTooCloseToTrack(Vector3 position, List<Vector3> curvePoints, float minDistance)
    {
        for (int i = 0; i < curvePoints.Count - 1; i++)
        {
            Vector3 a = curvePoints[i];
            Vector3 b = curvePoints[i + 1];

            Vector3 projected = ProjectPointOnSegment(position, a, b);
            float dist = Vector3.Distance(position, projected);
            if (dist < minDistance)
                return true;
        }
        return false;
    }
    Vector3 ProjectPointOnSegment(Vector3 p, Vector3 a, Vector3 b)
    {
        Vector3 ab = b - a;
        float t = Vector3.Dot(p - a, ab) / ab.sqrMagnitude;
        t = Mathf.Clamp01(t);
        return a + t * ab;
    }
    public class SceneData
    {
        public List<Vector3> pos;
        public TransformData startingLine;
        public List<TransformData> roads = new();
        public List<TransformData> tribunes = new();
        public List<Vector3> trees = new();
    }
    [System.Serializable]
    public class TransformData
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;

        public TransformData(Transform t)
        {
            position = t.position;
            rotation = t.rotation;
            scale = t.localScale;
        }
    }
    public void SaveScene()
    {
        string fileName = inputField.text.Trim();
        if(DataScript.createdtracks.Contains(fileName) || DataScript.generatedtracks.Contains(fileName))
        {
            error.text = "";
            error.text = "That name is already used!";
            return;
        }
        if (string.IsNullOrEmpty(fileName))
        {
            error.text = "";
            error.text += "Name can't be empty";
            return;
        }
        string folderPath = Path.Combine(Application.persistentDataPath, "GeneratedTracks");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        string path = Path.Combine(folderPath, fileName + ".json");
        int counter = 1;
        while (File.Exists(path))
        {
            path = Path.Combine(folderPath, fileName + ".json");
            counter++;
        }
        SceneData data = new SceneData
        {
            pos = pos.ToList()
        };
        GameObject startingLine = GameObject.FindGameObjectWithTag("StartingLine");
        if (startingLine != null)
            data.startingLine = new TransformData(startingLine.transform);

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Road"))
            data.roads.Add(new TransformData(obj.transform));

        foreach (GameObject tribuneObj in GameObject.FindGameObjectsWithTag("Tribune"))
            data.tribunes.Add(new TransformData(tribuneObj.transform));

        foreach (GameObject treeObj in GameObject.FindGameObjectsWithTag("Tree"))
            data.trees.Add(treeObj.transform.position);

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);

        Debug.Log("Saved track to: " + path);
        SceneManager.LoadScene("Menu");
    }
}