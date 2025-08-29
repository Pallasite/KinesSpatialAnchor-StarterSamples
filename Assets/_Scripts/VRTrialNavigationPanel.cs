/*
 * VR Trial Navigation Panel
 * 
 * This script creates a 3D VR user interface panel that allows users to navigate
 * through trials using VR controller ray casting, poke interactions, and hand tracking.
 * Compatible with Meta/Oculus VR SDK.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace OculusSampleFramework
{
    public class VRTrialNavigationPanel : MonoBehaviour
    {
        [Header("References")]
        public KinesBoxesManager boxesManager;
        public Button previousButton;
        public Button nextButton;
        public TextMeshProUGUI trialInfoText;
        public TextMeshProUGUI diagnosticLogText;
        
        [Header("Panel Settings")]
        public float followDistance = 1.5f;
        public float followHeight = 0.0f;
        public bool followPlayerHeadset = true;
        public float smoothSpeed = 2.0f;
        
        [Header("Diagnostic Settings")]
        public int maxLogLines = 10;
        public bool showDebugLogs = true;
        
        private OVRCameraRig _cameraRig;
        private Queue<string> logMessages = new Queue<string>();
        
        private void Awake()
        {
            _cameraRig = FindObjectOfType<OVRCameraRig>();
            
            // Find KinesBoxesManager if not assigned
            if (boxesManager == null)
            {
                boxesManager = FindObjectOfType<KinesBoxesManager>();
            }
            
            // Setup button listeners
            if (previousButton != null)
            {
                previousButton.onClick.AddListener(OnPreviousButtonClicked);
            }
            
            if (nextButton != null)
            {
                nextButton.onClick.AddListener(OnNextButtonClicked);
            }
            
            // Enable Unity log capturing for diagnostic display
            if (showDebugLogs)
            {
                Application.logMessageReceived += HandleLogMessage;
            }
        }
        
        private void Start()
        {
            // Initial positioning
            PositionPanel();
            UpdateTrialInfo();
        }
        
        private void Update()
        {
            if (followPlayerHeadset && _cameraRig != null)
            {
                PositionPanel();
            }
            
            UpdateTrialInfo();
        }
        
        private void PositionPanel()
        {
            if (_cameraRig == null) return;
            
            Transform centerEye = _cameraRig.centerEyeAnchor;
            Vector3 targetPosition = centerEye.position + centerEye.forward * followDistance;
            targetPosition.y = centerEye.position.y + followHeight;
            
            // Smooth follow
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);
            
            // Face the player
            Vector3 lookDirection = centerEye.position - transform.position;
            lookDirection.y = 0; // Keep panel upright
            if (lookDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(lookDirection);
            }
        }
        
        public void OnPreviousButtonClicked()
        {
            if (boxesManager != null)
            {
                boxesManager.LoadPreviousMath();
                Debug.Log("[VRTrialNavigation] Previous trial loaded");
                AddLogMessage("Previous trial loaded");
            }
            else
            {
                Debug.LogWarning("[VRTrialNavigation] KinesBoxesManager not found!");
                AddLogMessage("ERROR: KinesBoxesManager not found!");
            }
        }
        
        public void OnNextButtonClicked()
        {
            if (boxesManager != null)
            {
                boxesManager.LoadNextMath();
                Debug.Log("[VRTrialNavigation] Next trial loaded");
                AddLogMessage("Next trial loaded");
            }
            else
            {
                Debug.LogWarning("[VRTrialNavigation] KinesBoxesManager not found!");
                AddLogMessage("ERROR: KinesBoxesManager not found!");
            }
        }
        
        private void UpdateTrialInfo()
        {
            if (boxesManager != null && trialInfoText != null)
            {
                int currentTrial = boxesManager.GetCurrentTrialIndex();
                int totalTrials = boxesManager.GetTotalTrials();
                trialInfoText.text = $"Trial: {currentTrial}/{totalTrials}";
            }
        }
        
        private void HandleLogMessage(string logString, string stackTrace, LogType type)
        {
            if (!showDebugLogs) return;
            
            // Format the log message
            string prefix = "";
            switch (type)
            {
                case LogType.Error:
                    prefix = "<color=red>[ERROR]</color>";
                    break;
                case LogType.Warning:
                    prefix = "<color=yellow>[WARN]</color>";
                    break;
                case LogType.Log:
                    prefix = "<color=white>[INFO]</color>";
                    break;
            }
            
            string formattedMessage = $"{prefix} {logString}";
            AddLogMessage(formattedMessage);
        }
        
        private void AddLogMessage(string message)
        {
            logMessages.Enqueue(message);
            
            // Keep only the last maxLogLines messages
            while (logMessages.Count > maxLogLines)
            {
                logMessages.Dequeue();
            }
            
            // Update diagnostic text
            if (diagnosticLogText != null)
            {
                diagnosticLogText.text = string.Join("\n", logMessages.ToArray());
            }
        }
        
        public void ToggleFollowMode()
        {
            followPlayerHeadset = !followPlayerHeadset;
            AddLogMessage($"Follow mode: {(followPlayerHeadset ? "ON" : "OFF")}");
        }
        
        public void ClearDiagnosticLogs()
        {
            logMessages.Clear();
            if (diagnosticLogText != null)
            {
                diagnosticLogText.text = "";
            }
        }
        
        private void OnDestroy()
        {
            // Cleanup
            if (showDebugLogs)
            {
                Application.logMessageReceived -= HandleLogMessage;
            }
            
            if (previousButton != null)
            {
                previousButton.onClick.RemoveListener(OnPreviousButtonClicked);
            }
            
            if (nextButton != null)
            {
                nextButton.onClick.RemoveListener(OnNextButtonClicked);
            }
        }
    }
}