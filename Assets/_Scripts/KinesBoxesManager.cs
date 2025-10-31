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

    public GameObject operand_c_box;

    // Cached text components for better performance
    private TMPro.TextMeshProUGUI operand_a_text;
    private TMPro.TextMeshProUGUI operand_b_text;
    private TMPro.TextMeshProUGUI operand_c_text;

    private TMPro.TextMeshProUGUI operator_text;

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


    public float height_set_timer = 5f;

    // FSM states for user position relative to operator box
    private enum UserZoneState
    {
        Unknown,
        NegativeZone,
        PositiveZone
    }

    private UserZoneState user_zone_state = UserZoneState.Unknown;

    private void Start()
    {
        SetToUserHeight();
        
    }

    private void Update()
    {
        if (operand_a_box == null)
        {
            FindBoxes();
            //LoadCSVDataMath();
            //StartTrials();
        }
    }

    [ContextMenu("Initialize Test")]
    public void Initialize()
    {
        FindBoxes();
        LoadCSVDataMath();
        StartTrials();
    }

    private void FindBoxes()
    {
        visual_boxes = GameObject.FindGameObjectsWithTag(find_tag);

        foreach (GameObject box in visual_boxes)
        {
            if (box.name.Contains("Operand A"))
            {
                operand_a_box = box;
                operand_a_text = box.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            }
            else if (box.name.Contains("Operand B"))
            {
                operand_b_box = box;
                operand_b_text = box.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            }
            else if (box.name.Contains("Operator"))
            {
                operator_box = box;
                operator_text = box.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            }
            else if (box.name.Contains("Operand C"))
            {
                operand_c_box = box;
                operand_c_text = box.GetComponentInChildren<TMPro.TextMeshProUGUI>();
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


    // Trial advance detection using FSM
    void TrialAdvanceDetector()
    {
        float user_z = user_camera.transform.position.z;
        float operator_box_z = operator_box.transform.position.z;

        // Determine if the user is facing z positive
        //bool is_facing_positive_z = user_camera.transform.eulerAngles.y > 270 || user_camera.transform.eulerAngles.y < 90;
        bool is_facing_positive_z = Vector3.Dot(user_camera.transform.forward, Vector3.forward) > 0;

        Debug.Log($"[TrialAdvanceDetector] user_z: {user_z}, operator_box_z: {operator_box_z}, is_facing_positive_z: {is_facing_positive_z}, user_zone_state: {user_zone_state}");

        switch (user_zone_state)
        {
            case UserZoneState.Unknown:
                // Initialize state based on current position
                if (user_z > operator_box_z)
                {
                    user_zone_state = UserZoneState.PositiveZone;
                    Debug.Log("[TrialAdvanceDetector] Initialized to PositiveZone");
                }
                else
                {
                    user_zone_state = UserZoneState.NegativeZone;
                    Debug.Log("[TrialAdvanceDetector] Initialized to NegativeZone");
                }
                break;

            case UserZoneState.NegativeZone:
                if (is_facing_positive_z && user_z > operator_box_z + trial_advance_distance)
                {
                    // User entered positive zone from negative
                    Debug.Log("[TrialAdvanceDetector] Transition: NegativeZone -> PositiveZone. Advancing trial.");
                    LoadNextMath();
                    user_zone_state = UserZoneState.PositiveZone;
                }
                break;

            case UserZoneState.PositiveZone:
                if (!is_facing_positive_z && user_z < operator_box_z - trial_advance_distance)
                {
                    // User entered negative zone from positive
                    Debug.Log("[TrialAdvanceDetector] Transition: PositiveZone -> NegativeZone. Advancing trial.");
                    LoadNextMath();
                    user_zone_state = UserZoneState.NegativeZone;
                }
                break;
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
        if (stroop_index < csv_trial_data.Length)
        {
            SetStroopTrial(stroop_index);
            stroop_index++;
        }
        else
        {
            Debug.LogWarning("End of stroop trials reached.");
        }
    }

    public void LoadPreviousStroop()
    {
        stroop_index--;
        if (stroop_index < 0) stroop_index = 0;
        
        SetStroopTrial(stroop_index);
        stroop_index++; // Increment so next LoadNext() works correctly
    }

    private void SetStroopTrial(int index)
    {
        string[] line_data = csv_trial_data[index].Split(',');

        operand_a_text.text = line_data[0];
        operand_a_text.color = new Color(float.Parse(line_data[1]), float.Parse(line_data[2]), float.Parse(line_data[3]), 1);
        operator_text.text = line_data[4];
        operator_text.color = new Color(float.Parse(line_data[5]), float.Parse(line_data[6]), float.Parse(line_data[7]), 1);
        operand_b_text.text = line_data[8];
        operand_b_text.color = new Color(float.Parse(line_data[9]), float.Parse(line_data[10]), float.Parse(line_data[11]), 1);
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
        if (math_index < csv_trial_data.Length)
        {
            SetMathTrial(math_index);
            math_index++;
        }
        else
        {
            Debug.LogWarning("End of math trials reached.");
        }
    }

    public void LoadPreviousMath()
    {
        math_index--;
        if (math_index < 0) math_index = 0;
        
        SetMathTrial(math_index);
        math_index++; // Increment so next LoadNext() works correctly
    }

    private void SetMathTrial(int index)
    {
        string[] line_data = csv_trial_data[index].Split(',');
        operand_a_text.text = line_data[0];
        operator_text.text = line_data[1];
        operand_b_text.text = line_data[2];

        // Set operand_c_text to a string composed of operand_a, operator, and operand_b separated by a space  
        operand_c_text.text = $"{operand_a_text.text}{operator_text.text}{operand_b_text.text}";
    }
}
