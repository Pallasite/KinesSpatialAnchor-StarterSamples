using UnityEngine;
using TMPro;

public class VRUIManager : MonoBehaviour
{
    public KinesBoxesManager kinesBoxesManager;
    public TextMeshProUGUI currentTrialText;
    public TextMeshProUGUI diagnosticLogText;

    private void Start()
    {
        if (kinesBoxesManager == null)
        {
            kinesBoxesManager = FindObjectOfType<KinesBoxesManager>();
        }

        Application.logMessageReceived += HandleLog;
    }

    private void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (diagnosticLogText.text.Split('\n').Length > 20)
        {
            diagnosticLogText.text = "";
        }
        diagnosticLogText.text += logString + "\n";
    }

    void Update()
    {
        if (kinesBoxesManager != null && currentTrialText != null)
        {
            string mathTrialText;
            if (kinesBoxesManager.math_index > 0)
            {
                mathTrialText = "Math Trial: " + kinesBoxesManager.math_index;
            }
            else
            {
                mathTrialText = "Math Trial: (Not Started)";
            }

            string stroopTrialText;
            if (kinesBoxesManager.stroop_index > 0)
            {
                stroopTrialText = "Stroop Trial: " + kinesBoxesManager.stroop_index;
            }
            else
            {
                stroopTrialText = "Stroop Trial: (Not Started)";
            }

            currentTrialText.text = mathTrialText + "\n" + stroopTrialText;
        }
    }

    public void LoadNextMath()
    {
        if (kinesBoxesManager != null)
        {
            kinesBoxesManager.LoadNextMath();
        }
    }

    public void LoadPreviousMath()
    {
        if (kinesBoxesManager != null)
        {
            kinesBoxesManager.LoadPreviousMath();
        }
    }

    public void LoadNextStroop()
    {
        if (kinesBoxesManager != null)
        {
            kinesBoxesManager.LoadNextStroop();
        }
    }

    public void LoadPreviousStroop()
    {
        if (kinesBoxesManager != null)
        {
            kinesBoxesManager.LoadPreviousStroop();
        }
    }
}
