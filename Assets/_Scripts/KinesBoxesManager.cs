using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class KinesBoxesManager : MonoBehaviour
{
    public GameObject[] visual_boxes;

    [SerializeField]
    string find_tag = "VisualTargets";

    int operator_a = 0;
    int operand = 1;
    int operator_b = 2;
    [ContextMenu("User Height")]

    public void IvokeUserHeight()
    {
        Invoke(nameof(SetToUserHeight), 3f);
    }

    [ContextMenu("Save/Load Trials")]
    public void SaveAndLoadTrials()
    {
        TextAsset stroop = Resources.Load<TextAsset>("_Data/stroop_problems");
        //TextAsset math = Resources.Load("_Data/math_problems.csv") as TextAsset;
        string path = Application.persistentDataPath + "/stroop_problems.csv";
        File.WriteAllText(path, stroop.text);
    }

    void SetToUserHeight()
    {
        Vector3 eye_level = GameObject.Find("CenterEyeAnchor").transform.position;

        visual_boxes = GameObject.FindGameObjectsWithTag(find_tag);

        foreach (GameObject box in visual_boxes)
        {
            box.transform.position = new Vector3(box.transform.position.x, eye_level.y, box.transform.position.z);
            box.transform.eulerAngles = new Vector3(0, box.transform.eulerAngles.y, 0);
        }

    }

    // load a list of csv values from local application persistant data path file for three text mesh pro objects on the visual boxes
    [ContextMenu("Load Stroop")]
    public void LoadCSVDataStroop()
    {
        string[] csv_data = System.IO.File.ReadAllLines(Application.persistentDataPath + "/stroop_problems.csv");
        string[] line_data;
        for (int i = 0; i < csv_data.Length; i++)
        {
            line_data = csv_data[i].Split(',');
            visual_boxes[operator_a].GetComponentInChildren<TMPro.TextMeshProUGUI>().text = line_data[0];
            visual_boxes[operator_a].GetComponentInChildren<TMPro.TextMeshProUGUI>().color = new Color(float.Parse(line_data[1]), float.Parse(line_data[2]), float.Parse(line_data[3]), 1);
            visual_boxes[operand].GetComponentInChildren<TMPro.TextMeshProUGUI>().text = line_data[4];
            visual_boxes[operand].GetComponentInChildren<TMPro.TextMeshProUGUI>().color = new Color(float.Parse(line_data[5]), float.Parse(line_data[6]), float.Parse(line_data[7]), 1);
            visual_boxes[operator_b].GetComponentInChildren<TMPro.TextMeshProUGUI>().text = line_data[8];
            visual_boxes[operator_b].GetComponentInChildren<TMPro.TextMeshProUGUI>().color = new Color(float.Parse(line_data[9]), float.Parse(line_data[10]), float.Parse(line_data[11]), 1);
        }
    }

    // load a list of numerical csv values from local application persistant data path file for three text mesh pro objects on the visual boxes
    [ContextMenu("Load Math")]
    public void LoadCSVDataMath()
    {
        string[] csv_data = System.IO.File.ReadAllLines(Application.persistentDataPath + "/math_problems.csv");
        string[] line_data;
        for (int i = 0; i < csv_data.Length; i++)
        {
            line_data = csv_data[i].Split(',');
            visual_boxes[operator_a].GetComponentInChildren<TMPro.TextMeshProUGUI>().text = line_data[0];
            visual_boxes[operand].GetComponentInChildren<TMPro.TextMeshProUGUI>().text = line_data[1];
            visual_boxes[operator_b].GetComponentInChildren<TMPro.TextMeshProUGUI>().text = line_data[2];
        }
    }

    // read the next a single line of numerical csv values from local application persistant data path file each time this is called
    public void LoadNextCSVDataMath()
    {
        string[] csv_data = System.IO.File.ReadAllLines(Application.persistentDataPath + "/math_problems.csv");
        string[] line_data;
        for (int i = 0; i < csv_data.Length; i++)
        {
            line_data = csv_data[i].Split(',');
            visual_boxes[operator_a].GetComponentInChildren<TMPro.TextMeshProUGUI>().text = line_data[0];
            visual_boxes[operand].GetComponentInChildren<TMPro.TextMeshProUGUI>().text = line_data[1];
            visual_boxes[operator_b].GetComponentInChildren<TMPro.TextMeshProUGUI>().text = line_data[2];
        }
    }
}
