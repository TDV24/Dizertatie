using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    bool scriptFound = false;
    RaceScript raceScript;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!scriptFound && GameObject.FindGameObjectWithTag("Canvas").GetComponent<RaceScript>() != null)
        { 
            raceScript = GameObject.FindGameObjectWithTag("Canvas").GetComponent<RaceScript>();
            scriptFound = true; 
        }
    }
    private void OnTriggerExit(Collider other)
    {
        raceScript.increaseLaps(other.gameObject);
    }
}
