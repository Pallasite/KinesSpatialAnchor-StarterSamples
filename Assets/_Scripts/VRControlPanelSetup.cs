using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Setup script to create and configure the VR Trial Control Panel in the scene.
/// Add this script to any GameObject in the scene to automatically create the control panel.
/// </summary>
public class VRControlPanelSetup : MonoBehaviour
{
    [Header("Setup Configuration")]
    [SerializeField] private bool _createOnStart = true;
    [SerializeField] private bool _autoFindKinesManager = true;
    [SerializeField] private KinesBoxesManager _kinesManagerOverride;
    
    [Header("Panel Configuration")]
    [SerializeField] private Vector3 _panelPosition = new Vector3(0, 1.5f, 2f);
    [SerializeField] private Vector3 _panelRotation = new Vector3(0, 0, 0);
    [SerializeField] private Vector3 _panelScale = new Vector3(0.001f, 0.001f, 0.001f);
    
    private GameObject _controlPanel;
    
    private void Start()
    {
        if (_createOnStart)
        {
            CreateVRControlPanel();
        }
    }
    
    [ContextMenu("Create VR Control Panel")]
    public void CreateVRControlPanel()
    {
        if (_controlPanel != null)
        {
            Debug.LogWarning("VR Control Panel already exists! Destroying old one.");
            DestroyImmediate(_controlPanel);
        }
        
        // Create main panel GameObject
        _controlPanel = new GameObject("VRTrialControlPanel");
        _controlPanel.transform.position = _panelPosition;
        _controlPanel.transform.rotation = Quaternion.Euler(_panelRotation);
        
        // Add VR Control Panel script
        var panelScript = _controlPanel.AddComponent<VRTrialControlPanel>();
        
        // Add AudioSource for button sounds
        var audioSource = _controlPanel.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1.0f; // 3D sound
        audioSource.volume = 0.5f;
        audioSource.playOnAwake = false;
        
        // Find or assign KinesBoxesManager
        KinesBoxesManager kinesManager = _kinesManagerOverride;
        if (_autoFindKinesManager && kinesManager == null)
        {
            kinesManager = FindObjectOfType<KinesBoxesManager>();
        }
        
        // Create Canvas
        var canvas = CreateCanvas(_controlPanel.transform);
        
        // Create UI elements
        var statusText = CreateStatusText(canvas.transform);
        var logText = CreateLogText(canvas.transform);
        var buttonContainer = CreateButtonContainer(canvas.transform);
        
        // Create buttons
        CreateButton(buttonContainer, "Initialize", new Vector2(-300, 200), panelScript.OnInitializePressed);
        CreateButton(buttonContainer, "Start Trials", new Vector2(-100, 200), panelScript.OnStartTrialsPressed);
        CreateButton(buttonContainer, "Stop Trials", new Vector2(100, 200), panelScript.OnStopTrialsPressed);
        
        CreateButton(buttonContainer, "Prev Stroop", new Vector2(-300, 100), panelScript.OnPreviousStroopPressed);
        CreateButton(buttonContainer, "Next Stroop", new Vector2(-100, 100), panelScript.OnNextStroopPressed);
        
        CreateButton(buttonContainer, "Prev Math", new Vector2(-300, 0), panelScript.OnPreviousMathPressed);
        CreateButton(buttonContainer, "Next Math", new Vector2(-100, 0), panelScript.OnNextMathPressed);
        
        CreateButton(buttonContainer, "Clear Logs", new Vector2(100, 0), panelScript.ClearLogs);
        
        // Configure the panel script with references
        SetPanelReferences(panelScript, statusText, logText, kinesManager);
        
        Debug.Log($"VR Control Panel created successfully! KinesManager: {(kinesManager != null ? "Found" : "Not Found")}");
    }
    
    private Canvas CreateCanvas(Transform parent)
    {
        var canvasGO = new GameObject("Canvas");
        canvasGO.transform.SetParent(parent, false);
        canvasGO.layer = 5; // UI layer
        
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = null; // Will use main camera
        
        var canvasScaler = canvasGO.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
        canvasScaler.referencePixelsPerUnit = 100;
        canvasScaler.dynamicPixelsPerUnit = 1;
        
        var graphicRaycaster = canvasGO.AddComponent<GraphicRaycaster>();
        graphicRaycaster.ignoreReversedGraphics = true;
        
        var rectTransform = canvasGO.GetComponent<RectTransform>();
        rectTransform.localPosition = Vector3.zero;
        rectTransform.localRotation = Quaternion.identity;
        rectTransform.localScale = _panelScale;
        rectTransform.sizeDelta = new Vector2(800, 600);
        
        return canvas;
    }
    
