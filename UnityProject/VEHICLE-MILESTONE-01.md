# Vehicle Physics Milestone 01 — test scene

This milestone stays focused on one thing: **the vehicle's driving, steering and suspension**.

## Files
- `Assets/Scripts/Vehicle/HillClimbVehicleController.cs` — rear-wheel drive, braking, smoothed throttle, front-wheel steering, wheel visual sync, anti-roll stability and mobile input hooks.
- `Assets/Scripts/Vehicle/VehicleTestCameraFollow.cs` — smooth chase camera for the test scene.
- `Assets/Scripts/Vehicle/VehicleTouchButton.cs` — pointer down/up adapter for accelerator, reverse, brake, steering and reset UI buttons.
- `Assets/Editor/HillClimbVehicleTestScene.cs` — creates a repeatable test rig with four configured WheelColliders, asphalt track and edge markings, low-poly roadside trees and rocks, gentle ramp, soft directional shadows, ambient lighting and distance fog.

## Build the test rig in Unity
1. Open a Unity 3D project (Unity 2022 LTS or newer).
2. Copy the complete `UnityProject/Assets` folder into the Unity project's `Assets` folder.
3. Wait for Unity to compile. If Unity reports an error, fix it before testing.
4. In Unity's top menu, choose **Hill Climb 3D → Vehicle → Build Physics Test Scene**.
5. Save the generated scene as `VehiclePhysicsTest`.
6. The scene builder sets Android orientation to **Landscape Left** and uses a **1920 × 1080 (16:9) UI reference layout**. Press Play. Use **W / Up Arrow** to accelerate, **S / Down Arrow** to reverse, **A / D** or **Left / Right Arrow** to steer, and **Space** to brake. Touch input is included in the generated scene.
7. Test on the flat track first, then approach the gentle ramp at moderate speed. The camera follows the vehicle.

## Mobile input hooks
- `SetThrottle(1)` while accelerator is held, then `SetThrottle(0)` on release.
- `SetThrottle(-1)` for reverse if desired.
- `SetSteering(-1)` for left, `SetSteering(1)` for right, and `SetSteering(0)` on release.
- `SetBrake(true)` while brake is held, then `SetBrake(false)` on release.

To wire a mobile UI Button, add `VehicleTouchButton` to its GameObject, assign the vehicle controller, and choose its Control Type in the Inspector. The button needs a raycastable Image and the scene needs an EventSystem with an input module. The generated test scene now includes on-screen LEFT, RIGHT, REVERSE, BRAKE, GAS and RESET buttons. Accelerator, reverse and each steering direction now track their own pressed state, so releasing one finger does not cancel a different held control. The touch adapter releases continuous input when the finger lifts, leaves a button or the component is disabled. The generated UI uses Unity's legacy EventSystem input module; if the project is configured for the new Input System only, switch Active Input Handling to Both or configure an Input System UI module.

## Visual improvements in the test scene
- Asphalt road surface raised clearly above the ground, with outer edge lines and dashed center markings.
- The wheel cylinder visual is aligned to the WheelCollider axle before its physics pose is applied, avoiding the sideways axle/incorrect-looking spin issue.
- Low-poly roadside trees and rocks placed outside the main wheel path.
- Softer directional shadows, warmer sunlight, ambient sky/ground colors and light distance fog.
- These are development-scene visuals; the final Jeep model, terrain art and mobile graphics optimization are still future work.
- The browser prototype is a separate lightweight preview. It now has a visible road ribbon, forward/reverse wheel spin, brake-to-stop behavior and a portrait rotate prompt.

## What to inspect
- Smooth acceleration and reverse, without sudden jumps.
- Steering responds smoothly and becomes less sensitive at higher speed.
- All four visual wheels stay aligned with their WheelColliders.
- Chassis settles after landing rather than bouncing repeatedly.
- Vehicle remains upright and controllable on the gentle ramp.
- Braking slows the vehicle predictably and the camera keeps the vehicle framed.

## Tuning order
Change only one group at a time:
1. WheelCollider radius and position to match wheel meshes.
2. Suspension spring and damper to reduce bouncing without making the ride rigid.
3. Center of mass and anti-roll values to improve stability.
4. Steering angle/response and motor torque/friction for predictable handling and traction.

Do not add the next gameplay mechanic until these checks pass.

## Honest status
The Unity C# source and editor scene generator are committed to GitHub. They have **not** been compiled or run in Unity in this environment. Unity Editor is required to generate and verify the scene. The generated primitive rig is for physics testing, not final 3D art.
