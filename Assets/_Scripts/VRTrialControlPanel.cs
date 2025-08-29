using System.Collections;
using System.Text;
using UnityEngine;
using TMPro;

/// <summary>
/// VR Control Panel for managing trial progression in the Kines Spatial Anchor project.
/// Provides buttons for Previous/Next trial navigation for both Stroop and Math trials,
/// and displays current trial information and Unity logs.
/// </summary>
public class VRTrialControlPanel : MonoBehaviour
{
    [Header("Panel Positioning")]
    [SerializeField] private float _maxDistance = 0.8f;
    [SerializeField] private float _minDistance = 0.3f;
    [SerializeField] private float _followSpeed = 2.0f;
    [SerializeField] private Vector3 _panelOffset = new Vector3(0, -0.2f, 0.6f);
    
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI _statusText;
    [SerializeField] private TextMeshProUGUI _logText;
    
    [Header("Manager Reference")]
    [SerializeField] private KinesBoxesManager _kinesManager;
    
    private OVRCameraRig _cameraRig;
    private Vector3 _targetPosition;
    private Quaternion _targetRotation;
    private bool _isInitialized = false;
    private StringBuilder _logBuilder = new StringBuilder();
    private int _maxLogLines = 10;
    
    // Follow the HMD pattern from PanelHMDFollower
    private const float HMD_MOVEMENT_THRESHOLD = 0.3f;
    private const float TOTAL_DURATION = 2.0f;
    private Vector3 _prevPos = Vector3.zero;
    private Vector3 _lastMovedToPos = Vector3.zero;
    private Coroutine _followCoroutine = null;
    
    private void Awake()
    {
        _cameraRig = FindObjectOfType<OVRCameraRig>();
        if (_cameraRig == null)
        {
            Debug.LogWarning("[VRTrialControlPanel] OVRCameraRig not found. Panel will not follow HMD.");
        }
        
        if (_kinesManager == null)
        {
            _kinesManager = FindObjectOfType<KinesBoxesManager>();
        }
        
        if (_kinesManager == null)
        {
            Debug.LogError("[VRTrialControlPanel] KinesBoxesManager not found!");
        }
        
        // Subscribe to Unity log events
        Application.logMessageReceived += HandleLog;
        
        _isInitialized = true;
    }
    
    private void Start()
    {
        UpdateStatusDisplay();
        
        // Initial positioning
        if (_cameraRig != null)
        {
            PositionPanel();
        }
    }
    
    private void Update()
    {
        if (!_isInitialized || _cameraRig == null) return;
        
        // Follow HMD logic similar to PanelHMDFollower
        var centerEyeAnchorPos = _cameraRig.centerEyeAnchor.position;
        float distanceFromLastMovement = Vector3.Distance(centerEyeAnchorPos, _lastMovedToPos);
        float headMovementSpeed = (_cameraRig.centerEyeAnchor.position - _prevPos).magnitude / Time.deltaTime;
        var currDiffFromCenterEye = transform.position - centerEyeAnchorPos;
        var currDistanceFromCenterEye = currDiffFromCenterEye.magnitude;
        
        // Check if we need to reposition the panel
        if (((distanceFromLastMovement > _maxDistance) || 
             (currDistanceFromCenterEye > _maxDistance) || 
             (currDistanceFromCenterEye < _minDistance)) &&
            headMovementSpeed < HMD_MOVEMENT_THRESHOLD && 
            _followCoroutine == null)
        {
            _followCoroutine = StartCoroutine(SmoothFollowHMD());
        }
        
        _prevPos = _cameraRig.centerEyeAnchor.position;
    }
    
