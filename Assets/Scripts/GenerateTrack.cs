using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GenerateTrack : MonoBehaviour
{
    public GameObject corner;
    public GameObject roadPrefab;
    public GameObject startingLinePrefab;
    int noOfCorners;
    int width;
    int length;
    int maxIndex = 0;
    float maxDistance = 0f;
    private Vector3[] pos;
    // Start is called before the first frame update
    void Start()
    {
        width = PlayerPrefs.GetInt("TrackWidth");
        length = PlayerPrefs.GetInt("TrackLength");
        noOfCorners = PlayerPrefs.GetInt("noOfCorners");
        pos = new Vector3[noOfCorners];
        float radius = length / 2f;
        for (int i = 0; i < noOfCorners; i++)
        {
            float angle = (360f / noOfCorners) * i;
            float rad = Mathf.Deg2Rad * angle;
            float noise = Random.Range(-radius * 0.3f, radius * 0.3f); // +/- 30% noise
            float r = radius + noise;
            pos[i] = new Vector3(
                500 + Mathf.Cos(rad) * r,
                1,
                500 + Mathf.Sin(rad) * r
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
        Vector3 start = pos[maxIndex];
        Vector3 end = pos[maxIndex + 1];

        Vector3 direction = end - start;
        Vector3 midPoint = start + direction / 2f;
        startingLinePrefab.transform.localScale = new Vector3(startingLinePrefab.transform.localScale.x, 
            startingLinePrefab.transform.localScale.y, Vector3.Distance(start, end));
        Instantiate(startingLinePrefab, midPoint, Quaternion.LookRotation(direction));
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
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z)) 
        {
            SceneManager.LoadScene("Menu");
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
}
