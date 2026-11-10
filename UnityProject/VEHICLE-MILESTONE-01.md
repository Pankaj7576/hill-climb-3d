# Vehicle Physics Milestone 01 — test scene

This milestone stays focused on one thing: **the vehicle's driving and suspension**.

## Files
- `Assets/Scripts/Vehicle/HillClimbVehicleController.cs` — rear-wheel drive, braking, smoothed throttle, wheel visual sync, anti-roll stability and mobile input hooks.
- `Assets/Editor/HillClimbVehicleTestScene.cs` — creates a repeatable primitive test rig with four configured WheelColliders, a flat track, gentle ramp, camera and light.

## Build the test rig in Unity
1. Open a Unity 3D project (Unity 2022 LTS or newer).
2. Copy the complete `UnityProject/Assets` folder into the Unity project's `Assets` folder. Keep the folder names unchanged.
3. Wait for Unity to compile. If Unity reports an error, stop and fix it before testing.
4. In Unity's top menu, choose **Hill Climb 3D → Vehicle → Build Physics Test Scene**.
5. Save the generated scene as `VehiclePhysicsTest`.
6. Press Play. Use **W / Up Arrow** to accelerate, **S / Down Arrow** to reverse, and **Space** to brake.
7. Test on the flat track first, then approach the gentle ramp at moderate speed.

## What to inspect
- Smooth acceleration and reverse, without sudden jumps.
- All four visual wheels stay aligned with their WheelColliders.
- Chassis settles after landing rather than bouncing repeatedly.
- Vehicle remains upright and controllable on the gentle ramp.
- Braking slows the vehicle predictably.

## Tuning order
Change only one group at a time:
1. WheelCollider radius and position to match wheel meshes.
2. Suspension spring and damper to reduce bouncing without making the ride rigid.
3. Center of mass and anti-roll values to improve stability.
4. Motor torque and friction to achieve predictable acceleration and traction.

Do not add the next gameplay mechanic until these checks pass.

## Honest status
The Unity C# controller and editor scene generator are source files committed to GitHub. They have **not** been compiled or run in Unity in this environment. Unity Editor is required to generate and verify the scene. The generated primitive rig is for physics testing, not final 3D art.
