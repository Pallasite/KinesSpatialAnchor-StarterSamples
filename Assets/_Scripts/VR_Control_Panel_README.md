# VR Trial Control Panel

This VR control panel provides an intuitive 3D interface for managing trial progression in the Kines Spatial Anchor project. The panel supports both VR controller and hand tracking interactions.

## Features

- **HMD Following**: The panel automatically follows the user's head movement, maintaining optimal viewing distance
- **VR Interactions**: Compatible with both Oculus controllers and hand tracking
- **Trial Control**: Buttons for Previous/Next navigation for both Stroop and Math trials
- **System Control**: Initialize, Start/Stop trial detection
- **Live Feedback**: Real-time display of system status and Unity console logs
- **Audio Feedback**: Spatial audio feedback for button interactions

## Components

### 1. VRTrialControlPanel.cs
The main panel controller that:
- Follows the user's HMD using a smooth tracking algorithm based on Meta's PanelHMDFollower
- Manages UI updates and status display
- Interfaces with KinesBoxesManager for trial control
- Captures and displays Unity console logs

### 2. VRButton.cs
A comprehensive VR button system that:
- Creates interaction zones for both proximity (hover) and contact (press) detection
- Provides visual and audio feedback
- Compatible with hand tracking, controllers, and Meta SDK InteractableTools
- Falls back to standard UI Button for compatibility

### 3. VRControlPanelSetup.cs
A utility script that programmatically creates the entire control panel:
- Automatically finds and connects to KinesBoxesManager
- Creates all UI elements with proper VR scaling
- Sets up button interactions and event handlers
- Can be triggered via context menu or on scene start

## Setup Instructions

### Method 1: Automatic Setup (Recommended)
1. Add the `VRControlPanelSetup` script to any GameObject in your scene
2. In the Inspector, ensure "Create On Start" is checked
3. Run the scene - the control panel will be created automatically

### Method 2: Manual Setup
1. Add the `VRControlPanelSetup` script to any GameObject in your scene
2. Right-click on the script in the Inspector
3. Select "Create VR Control Panel" from the context menu

### Method 3: Manual Creation
If you prefer to create the panel manually:
1. Create an empty GameObject named "VRTrialControlPanel"
2. Add the `VRTrialControlPanel` script
3. Create a Canvas as a child (World Space, scaled to 0.001, 0.001, 0.001)
4. Add UI elements and assign them to the script's fields

## Configuration

### Panel Positioning
- `_maxDistance`: Maximum distance from user before panel repositions (default: 0.8m)
- `_minDistance`: Minimum distance from user before panel repositions (default: 0.3m)
- `_panelOffset`: Relative position from user's head (default: 0, -0.2, 0.6)

### Button Functions
- **Initialize**: Calls `KinesBoxesManager.Initialize()`
- **Start Trials**: Calls `KinesBoxesManager.StartTrials()`
- **Stop Trials**: Calls `KinesBoxesManager.StopTrials()`
- **Prev/Next Stroop**: Calls `LoadPreviousStroop()` and `LoadNextStroop()`
- **Prev/Next Math**: Calls `LoadPreviousMath()` and `LoadNextMath()`
- **Clear Logs**: Clears the log display

## VR Interaction Support

The buttons automatically detect and respond to:
- **OVRHand** components (hand tracking)
- **OVRControllerHelper** components (Touch controllers)
- **InteractableTool** components (Meta SDK)
- Objects tagged as "Hand" or "Controller"
- Objects with names containing interaction-related keywords

## Troubleshooting

### Panel Not Following Head
- Ensure an OVRCameraRig is present in the scene
- Check that the panel's `_cameraRig` reference is set correctly

### Buttons Not Responding
- Verify that hands/controllers have proper colliders
- Check that the interaction objects have the correct tags or components
- Ensure the Canvas has a GraphicRaycaster component

### KinesBoxesManager Not Found
- Make sure KinesBoxesManager is in the scene
- Check that the script reference is set in VRTrialControlPanel
- Enable "Auto Find Kines Manager" in the setup script

### No Audio Feedback
- Ensure the panel GameObject has an AudioSource component
- Check that spatial blend is set to 1.0 for 3D audio
- Verify audio clips are assigned to button sound effects

## Integration Notes

The VR control panel integrates seamlessly with the existing KinesBoxesManager without requiring any modifications to the original code. It calls the same public methods that were designed for the system:

- `LoadNextStroop()` / `LoadPreviousStroop()`
- `LoadNextMath()` / `LoadPreviousMath()`
- `Initialize()`
- `StartTrials()` / `StopTrials()`

This ensures backward compatibility while adding VR functionality to the kinesiology experiment setup.

## Performance Considerations

- The panel uses efficient UI update patterns to minimize performance impact
- Log display is limited to the last 10 entries to prevent memory buildup
- Button interactions use trigger-based detection for optimal performance
- HMD following uses coroutines to smooth movement calculations

## Future Enhancements

Potential improvements that could be added:
- Voice control integration
- Gesture-based navigation
- Customizable button layouts
- Data visualization displays
- Remote monitoring capabilities