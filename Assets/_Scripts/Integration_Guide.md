# Integration Guide: Adding VR Control Panel to SpatialAnchor Kineseology 3 08-21-25 Scene

## Quick Setup (Recommended)

The easiest way to add VR control panel functionality to the 08-21 scene:

### Steps:
1. **Open the target scene**
   - Navigate to `Assets/_Scenes/`
   - Open `SpatialAnchor Kineseology 3 08-21-25.unity`

2. **Add the quick setup script**
   - Create a new empty GameObject in the scene (GameObject → Create Empty)
   - Name it "VR_Control_Panel_Manager" 
   - Add the `VRPanelQuickSetup` component to it
   - The component will be found in: Add Component → Scripts → VRPanelQuickSetup

3. **Configure (optional)**
   - In the Inspector, you can adjust:
     - `Enable VR Control Panel`: Keep this checked
     - `Show Debug Logs`: Keep checked for troubleshooting
     - `Initial Panel Position`: Default (0, 1.5, 1.5) should work well

4. **Test the setup**
   - Enter Play mode or deploy to VR headset
   - The control panel should appear automatically
   - Use hand tracking or controllers to interact with buttons

## What Gets Created

The system will automatically create:
- A world-space Canvas with VR-optimized scaling
- Buttons for:
  - Initialize (setup the trial system)
  - Start Trials / Stop Trials (begin/end automatic trial detection)
  - Previous Stroop / Next Stroop (manual navigation)
  - Previous Math / Next Math (manual navigation)  
  - Clear Logs (reset the log display)
- Status display showing current trial numbers and system state
- Log display showing Unity console messages
- VR interaction zones for both hand tracking and controller input

## Verification Steps

After setup, verify the integration:

1. **Check System Status**
   - Right-click on the VRPanelQuickSetup component
   - Select "Check VR Setup Status" from the context menu
   - Review the console output for any issues

2. **Expected Console Messages**
   ```
   === VR Control Panel Setup Status ===
   OVRCameraRig found: True
   KinesBoxesManager found: True  
   VR Control Panel exists: True
   VR Buttons found: 8
   ================================
   ✓ VR Control Panel is ready for use!
   ```

## Troubleshooting

### "OVRCameraRig not found"
- Ensure the scene has proper Oculus VR setup
- Look for OVRCameraRig in the scene hierarchy
- If missing, add one from Oculus Integration package

### "KinesBoxesManager not found" 
- Verify KinesBoxesManager script is attached to a GameObject in the scene
- Check that the script is enabled
- Look for objects with "VisualTargets" tag (operand boxes)

### "Panel not appearing in VR"
- Check the panel position isn't inside other objects
- Try adjusting Initial Panel Position in the setup script
- Ensure the scene has proper lighting for UI visibility

### "Buttons not responding"
- Verify hands/controllers have colliders
- Check that OVR Hand Tracking is enabled in project settings
- Test with both hand tracking and controllers

## Manual Alternative

If you prefer manual setup:

1. Add `VRControlPanelSetup` script to any GameObject
2. Right-click the script → "Create VR Control Panel"
3. The panel will be created and configured automatically

## Scene Persistence

The VR control panel is created at runtime and won't persist when you stop the scene. This is by design to keep the scene file clean. The panel will be recreated each time you play the scene.

If you want a permanent panel in the scene:
1. Run the setup once to create the panel
2. In the scene hierarchy, right-click the created "VRTrialControlPanel" 
3. Select "Apply to Prefab" or make it a prefab manually
4. Now you can save it as part of the scene

## Performance Notes

- The panel uses efficient update patterns (2-second status refresh)
- Log display is limited to 10 recent entries
- VR interactions use trigger-based collision detection
- Panel repositioning uses coroutines for smooth movement

## Next Steps

After successful integration:
1. Test all button functions in VR
2. Verify trial navigation works correctly
3. Test with both hand tracking and controllers
4. Adjust panel position if needed for user comfort
5. Customize button layout or styling if desired

The system is designed to work seamlessly with the existing KinesBoxesManager without requiring any modifications to the original experiment code.