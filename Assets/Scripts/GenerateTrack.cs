using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GenerateTrack : MonoBehaviour
{
    public GameObject corner;
    int noOfCorners;
    int width;
    int length;
    private Vector3[] pos;
    private LineRenderer lineRenderer;
    // Start is called before the first frame update
    void Start()
    {
        width = PlayerPrefs.GetInt("TrackWidth");
        length = PlayerPrefs.GetInt("TrackLength");
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        noOfCorners = PlayerPrefs.GetInt("noOfCorners");
        pos = new Vector3[noOfCorners];
        for(int i = 0; i < noOfCorners; i++)
        {
            pos[i] = new Vector3(Random.Range(0, length), 1, Random.Range(0, width));
            Instantiate(corner, pos[i], Quaternion.identity);
        }
        for(int i = 0; i < noOfCorners; ++i)
        {
            Debug.Log(pos[i]);
        }
        lineRenderer.positionCount = noOfCorners + 1;
        lineRenderer.loop = true;
        pos = PointsSorting(pos);
        for(int i = 0; i < pos.Length; i++) 
        {
            lineRenderer.SetPosition(i, pos[i]);
        }
        lineRenderer.SetPosition(pos.Length, pos[0]);
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
        Vector3 centroid = new Vector3(length/2, 1, width/2);
        foreach (Vector3 pos in positions) 
        { 
            centroid += pos;
        }
        centroid /= positions.Length;

        return positions.OrderBy(p => Mathf.Atan2(p.z - centroid.z, p.x - centroid.x)).ToArray();
    }
}
