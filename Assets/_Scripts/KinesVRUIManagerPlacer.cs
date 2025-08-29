using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KinesVRUIManagerPlacer : MonoBehaviour
{
    public GameObject canvasPrefab;
    public GameObject buttonPrefab;
    public GameObject labelPrefab;

    void Start()
    {
        if (canvasPrefab == null || buttonPrefab == null || labelPrefab == null)
        {
            Debug.LogError("Prefabs not assigned in KinesVRUIManagerPlacer. UI will not be created.");
            return;
        }

        // --- 1. Create Canvas ---
        GameObject canvasGO = Instantiate(canvasPrefab);
        canvasGO.name = "KinesVRUIPanel";

        // Add VRUIManager component
        VRUIManager uiManager = canvasGO.AddComponent<VRUIManager>();

        // --- 2. Create Text Fields ---
        // Trial Text
        GameObject trialTextGO = Instantiate(labelPrefab, canvasGO.transform);
        trialTextGO.name = "TrialText";
        uiManager.currentTrialText = trialTextGO.GetComponentInChildren<TextMeshProUGUI>();
        if (uiManager.currentTrialText != null)
        {
            uiManager.currentTrialText.text = "Trial: 0";
        }
        trialTextGO.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 100);

        // Log Text
        GameObject logTextGO = Instantiate(labelPrefab, canvasGO.transform);
        logTextGO.name = "LogText";
        uiManager.diagnosticLogText = logTextGO.GetComponentInChildren<TextMeshProUGUI>();
        if (uiManager.diagnosticLogText != null)
        {
            uiManager.diagnosticLogText.text = "Logs will appear here...";
        }
        logTextGO.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -50);
        logTextGO.GetComponent<RectTransform>().sizeDelta = new Vector2(400, 200);


        // --- 3. Create Buttons ---
        // Next Math Button
        GameObject nextMathBtnGO = Instantiate(buttonPrefab, canvasGO.transform);
        nextMathBtnGO.name = "NextMathButton";
        nextMathBtnGO.GetComponentInChildren<TextMeshProUGUI>().text = "Next Math";
        nextMathBtnGO.GetComponent<Button>().onClick.AddListener(uiManager.LoadNextMath);
        nextMathBtnGO.GetComponent<RectTransform>().anchoredPosition = new Vector2(100, 50);

        // Previous Math Button
        GameObject prevMathBtnGO = Instantiate(buttonPrefab, canvasGO.transform);
        prevMathBtnGO.name = "PreviousMathButton";
        prevMathBtnGO.GetComponentInChildren<TextMeshProUGUI>().text = "Previous Math";
        prevMathBtnGO.GetComponent<Button>().onClick.AddListener(uiManager.LoadPreviousMath);
        prevMathBtnGO.GetComponent<RectTransform>().anchoredPosition = new Vector2(-100, 50);

        // Next Stroop Button
        GameObject nextStroopBtnGO = Instantiate(buttonPrefab, canvasGO.transform);
        nextStroopBtnGO.name = "NextStroopButton";
        nextStroopBtnGO.GetComponentInChildren<TextMeshProUGUI>().text = "Next Stroop";
        nextStroopBtnGO.GetComponent<Button>().onClick.AddListener(uiManager.LoadNextStroop);
        nextStroopBtnGO.GetComponent<RectTransform>().anchoredPosition = new Vector2(100, 0);

        // Previous Stroop Button
        GameObject prevStroopBtnGO = Instantiate(buttonPrefab, canvasGO.transform);
        prevStroopBtnGO.name = "PreviousStroopButton";
        prevStroopBtnGO.GetComponentInChildren<TextMeshProUGUI>().text = "Previous Stroop";
        prevStroopBtnGO.GetComponent<Button>().onClick.AddListener(uiManager.LoadPreviousStroop);
        prevStroopBtnGO.GetComponent<RectTransform>().anchoredPosition = new Vector2(-100, 0);
    }
}