    private void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
    }
    
    private void PositionPanel()
    {
        if (_cameraRig == null) return;
        
        var headTransform = _cameraRig.centerEyeAnchor;
        _targetPosition = headTransform.position + headTransform.TransformDirection(_panelOffset);
        _targetRotation = Quaternion.LookRotation(_targetPosition - headTransform.position, Vector3.up);
        
        transform.position = _targetPosition;
        transform.rotation = _targetRotation;
        
        _lastMovedToPos = headTransform.position;
    }
    
    private IEnumerator SmoothFollowHMD()
    {
        if (_cameraRig == null) yield break;
        
        var headTransform = _cameraRig.centerEyeAnchor;
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        
        _targetPosition = headTransform.position + headTransform.TransformDirection(_panelOffset);
        _targetRotation = Quaternion.LookRotation(_targetPosition - headTransform.position, Vector3.up);
        
        _lastMovedToPos = headTransform.position;
        
        float startTime = Time.time;
        float endTime = Time.time + TOTAL_DURATION;
        
        while (Time.time < endTime)
        {
            float t = (Time.time - startTime) / TOTAL_DURATION;
            transform.position = Vector3.Lerp(startPos, _targetPosition, t);
            transform.rotation = Quaternion.Lerp(startRot, _targetRotation, t);
            yield return null;
        }
        
        transform.position = _targetPosition;
        transform.rotation = _targetRotation;
        _followCoroutine = null;
    }
    
    // Button callback methods for UI buttons
    public void OnPreviousStroopPressed()
    {
        if (_kinesManager != null)
        {
            _kinesManager.LoadPreviousStroop();
            UpdateStatusDisplay();
            Debug.Log("[VRTrialControlPanel] Previous Stroop trial loaded");
        }
    }
    
    public void OnNextStroopPressed()
    {
        if (_kinesManager != null)
        {
            _kinesManager.LoadNextStroop();
            UpdateStatusDisplay();
            Debug.Log("[VRTrialControlPanel] Next Stroop trial loaded");
        }
    }
    
    public void OnPreviousMathPressed()
    {
        if (_kinesManager != null)
        {
            _kinesManager.LoadPreviousMath();
            UpdateStatusDisplay();
            Debug.Log("[VRTrialControlPanel] Previous Math trial loaded");
        }
    }
    
    public void OnNextMathPressed()
    {
        if (_kinesManager != null)
        {
            _kinesManager.LoadNextMath();
            UpdateStatusDisplay();
            Debug.Log("[VRTrialControlPanel] Next Math trial loaded");
        }
    }
    
    public void OnStartTrialsPressed()
    {
        if (_kinesManager != null)
        {
            _kinesManager.StartTrials();
            UpdateStatusDisplay();
            Debug.Log("[VRTrialControlPanel] Trials started");
        }
    }
    
    public void OnStopTrialsPressed()
    {
        if (_kinesManager != null)
        {
            _kinesManager.StopTrials();
            UpdateStatusDisplay();
            Debug.Log("[VRTrialControlPanel] Trials stopped");
        }
    }
    
    public void OnInitializePressed()
    {
        if (_kinesManager != null)
        {
            _kinesManager.Initialize();
            UpdateStatusDisplay();
            Debug.Log("[VRTrialControlPanel] System initialized");
        }
    }
    
    private void UpdateStatusDisplay()
    {
        if (_statusText == null || _kinesManager == null) return;
        
        StringBuilder status = new StringBuilder();
        status.AppendLine("=== VR Trial Control Panel ===");
        status.AppendLine($"Manager Found: {(_kinesManager != null ? "Yes" : "No")}");
        status.AppendLine($"Time: {System.DateTime.Now:HH:mm:ss}");
        status.AppendLine("");
        status.AppendLine("Use buttons below to control trials:");
        status.AppendLine("• Initialize: Setup system");
        status.AppendLine("• Start/Stop: Control trial detection");
        status.AppendLine("• Prev/Next: Manual trial navigation");
        
        _statusText.text = status.ToString();
    }
    
    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (_logText == null) return;
        
        // Add new log entry
        string logEntry = $"[{System.DateTime.Now:HH:mm:ss}] {type}: {logString}";
        _logBuilder.AppendLine(logEntry);
        
        // Keep only the last N log lines
        string[] lines = _logBuilder.ToString().Split('\n');
        if (lines.Length > _maxLogLines)
        {
            _logBuilder.Clear();
            for (int i = lines.Length - _maxLogLines; i < lines.Length - 1; i++)
            {
                if (i >= 0 && !string.IsNullOrEmpty(lines[i]))
                    _logBuilder.AppendLine(lines[i]);
            }
        }
        
        _logText.text = _logBuilder.ToString();
    }
    
    public void ClearLogs()
    {
        _logBuilder.Clear();
        if (_logText != null)
            _logText.text = "";
    }
}