using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    RaceScript raceScript;
    // Start is called before the first frame update
    void Start()
    {
        raceScript = GameObject.FindGameObjectWithTag("Canvas").GetComponent<RaceScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerExit(Collider other)
    {
        Debug.Log(other.gameObject.name);
        raceScript.increaseLaps(other.gameObject.name);
    }
}
