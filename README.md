# Hill Climb 3D

A mobile-focused Hill Climb game project. Development is being done in small, testable milestones: finish and tune one mechanic before starting the next.

## Current milestone: vehicle physics
The active focus is driving, front steering, suspension stability and a repeatable Unity test scene.

- [Vehicle physics setup and test guide](UnityProject/VEHICLE-MILESTONE-01.md)
- [Unity vehicle controller](UnityProject/Assets/Scripts/Vehicle/HillClimbVehicleController.cs)
- [Smooth test chase camera](UnityProject/Assets/Scripts/Vehicle/VehicleTestCameraFollow.cs)
- [One-click Unity test-scene builder](UnityProject/Assets/Editor/HillClimbVehicleTestScene.cs)

### Create the Unity test scene
Copy the `UnityProject/Assets` folder into a Unity 2022 LTS (or newer) 3D project. In Unity, choose **Hill Climb 3D → Vehicle → Build Physics Test Scene**. Read the milestone guide before testing.

## Browser prototype
The root `index.html` is an earlier Three.js browser prototype, separate from the Unity project:
https://pankaj7576.github.io/hill-climb-3d/

The browser prototype is not the final Unity game and is not an Android APK.

## Important build note
The Unity source files are stored here, but a Unity Editor or compatible cloud build is required to compile, run the test scene and build an Android APK. The current Unity scripts have not yet been compiled or play-tested in Unity, so this milestone is still awaiting in-Editor verification.
