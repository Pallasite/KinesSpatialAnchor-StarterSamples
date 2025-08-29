using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class KinesBoxesManager : MonoBehaviour
{
    [Header("Visual Boxes")]
    public GameObject operand_a_box;
    public GameObject operand_b_box;
    public GameObject operator_box;
    
    [Header("UI References")]
    public TMPro.TextMeshProUGUI operand_a_text;
    public TMPro.TextMeshProUGUI operand_b_text;
    public TMPro.TextMeshProUGUI operator_text;
    
    [Header("Trial Settings")]
    public float trial_advance_distance = 1.0f;
    public Camera user_camera;
    public string find_tag = "TrialBox";
    
    private GameObject[] visual_boxes;
    private string[] csv_trial_data;
    private int math_index = 0;
    private int stroop_index = 0;
    
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
        if (user_camera == null || operator_box == null) return;
        
        float user_z = user_camera.transform.position.z;
        float operator_box_z = operator_box.transform.position.z;

        // Determine if the user is facing z positive
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
        SetToUserHeight();
    }

    [ContextMenu("Save/Load Trials")]
    public void SaveAndLoadTrials()
    {
        // Implementation for saving/loading trials
    }

    void SetToUserHeight()
    {
        if (user_camera != null)
        {
            // Adjust trial boxes to user height if needed
            float userHeight = user_camera.transform.position.y;
            Debug.Log($"User height: {userHeight}");
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
        string filePath = Application.persistentDataPath + "/stroop_problems.csv";
        if (File.Exists(filePath))
        {
            csv_trial_data = File.ReadAllLines(filePath);
            stroop_index = 0;
            LoadNextStroop();
        }
        else
        {
            Debug.LogWarning("Stroop CSV file not found: " + filePath);
        }
    }

    public void LoadNextStroop()
    {
        if (csv_trial_data != null && stroop_index < csv_trial_data.Length)
        {
            SetStroopTrial(stroop_index);
            stroop_index++;
        }
        else
        {
            Debug.LogWarning("End of stroop trials reached or data not loaded.");
        }
    }

    public void LoadPreviousStroop()
    {
        stroop_index--;
        if (stroop_index < 0) stroop_index = 0;
        
        if (csv_trial_data != null)
        {
            SetStroopTrial(stroop_index);
            stroop_index++; // Increment so next LoadNext() works correctly
        }
    }

    private void SetStroopTrial(int index)
    {
        if (csv_trial_data != null && index < csv_trial_data.Length)
        {
            string[] line_data = csv_trial_data[index].Split(',');
            if (line_data.Length >= 3)
            {
                if (operand_a_text != null) operand_a_text.text = line_data[0];
                if (operator_text != null) operator_text.text = line_data[1];
                if (operand_b_text != null) operand_b_text.text = line_data[2];
            }
        }
    }

    // load a list of numerical csv values from local application persistant data path file for three text mesh pro objects on the visual boxes
    [ContextMenu("Load Math")]
    public void LoadCSVDataMath()
    {
        string filePath = Application.persistentDataPath + "/math_problems.csv";
        if (File.Exists(filePath))
        {
            csv_trial_data = File.ReadAllLines(filePath);
            math_index = 0;
            LoadNextMath();
        }
        else
        {
            Debug.LogWarning("Math CSV file not found: " + filePath + ". Creating sample data.");
            CreateSampleMathData();
        }
    }

    private void CreateSampleMathData()
    {
        // Create some sample math problems for testing
        List<string> sampleData = new List<string>
        {
            "2,+,3",
            "5,-,1",
            "4,*,2", 
            "8,/,2",
            "3,+,7",
            "9,-,4",
            "6,*,3",
            "12,/,4"
        };
        
        csv_trial_data = sampleData.ToArray();
        math_index = 0;
        LoadNextMath();
        Debug.Log("Created sample math data with " + csv_trial_data.Length + " problems");
    }

    public void LoadNextMath()
    {
        if (csv_trial_data != null && math_index < csv_trial_data.Length)
        {
            SetMathTrial(math_index);
            math_index++;
            Debug.Log($"Loaded math trial {math_index}/{csv_trial_data.Length}");
        }
        else
        {
            Debug.LogWarning("End of math trials reached or data not loaded.");
        }
    }

    public void LoadPreviousMath()
    {
        math_index--;
        if (math_index < 0) math_index = 0;
        
        if (csv_trial_data != null)
        {
            SetMathTrial(math_index);
            math_index++; // Increment so next LoadNext() works correctly
            Debug.Log($"Loaded previous math trial {math_index}/{csv_trial_data.Length}");
        }
    }

    private void SetMathTrial(int index)
    {
        if (csv_trial_data != null && index < csv_trial_data.Length)
        {
            string[] line_data = csv_trial_data[index].Split(',');
            if (line_data.Length >= 3)
            {
                if (operand_a_text != null) operand_a_text.text = line_data[0];
                if (operator_text != null) operator_text.text = line_data[1];
                if (operand_b_text != null) operand_b_text.text = line_data[2];
            }
        }
    }

    public int GetCurrentTrialIndex()
    {
        return math_index;
    }

    public int GetTotalTrials()
    {
        return csv_trial_data?.Length ?? 0;
    }
}