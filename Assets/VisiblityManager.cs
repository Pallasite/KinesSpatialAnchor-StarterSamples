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
        // Find all objects with the tag SetupViz and add them to the list
        objects_to_toggle = new List<GameObject>(GameObject.FindGameObjectsWithTag("SetupViz"));
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