    private TextMeshProUGUI CreateStatusText(Transform parent)
    {
        var textGO = new GameObject("StatusText");
        textGO.transform.SetParent(parent, false);
        
        var text = textGO.AddComponent<TextMeshProUGUI>();
        text.text = "VR Trial Control Panel\nInitializing...";
        text.fontSize = 16;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.TopLeft;
        text.enableWordWrapping = true;
        
        var rectTransform = textGO.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 0.5f);
        rectTransform.anchorMax = new Vector2(0.45f, 1f);
        rectTransform.anchoredPosition = new Vector2(0, 0);
        rectTransform.offsetMin = new Vector2(10, 10);
        rectTransform.offsetMax = new Vector2(-10, -10);
        
        // Add background
        var image = textGO.AddComponent<Image>();
        image.color = new Color(0, 0, 0, 0.8f);
        
        return text;
    }
    
    private TextMeshProUGUI CreateLogText(Transform parent)
    {
        var textGO = new GameObject("LogText");
        textGO.transform.SetParent(parent, false);
        
        var text = textGO.AddComponent<TextMeshProUGUI>();
        text.text = "Logs will appear here...";
        text.fontSize = 12;
        text.color = Color.yellow;
        text.alignment = TextAlignmentOptions.TopLeft;
        text.enableWordWrapping = true;
        
        var rectTransform = textGO.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.55f, 0f);
        rectTransform.anchorMax = new Vector2(1f, 1f);
        rectTransform.anchoredPosition = new Vector2(0, 0);
        rectTransform.offsetMin = new Vector2(10, 10);
        rectTransform.offsetMax = new Vector2(-10, -10);
        
        // Add background
        var image = textGO.AddComponent<Image>();
        image.color = new Color(0, 0, 0, 0.8f);
        
        return text;
    }
    
    private Transform CreateButtonContainer(Transform parent)
    {
        var containerGO = new GameObject("ButtonContainer");
        containerGO.transform.SetParent(parent, false);
        
        var rectTransform = containerGO.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            rectTransform = containerGO.AddComponent<RectTransform>();
        }
        
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0.45f, 0.45f);
        rectTransform.anchoredPosition = new Vector2(0, 0);
        rectTransform.offsetMin = new Vector2(10, 10);
        rectTransform.offsetMax = new Vector2(-10, -10);
        
        return containerGO.transform;
    }
    
    private Button CreateButton(Transform parent, string buttonText, Vector2 position, System.Action onClick)
    {
        var buttonGO = new GameObject($"Button_{buttonText.Replace(" ", "")}");
        buttonGO.transform.SetParent(parent, false);
        
        var button = buttonGO.AddComponent<Button>();
        var image = buttonGO.AddComponent<Image>();
        image.color = new Color(0.2f, 0.5f, 1f, 0.8f);
        
        // Create text child
        var textGO = new GameObject("Text");
        textGO.transform.SetParent(buttonGO.transform, false);
        
        var text = textGO.AddComponent<TextMeshProUGUI>();
        text.text = buttonText;
        text.fontSize = 14;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Center;
        
        // Configure button RectTransform
        var buttonRect = buttonGO.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.anchoredPosition = position;
        buttonRect.sizeDelta = new Vector2(180, 40);
        
        // Configure text RectTransform
        var textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.anchoredPosition = Vector2.zero;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        // Add VR interaction capability
        var vrButton = buttonGO.AddComponent<VRButton>();
        vrButton.SetUIButton(button);
        
        // Add click listener
        if (onClick != null)
        {
            button.onClick.AddListener(() => onClick());
        }
        
        // Add hover effects
        var colorBlock = button.colors;
        colorBlock.normalColor = new Color(0.2f, 0.5f, 1f, 0.8f);
        colorBlock.highlightedColor = new Color(0.3f, 0.6f, 1f, 1f);
        colorBlock.pressedColor = new Color(0.1f, 0.4f, 0.9f, 1f);
        colorBlock.selectedColor = new Color(0.25f, 0.55f, 1f, 0.9f);
        button.colors = colorBlock;
        
        return button;
    }
    
    private void SetPanelReferences(VRTrialControlPanel panelScript, TextMeshProUGUI statusText, TextMeshProUGUI logText, KinesBoxesManager kinesManager)
    {
        // Using reflection to set private fields since Unity serialization happens in editor
        var panelType = typeof(VRTrialControlPanel);
        
        var statusField = panelType.GetField("_statusText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (statusField != null)
            statusField.SetValue(panelScript, statusText);
        
        var logField = panelType.GetField("_logText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (logField != null)
            logField.SetValue(panelScript, logText);
        
        var kinesField = panelType.GetField("_kinesManager", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (kinesField != null)
            kinesField.SetValue(panelScript, kinesManager);
    }
    
    [ContextMenu("Destroy VR Control Panel")]
    public void DestroyVRControlPanel()
    {
        if (_controlPanel != null)
        {
            DestroyImmediate(_controlPanel);
            _controlPanel = null;
            Debug.Log("VR Control Panel destroyed.");
        }
    }
}