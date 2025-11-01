using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class VisiblityManager : MonoBehaviour
{
    public List<GameObject> objects_to_toggle;
    public List<GameObject> visual_boxes;
    string find_tag = "VisualTargets";

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

        // Find all objects with the tag find_tag
        GameObject[] found_boxes = GameObject.FindGameObjectsWithTag(find_tag);

        // Loop through each object that was found and add to visual_boxes array if not already present
        foreach (GameObject box in found_boxes)
        {
                        if (!visual_boxes.Contains(box))
            {
                visual_boxes.Add(box);
            }
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.B))
        {
            ToggleVisibility(visual_boxes.ToArray(), "Operand A", "Operand B", "Operator");
        }

        if (OVRInput.GetDown(OVRInput.RawButton.Y))
        {
            ToggleVisibility(visual_boxes.ToArray(), "Operand C");
        }

        if (OVRInput.GetDown(OVRInput.RawButton.X))
        {
            ToggleVisibility(objects_to_toggle.ToArray());
        }
    }

    public void ToggleVisibility(GameObject[] objectsArray, params string[] groupNames)
    {
        if (objectsArray == null) return;

        foreach (GameObject obj in objectsArray)
        {
            if (obj == null) continue;

            // If no group names specified, toggle all objects in the array
            if (groupNames.Length == 0)
            {
                obj.SetActive(!obj.activeSelf);
            }
            else
            {
                // Only toggle if the object name contains any of the specified group names
                foreach (string groupName in groupNames)
                {
                    if (obj.name.Contains(groupName))
                    {
                        obj.SetActive(!obj.activeSelf);
                        break; // Prevent toggling the same object multiple times
                    }
                }
            }
        }
    }
}
