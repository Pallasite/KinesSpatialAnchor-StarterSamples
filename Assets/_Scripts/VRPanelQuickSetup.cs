using UnityEngine;

/// <summary>
/// Simple script to add VR Control Panel functionality to any scene.
/// Just attach this to any GameObject in the scene and it will automatically
/// create and configure the VR control panel when the scene starts.
/// 
/// Usage:
/// 1. Open the target Unity scene (e.g., "SpatialAnchor Kineseology 3 08-21-25")
/// 2. Create an empty GameObject or use an existing one
/// 3. Add this script to the GameObject
/// 4. Play the scene in VR - the control panel will appear automatically
/// </summary>
[System.Serializable]
public class VRPanelQuickSetup : MonoBehaviour
{
    [Header("Quick Setup Configuration")]
    [SerializeField] private bool _enableVRControlPanel = true;
    [SerializeField] private bool _showDebugLogs = true;
    [SerializeField] private Vector3 _initialPanelPosition = new Vector3(0, 1.5f, 1.5f);
    
    [Header("Status")]
    [SerializeField] private bool _panelCreated = false;
    
    private void Start()
    {
        if (_enableVRControlPanel && !_panelCreated)
        {
            CreateVRControlPanel();
        }
    }
    
    private void CreateVRControlPanel()
    {
        if (_showDebugLogs)
            Debug.Log("[VRPanelQuickSetup] Creating VR Control Panel...");
        
        // Check if panel already exists
        var existingPanel = FindObjectOfType<VRTrialControlPanel>();
        if (existingPanel != null)
        {
            if (_showDebugLogs)
                Debug.Log("[VRPanelQuickSetup] VR Control Panel already exists!");
            _panelCreated = true;
            return;
        }
        
        // Check if setup script exists
        var setupScript = FindObjectOfType<VRControlPanelSetup>();
        if (setupScript == null)
        {
            // Create a GameObject with the setup script
            var setupGO = new GameObject("VRControlPanelSetup");
            setupScript = setupGO.AddComponent<VRControlPanelSetup>();
            
            // Configure the setup script
            var setupType = typeof(VRControlPanelSetup);
            var positionField = setupType.GetField("_panelPosition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (positionField != null)
                positionField.SetValue(setupScript, _initialPanelPosition);
        }
        
        // Trigger the panel creation
        setupScript.CreateVRControlPanel();
        
        _panelCreated = true;
        
        if (_showDebugLogs)
        {
            Debug.Log("[VRPanelQuickSetup] VR Control Panel setup complete!");
            Debug.Log("[VRPanelQuickSetup] The panel will automatically follow your head in VR.");
            Debug.Log("[VRPanelQuickSetup] Use hand tracking or controllers to interact with buttons.");
        }
    }
    
    /// <summary>
    /// Manual trigger for creating the panel (can be called from editor or other scripts)
    /// </summary>
    [ContextMenu("Create VR Panel Now")]
    public void CreatePanelManually()
    {
        CreateVRControlPanel();
    }
    
    /// <summary>
    /// Remove the VR control panel
    /// </summary>
    [ContextMenu("Remove VR Panel")]
    public void RemoveVRControlPanel()
    {
        var panel = FindObjectOfType<VRTrialControlPanel>();
        if (panel != null)
        {
            DestroyImmediate(panel.gameObject);
            _panelCreated = false;
            if (_showDebugLogs)
                Debug.Log("[VRPanelQuickSetup] VR Control Panel removed.");
        }
        else
        {
            if (_showDebugLogs)
                Debug.Log("[VRPanelQuickSetup] No VR Control Panel found to remove.");
        }
    }
    
    /// <summary>
    /// Toggle the VR control panel on/off
    /// </summary>
    public void ToggleVRControlPanel()
    {
        if (_panelCreated)
        {
            RemoveVRControlPanel();
        }
        else
        {
            CreateVRControlPanel();
        }
    }
    
    /// <summary>
    /// Check system status and provide diagnostics
    /// </summary>
    [ContextMenu("Check VR Setup Status")]
    public void CheckVRSetupStatus()
    {
        Debug.Log("=== VR Control Panel Setup Status ===");
        
        // Check for OVRCameraRig
        var cameraRig = FindObjectOfType<OVRCameraRig>();
        Debug.Log($"OVRCameraRig found: {cameraRig != null}");
        
        // Check for KinesBoxesManager
        var kinesManager = FindObjectOfType<KinesBoxesManager>();
        Debug.Log($"KinesBoxesManager found: {kinesManager != null}");
        
        // Check for VR Control Panel
        var panel = FindObjectOfType<VRTrialControlPanel>();
        Debug.Log($"VR Control Panel exists: {panel != null}");
        
        // Check for VR Button components
        var vrButtons = FindObjectsOfType<VRButton>();
        Debug.Log($"VR Buttons found: {vrButtons.Length}");
        
        Debug.Log("================================");
        
        if (cameraRig == null)
        {
            Debug.LogWarning("OVRCameraRig not found! Make sure you have Oculus VR setup in your scene.");
        }
        
        if (kinesManager == null)
        {
            Debug.LogWarning("KinesBoxesManager not found! The control panel won't be able to control trials.");
        }
        
        if (panel != null && kinesManager != null)
        {
            Debug.Log("✓ VR Control Panel is ready for use!");
        }
    }
}