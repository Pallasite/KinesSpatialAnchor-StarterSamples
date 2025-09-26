using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class VisiblityManager : MonoBehaviour
{
    public List<GameObject> objects_to_toggle;

    // Start is called before the first frame update
    void Start()
    {
        objects_to_toggle = new List<GameObject>(GameObject.FindGameObjectsWithTag("SetupViz"));

        InvokeRepeating("FindObjects", 1.0f, 1.0f);
    }

    public void FindObjects()
    {
        // Find all objects with the tag "SetupViz"
        GameObject[] found_objects = GameObject.FindGameObjectsWithTag("SetupViz");

        // Loop through each object that was found
        foreach (GameObject obj in found_objects)
        {
            // If the list does NOT already contain the object, add it
            if (!objects_to_toggle.Contains(obj))
            {
                objects_to_toggle.Add(obj);
            }
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.B))
        {
            ToggleVisiblity();
        }
    }

    public void ToggleVisiblity()
    {
        foreach (GameObject obj in objects_to_toggle)
        {
            obj.SetActive(!obj.activeSelf);
        }
    }
}
