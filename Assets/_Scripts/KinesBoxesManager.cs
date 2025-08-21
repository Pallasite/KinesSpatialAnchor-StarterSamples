using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class KinesBoxesManager : MonoBehaviour
{
    public GameObject[] visual_boxes;

    public GameObject operand_a_box;
    public GameObject operand_b_box;
    public GameObject operator_box;

    public GameObject user_camera;

    public float trial_advance_distance = 0.5f;

    [SerializeField]
    string find_tag = "VisualTargets";

    //int operator_a = 0;
    //int operand = 1;
    //int operator_b = 2;

    string[] csv_trial_data;
    int stroop_index = 0;
    int math_index = 0;

    bool has_advanced = false;

    public float height_set_timer = 5f;

    private void Update()
    {
        if(operand_a_box == null)
        {
            FindBoxes();
        }

    }

    private void FindBoxes()
    {
        visual_boxes = GameObject.FindGameObjectsWithTag(find_tag);

        foreach (GameObject box in visual_boxes)
        {
            if (box.name.Contains("Operand A"))
            {
                operand_a_box = box;
            }
            else if (box.name.Contains("Operand B"))
            {
                operand_b_box = box;
            }
            else if (box.name.Contains("Operator"))
            {
                operator_box = box;
            }
        }
    }

    public void StartTrials()
    {
        InvokeRepeating(nameof(TrialAdvanceDetector), 1f, 0.1f);
    }

    public void StopTrials()
    {
        CancelInvoke(nameof(TrialAdvanceDetector));
    }


    // Trial advance detection. When the user camera passes beyond 50 cm past the z position of the operator box, the trial advances  
    void TrialAdvanceDetector()
    {
        if (!has_advanced)
        {
            // Determine if the user is facing z positive  
            bool isFacingZPositive = user_camera.transform.eulerAngles.y > 270 || user_camera.transform.eulerAngles.y < 90;

            //float direction = (user_camera.transform.position.z > operator_box.transform.position.z) ? -1 : 1;

            if (isFacingZPositive)
            {
                if (user_camera.transform.position.z > operand_b_box.transform.position.z + trial_advance_distance)
                {
                    //LoadNextStroop();
                    LoadNextMath();
                    has_advanced = true;
                }
            }
            else if (user_camera.transform.eulerAngles.y > 90 && user_camera.transform.eulerAngles.y < 270)
            {
                if (user_camera.transform.position.z < operand_b_box.transform.position.z - trial_advance_distance)
                {
                    //LoadNextStroop();
                    LoadNextMath();
                    has_advanced = true;
                }
            }
        }
    }

    [ContextMenu("User Height")]

    public void IvokeUserHeight()
    {
        Invoke(nameof(SetToUserHeight), height_set_timer);
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

        foreach (GameObject box in visual_boxes)
        {
            box.transform.position = new Vector3(box.transform.position.x, eye_level.y, box.transform.position.z);
            box.transform.eulerAngles = new Vector3(0, box.transform.eulerAngles.y, 0);
        }

    }

    // swap the operator boxes based on which direction the user is facing at the start of the trial
    void FlipOperands()
    {
        GameObject temp = operand_a_box;
        operand_a_box = operand_b_box;
        operand_b_box = temp;
    }

    // load a list of csv values from local application persistant data path file for three text mesh pro objects on the visual boxes
    [ContextMenu("Load Stroop")]
    public void LoadCSVDataStroop()
    {
        csv_trial_data = System.IO.File.ReadAllLines(Application.persistentDataPath + "/stroop_problems.csv");
        stroop_index = 0;
        LoadNextStroop();
    }

    //Directionality will matter I think! For things like subtraction order of operation will be messed up
    //we will have to consider swapping the boxes or changing the logic.
    public void LoadNextStroop()
    {
        string[] line_data;

        line_data = csv_trial_data[stroop_index].Split(',');

        operand_a_box.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = line_data[0];
        operand_a_box.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = new Color(float.Parse(line_data[1]), float.Parse(line_data[2]), float.Parse(line_data[3]), 1);
        operator_box.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = line_data[4];
        operator_box.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = new Color(float.Parse(line_data[5]), float.Parse(line_data[6]), float.Parse(line_data[7]), 1);
        operand_b_box.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = line_data[8];
        operand_b_box.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = new Color(float.Parse(line_data[9]), float.Parse(line_data[10]), float.Parse(line_data[11]), 1);
        
        stroop_index++;

        has_advanced = false;
    }

    public void LoadPreviousStroop()
    {
        string[] line_data;

        stroop_index -= 1;

        if (stroop_index < 0) stroop_index = 0;

        line_data = csv_trial_data[stroop_index].Split(',');
        operand_a_box.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = line_data[0];
        operand_a_box.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = new Color(float.Parse(line_data[1]), float.Parse(line_data[2]), float.Parse(line_data[3]), 1);
        operator_box.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = line_data[4];
        operator_box.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = new Color(float.Parse(line_data[5]), float.Parse(line_data[6]), float.Parse(line_data[7]), 1);
        operand_b_box.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = line_data[8];
        operand_b_box.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = new Color(float.Parse(line_data[9]), float.Parse(line_data[10]), float.Parse(line_data[11]), 1);
        stroop_index++;

        has_advanced = false;

    }

    // load a list of numerical csv values from local application persistant data path file for three text mesh pro objects on the visual boxes
    [ContextMenu("Load Math")]
    public void LoadCSVDataMath()
    {
        csv_trial_data = System.IO.File.ReadAllLines(Application.persistentDataPath + "/math_problems.csv");
        math_index = 0;
        LoadNextMath();       
    }

    public void LoadNextMath()
    {
        string[] line_data;

        line_data = csv_trial_data[math_index].Split(',');
        operand_a_box.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = line_data[0];
        operator_box.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = line_data[1];
        operand_b_box.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = line_data[2];

        math_index++;
        has_advanced = false;

    }

    public void LoadPreviousMath()
    {
        string[] line_data;

        math_index -= 1;

        if (math_index < 0) math_index = 0;

        line_data = csv_trial_data[math_index].Split(',');
        operand_a_box.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = line_data[0];
        operator_box.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = line_data[1];
        operand_b_box.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = line_data[2];

        math_index++;
        has_advanced = false;

    }
}
