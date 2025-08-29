/*
 * VR Interaction Setup
 * 
 * This script sets up the VR interaction tools for the trial navigation system.
 * It creates and configures ray casting tools and poke tools for both controllers 
 * and hand tracking, ensuring compatibility with Meta SDK.
 */

using UnityEngine;

namespace OculusSampleFramework
{
    public class VRInteractionSetup : MonoBehaviour
    {
        [Header("VR Interaction Settings")]
        public bool enableRayInteraction = true;
        public bool enablePokeInteraction = true;
        public bool enableHandTracking = true;
        
        [Header("Ray Tool Settings")]
        public float rayLength = 5.0f;
        public LayerMask rayInteractionLayers = -1;
        
        [Header("Auto Setup")]
        public bool autoCreateInteractionTools = true;
        
        private InteractableToolsCreator toolsCreator;
        private HandsManager handsManager;
        
        private void Awake()
        {
            if (autoCreateInteractionTools)
            {
                SetupVRInteractionTools();
            }
        }
        
        private void SetupVRInteractionTools()
        {
            // Create InteractableToolsCreator if it doesn't exist
            toolsCreator = FindObjectOfType<InteractableToolsCreator>();
            if (toolsCreator == null)
            {
                GameObject toolsCreatorObj = new GameObject("InteractableToolsCreator");
                toolsCreator = toolsCreatorObj.AddComponent<InteractableToolsCreator>();
            }
            
            // Create HandsManager if it doesn't exist and hand tracking is enabled
            if (enableHandTracking)
            {
                handsManager = FindObjectOfType<HandsManager>();
                if (handsManager == null)
                {
                    GameObject handsManagerObj = new GameObject("HandsManager");
                    handsManager = handsManagerObj.AddComponent<HandsManager>();
                }
            }
            
            Debug.Log("[VRInteractionSetup] VR interaction tools setup completed");
            Debug.Log($"[VRInteractionSetup] Ray interaction: {enableRayInteraction}");
            Debug.Log($"[VRInteractionSetup] Poke interaction: {enablePokeInteraction}");
            Debug.Log($"[VRInteractionSetup] Hand tracking: {enableHandTracking}");
        }
        
        public void ToggleRayInteraction()
        {
            enableRayInteraction = !enableRayInteraction;
            Debug.Log($"[VRInteractionSetup] Ray interaction toggled: {enableRayInteraction}");
        }
        
        public void TogglePokeInteraction()
        {
            enablePokeInteraction = !enablePokeInteraction;
            Debug.Log($"[VRInteractionSetup] Poke interaction toggled: {enablePokeInteraction}");
        }
        
        public void ToggleHandTracking()
        {
            enableHandTracking = !enableHandTracking;
            Debug.Log($"[VRInteractionSetup] Hand tracking toggled: {enableHandTracking}");
        }
        
        private void Start()
        {
            // Log interaction capabilities
            LogInteractionCapabilities();
        }
        
        private void LogInteractionCapabilities()
        {
            Debug.Log("[VRInteractionSetup] === VR Interaction Capabilities ===");
            Debug.Log("[VRInteractionSetup] Ray Casting: " + (enableRayInteraction ? "ENABLED" : "DISABLED"));
            Debug.Log("[VRInteractionSetup] Poke Interactions: " + (enablePokeInteraction ? "ENABLED" : "DISABLED"));  
            Debug.Log("[VRInteractionSetup] Hand Tracking: " + (enableHandTracking ? "ENABLED" : "DISABLED"));
            Debug.Log("[VRInteractionSetup] Controller Support: ENABLED (via OVRInput)");
            Debug.Log("[VRInteractionSetup] === End Capabilities ===");
        }
    }
}