# VR Trial Navigation System

A comprehensive 3D VR user interface system for navigating through trials in Unity using the Meta/Oculus SDK. This system provides intuitive VR interactions including ray casting, poke interactions, and hand tracking compatibility.

## Overview

This system was built using the Meta SDK samples as a starting point and provides a complete VR interface for trial navigation with the following features:

- 🎮 **VR Controller Support** - Ray casting interactions for precise selection
- 👆 **Poke Interactions** - Direct touch interactions for natural feel
- 👋 **Hand Tracking** - Full hand tracking compatibility
- 📊 **Real-time Trial Display** - Shows current trial information
- 🔍 **Diagnostic Logging** - Real-time Unity console log viewer
- 🔄 **Auto-positioning** - UI follows the user's headset automatically
- 🎨 **Visual Feedback** - Button color changes and animations
- 🔊 **Audio Feedback** - Optional button press sounds

## Components

### Core Scripts

1. **KinesBoxesManager.cs** - Manages trial data and navigation
2. **VRTrialNavigationPanel.cs** - Main UI panel controller
3. **VRNavigationButton.cs** - VR-compatible button component
4. **VRInteractionSetup.cs** - Configures VR interaction tools
5. **VRTrialNavigationDemo.cs** - Demo and testing utilities

### Scene: 08-21.unity

The main scene containing:
- VR Camera Rig with OVRCameraRig component
- Trial navigation panel with 3D buttons
- TextMeshPro elements for information display
- Lighting and environment setup

## Setup Instructions

### 1. Import Required Packages

Ensure you have the following Unity packages installed:
- Oculus Integration SDK
- TextMeshPro
- XR Plugin Management

### 2. Scene Setup

1. Open the `08-21.unity` scene from `Assets/StarterSamples/Usage/`
2. The scene contains all necessary components pre-configured

### 3. CSV Data Setup

The system expects CSV files in the persistent data path:
- `math_problems.csv` - Math trial data in format: `operand1,operator,operand2`
- `stroop_problems.csv` - Stroop trial data (optional)

Example math_problems.csv:
```csv
2,+,3
5,-,1
4,*,2
8,/,2
```

If no CSV files are found, the system creates sample data automatically.

## Usage

### In VR

1. **Ray Casting**: Point your controller at buttons and pull the trigger
2. **Poke Interaction**: Reach out and directly touch the buttons
3. **Hand Tracking**: Use your hands to interact directly with buttons

### Keyboard Shortcuts (Testing)

When testing in the editor without VR:
- `N` - Load next trial
- `P` - Load previous trial
- `T` - Toggle navigation panel visibility
- `R` - Reset system and reload data
- `D` - Run demo sequence

### Button Functions

- **Previous Button** (Left) - Loads the previous trial
- **Next Button** (Right) - Loads the next trial

### Text Displays

- **Trial Info** (Top) - Shows current trial number and total
- **Diagnostic Logs** (Bottom) - Shows recent Unity console messages

## Customization

### Adjusting Button Sensitivity

In `VRNavigationButton.cs`, modify:
```csharp
public float pressDepth = 0.02f; // How deep button press goes
public float animationSpeed = 5.0f; // Animation speed
```

### Changing UI Position

In `VRTrialNavigationPanel.cs`, adjust:
```csharp
public float followDistance = 1.5f; // Distance from user
public float followHeight = 0.0f; // Height offset
public float smoothSpeed = 2.0f; // Follow smoothness
```

### Modifying Interaction Settings

In `VRInteractionSetup.cs`, configure:
```csharp
public bool enableRayInteraction = true;
public bool enablePokeInteraction = true;
public bool enableHandTracking = true;
public float rayLength = 5.0f;
```

### Button Visual Feedback

In `VRNavigationButton.cs`, customize colors:
```csharp
public Color normalColor = Color.white;
public Color highlightColor = Color.cyan;
public Color pressedColor = Color.green;
```

## Trial Data Management

### Adding Custom Trial Types

1. Extend `KinesBoxesManager.cs` with new trial loading methods
2. Create corresponding CSV format
3. Add UI buttons if needed for different trial types

### Custom Data Sources

Instead of CSV files, you can modify the data loading methods to:
- Load from databases
- Fetch from web APIs
- Generate procedurally

## Troubleshooting

### Common Issues

1. **Buttons not responding**
   - Check that collision zones are properly configured
   - Verify VRInteractionSetup is active
   - Ensure Meta SDK is properly imported

2. **UI not following headset**
   - Verify OVRCameraRig is in the scene
   - Check that followPlayerHeadset is enabled

3. **No trial data**
   - CSV files should be in Application.persistentDataPath
   - System creates sample data if files are missing
   - Check console for loading errors

### Debug Information

The system provides extensive logging:
- All button interactions are logged
- Trial loading status is reported
- System component status is displayed on start

## Extension Points

### Adding New Interaction Types

1. Create a new tool class extending `InteractableTool`
2. Register it with `InteractableToolsCreator`
3. Add corresponding interaction logic

### Custom UI Elements

1. Extend `VRTrialNavigationPanel` with new UI components
2. Add them to the scene hierarchy as children
3. Wire up events and references

### Integration with Other Systems

The system is designed to be modular:
- `KinesBoxesManager` can be used independently
- VR UI components can work with any trial system
- Interaction tools are reusable for other VR interfaces

## Performance Considerations

- UI updates are optimized for VR frame rates
- Text updates only occur when trial data changes
- Button animations use efficient interpolation
- Diagnostic logging has configurable limits

## Dependencies

- Unity 2022.3.15f1+
- Oculus Integration SDK
- Meta XR SDK
- TextMeshPro
- XR Plugin Management

## License

This system is built on top of Meta's Oculus SDK samples and follows their licensing terms.