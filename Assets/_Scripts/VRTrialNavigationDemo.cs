/*
 * VR Trial Navigation Demo
 * 
 * This script demonstrates how to use the VR Trial Navigation system.
 * It shows how to programmatically interact with the trial system and
 * provides examples of extending the functionality.
 */

using System.Collections;
using UnityEngine;

namespace OculusSampleFramework
{
    public class VRTrialNavigationDemo : MonoBehaviour
    {
        [Header("Demo Settings")]
        public bool runDemoOnStart = false;
        public float demoDelay = 3.0f;
        
        [Header("References")]
        public KinesBoxesManager boxesManager;
        public VRTrialNavigationPanel navigationPanel;
        public VRInteractionSetup interactionSetup;
        
        private void Start()
        {
            // Find components if not assigned
            if (boxesManager == null)
                boxesManager = FindObjectOfType<KinesBoxesManager>();
            
            if (navigationPanel == null)
                navigationPanel = FindObjectOfType<VRTrialNavigationPanel>();
            
            if (interactionSetup == null)
                interactionSetup = FindObjectOfType<VRInteractionSetup>();
            
            if (runDemoOnStart)
            {
                StartCoroutine(RunDemo());
            }
            
            LogSystemStatus();
        }
        
        private IEnumerator RunDemo()
        {
            Debug.Log("[VRTrialDemo] Starting VR Trial Navigation Demo...");
            
            yield return new WaitForSeconds(demoDelay);
            
            // Demo 1: Load sample math problems
            Debug.Log("[VRTrialDemo] Demo 1: Loading sample math problems");
            if (boxesManager != null)
            {
                boxesManager.LoadCSVDataMath();
            }
            
            yield return new WaitForSeconds(2f);
            
            // Demo 2: Navigate through trials
            Debug.Log("[VRTrialDemo] Demo 2: Navigating through trials");
            for (int i = 0; i < 3; i++)
            {
                if (boxesManager != null)
                {
                    boxesManager.LoadNextMath();
                    yield return new WaitForSeconds(1.5f);
                }
            }
            
            // Demo 3: Go back to previous trials
            Debug.Log("[VRTrialDemo] Demo 3: Going back to previous trials");
            for (int i = 0; i < 2; i++)
            {
                if (boxesManager != null)
                {
                    boxesManager.LoadPreviousMath();
                    yield return new WaitForSeconds(1.5f);
                }
            }
            
            Debug.Log("[VRTrialDemo] Demo completed!");
        }
        
        private void LogSystemStatus()
        {
            Debug.Log("=== VR TRIAL NAVIGATION SYSTEM STATUS ===");
            Debug.Log($"KinesBoxesManager: {(boxesManager != null ? "FOUND" : "MISSING")}");
            Debug.Log($"VRTrialNavigationPanel: {(navigationPanel != null ? "FOUND" : "MISSING")}");
            Debug.Log($"VRInteractionSetup: {(interactionSetup != null ? "FOUND" : "MISSING")}");
            Debug.Log("==========================================");
            
            if (boxesManager != null)
            {
                Debug.Log($"Current Trial: {boxesManager.GetCurrentTrialIndex()}");
                Debug.Log($"Total Trials: {boxesManager.GetTotalTrials()}");
            }
        }
        
        // Public methods for UI or other scripts to call
        [ContextMenu("Run Demo")]
        public void StartDemo()
        {
            if (!runDemoOnStart)
            {
                StartCoroutine(RunDemo());
            }
        }
        
        [ContextMenu("Load Next Trial")]
        public void LoadNextTrial()
        {
            if (boxesManager != null)
            {
                boxesManager.LoadNextMath();
                Debug.Log("[VRTrialDemo] Manual: Next trial loaded");
            }
        }
        
        [ContextMenu("Load Previous Trial")]
        public void LoadPreviousTrial()
        {
            if (boxesManager != null)
            {
                boxesManager.LoadPreviousMath();
                Debug.Log("[VRTrialDemo] Manual: Previous trial loaded");
            }
        }
        
        [ContextMenu("Toggle Navigation Panel")]
        public void ToggleNavigationPanel()
        {
            if (navigationPanel != null)
            {
                navigationPanel.gameObject.SetActive(!navigationPanel.gameObject.activeSelf);
                Debug.Log($"[VRTrialDemo] Navigation panel: {(navigationPanel.gameObject.activeSelf ? "SHOWN" : "HIDDEN")}");
            }
        }
        
        [ContextMenu("Reset System")]
        public void ResetSystem()
        {
            if (boxesManager != null)
            {
                boxesManager.LoadCSVDataMath();
                Debug.Log("[VRTrialDemo] System reset - reloaded trial data");
            }
        }
        
        private void Update()
        {
            // Keyboard shortcuts for testing (when not in VR)
            if (Input.GetKeyDown(KeyCode.N))
            {
                LoadNextTrial();
            }
            
            if (Input.GetKeyDown(KeyCode.P))
            {
                LoadPreviousTrial();
            }
            
            if (Input.GetKeyDown(KeyCode.T))
            {
                ToggleNavigationPanel();
            }
            
            if (Input.GetKeyDown(KeyCode.R))
            {
                ResetSystem();
            }
            
            if (Input.GetKeyDown(KeyCode.D))
            {
                StartDemo();
            }
        }
    }
}