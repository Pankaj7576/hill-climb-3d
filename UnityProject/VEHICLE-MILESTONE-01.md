# Hill Climb 3D — Unity vehicle milestone 01

## Current focus
Only the first milestone is being built: **basic vehicle driving and suspension stability**.
Do not add terrain generation, coins, fuel, damage, tilt controls, menus, or other gameplay systems until this vehicle milestone has been tested and tuned.

## Unity setup
1. Create a new 3D Unity project (Unity 2022 LTS or newer).
2. Copy `Assets/Scripts/Vehicle/HillClimbVehicleController.cs` into the same path in the Unity project.
3. Create a vehicle root GameObject and add a Rigidbody.
4. Add four WheelCollider objects under the vehicle: front-left, front-right, rear-left, rear-right.
5. Assign the four colliders in the controller Inspector. Assign wheel mesh transforms if available.
6. Keep the vehicle model's forward direction along local +Z and its up direction along local +Y.
7. Set each WheelCollider's radius to match its wheel mesh. Start with suspension distance around 0.25–0.35 m and tune spring/damper in the Inspector.
8. Place the Rigidbody center of mass lower than the chassis. The script starts with a downward offset of 0.45 m.
9. Press Play and use W/S or Up/Down to drive/reverse; hold Space to brake.

## Mobile input hooks
The controller exposes:
- `SetThrottle(1)` while the accelerator is held, then `SetThrottle(0)` on release.
- `SetThrottle(-1)` for reverse if desired.
- `SetBrake(true)` while brake is held, then `SetBrake(false)` on release.

A touch UI will be added only after the physics tuning milestone is approved.

## Completion checklist
- [ ] Vehicle accelerates and reverses predictably.
- [ ] All four wheels follow the suspension and their visual meshes stay aligned.
- [ ] Vehicle remains controllable on a simple ramp.
- [ ] No persistent bouncing, excessive rolling, or wheel jitter.
- [ ] Keyboard and mobile input hooks both work.

## Important
This is a Unity source milestone, not a ready-to-install Android APK. Building and testing the scene requires Unity Editor on a computer or a compatible cloud build environment. The Android phone can be used to test a built APK later.
