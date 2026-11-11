#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Creates a repeatable, primitive-only test scene for Vehicle Physics Milestone 01.
/// This is a development rig, not final art.
/// </summary>
public static class HillClimbVehicleTestScene
{
    [MenuItem("Hill Climb 3D/Vehicle/Build Physics Test Scene")]
    public static void Build()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;

        var root = new GameObject("HC3D_Vehicle_Test_Rig");
        root.transform.position = new Vector3(0f, 0.58f, 0f);

        var body = root.AddComponent<Rigidbody>();
        body.mass = 900f;
        body.drag = 0.02f;
        body.angularDrag = 0.5f;
        body.interpolation = RigidbodyInterpolation.Interpolate;
        body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        var controller = root.AddComponent<HillClimbVehicleController>();

        Material chassisMat = MakeMaterial("HC3D_Chassis_Material", new Color(0.12f, 0.34f, 0.82f));
        Material wheelMat = MakeMaterial("HC3D_Tire_Material", new Color(0.045f, 0.05f, 0.06f));
        Material groundMat = MakeMaterial("HC3D_Test_Ground_Material", new Color(0.28f, 0.34f, 0.24f));
        Material rampMat = MakeMaterial("HC3D_Test_Ramp_Material", new Color(0.48f, 0.43f, 0.34f));

        GameObject chassis = GameObject.CreatePrimitive(PrimitiveType.Cube);
        chassis.name = "Chassis_Blockout";
        chassis.transform.SetParent(root.transform, false);
        chassis.transform.localPosition = new Vector3(0f, 0.38f, 0f);
        chassis.transform.localScale = new Vector3(1.85f, 0.58f, 3.25f);
        chassis.GetComponent<Renderer>().sharedMaterial = chassisMat;

        WheelCollider fl = CreateWheel(root.transform, "WheelCollider_FL", -0.96f, -0.16f, 1.08f, wheelMat, out Transform flVisual);
        WheelCollider fr = CreateWheel(root.transform, "WheelCollider_FR",  0.96f, -0.16f, 1.08f, wheelMat, out Transform frVisual);
        WheelCollider rl = CreateWheel(root.transform, "WheelCollider_RL", -0.96f, -0.16f, -1.08f, wheelMat, out Transform rlVisual);
        WheelCollider rr = CreateWheel(root.transform, "WheelCollider_RR",  0.96f, -0.16f, -1.08f, wheelMat, out Transform rrVisual);

        SerializedObject so = new SerializedObject(controller);
        SetObject(so, "frontLeft", fl);
        SetObject(so, "frontRight", fr);
        SetObject(so, "rearLeft", rl);
        SetObject(so, "rearRight", rr);
        SetObject(so, "frontLeftVisual", flVisual);
        SetObject(so, "frontRightVisual", frVisual);
        SetObject(so, "rearLeftVisual", rlVisual);
        SetObject(so, "rearRightVisual", rrVisual);
        so.ApplyModifiedPropertiesWithoutUndo();

        CreateBlock("Flat_Test_Ground", new Vector3(0f, -0.12f, 0f), new Vector3(18f, 0.24f, 35f), groundMat);
        CreateBlock("Gentle_Test_Ramp", new Vector3(0f, 0.38f, 13f), new Vector3(9f, 0.35f, 12f), rampMat, Quaternion.Euler(-10f, 0f, 0f));

        GameObject cameraObject = new GameObject("Vehicle_Test_Camera");
        Camera camera = cameraObject.AddComponent<Camera>();
        cameraObject.tag = "MainCamera";
        camera.fieldOfView = 62f;
        cameraObject.transform.position = new Vector3(5.5f, 4.2f, -7.5f);
        cameraObject.transform.LookAt(root.transform.position + new Vector3(0f, 0.2f, 2f));
        VehicleTestCameraFollow follow = cameraObject.AddComponent<VehicleTestCameraFollow>();
        follow.SetTarget(root.transform);
        CreateMobileControls(controller);

        GameObject lightObject = new GameObject("Vehicle_Test_Directional_Light");
        Light sun = lightObject.AddComponent<Light>();
        sun.type = LightType.Directional;
        sun.intensity = 1.2f;
        lightObject.transform.rotation = Quaternion.Euler(45f, -30f, 0f);

        Selection.activeGameObject = root;
        EditorSceneManager.MarkSceneDirty(root.scene);
        Debug.Log("HC3D test rig created. Press Play; use W/S or Up/Down and Space to brake. Drive toward the gentle ramp.");
    }


    private static void CreateMobileControls(HillClimbVehicleController controller)
    {
        if (Object.FindObjectOfType<EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }

        GameObject canvasObject = new GameObject("Mobile_Vehicle_Controls");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;
        canvasObject.AddComponent<GraphicRaycaster>();

        CreateTouchButton(canvas.transform, "LEFT", new Vector2(0f, 0f), new Vector2(130f, 125f), controller, VehicleTouchButton.ControlType.SteerLeft);
        CreateTouchButton(canvas.transform, "RIGHT", new Vector2(0f, 0f), new Vector2(300f, 125f), controller, VehicleTouchButton.ControlType.SteerRight);
        CreateTouchButton(canvas.transform, "BRAKE", new Vector2(1f, 0f), new Vector2(-300f, 125f), controller, VehicleTouchButton.ControlType.Brake);
        CreateTouchButton(canvas.transform, "GAS", new Vector2(1f, 0f), new Vector2(-125f, 125f), controller, VehicleTouchButton.ControlType.Accelerator);
        CreateTouchButton(canvas.transform, "RESET", new Vector2(1f, 1f), new Vector2(-105f, -75f), controller, VehicleTouchButton.ControlType.Reset);
    }

    private static void CreateTouchButton(Transform parent, string label, Vector2 anchor, Vector2 position,
        HillClimbVehicleController controller, VehicleTouchButton.ControlType control)
    {
        GameObject buttonObject = new GameObject(label + "_Button");
        buttonObject.transform.SetParent(parent, false);
        RectTransform rect = buttonObject.AddComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(150f, 125f);

        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.04f, 0.10f, 0.16f, 0.78f);
        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;

        GameObject labelObject = new GameObject("Label");
        labelObject.transform.SetParent(buttonObject.transform, false);
        RectTransform labelRect = labelObject.AddComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;
        Text text = labelObject.AddComponent<Text>();
        text.text = label;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 30;
        text.fontStyle = FontStyle.Bold;

        VehicleTouchButton touch = buttonObject.AddComponent<VehicleTouchButton>();
        touch.Configure(controller, control);
    }

    private static WheelCollider CreateWheel(Transform parent, string objectName, float x, float y, float z, Material tireMaterial, out Transform visual)
    {
        var wheelObject = new GameObject(objectName);
        wheelObject.transform.SetParent(parent, false);
        wheelObject.transform.localPosition = new Vector3(x, y, z);

        var wheel = wheelObject.AddComponent<WheelCollider>();
        wheel.mass = 22f;
        wheel.radius = 0.42f;
        wheel.suspensionDistance = 0.32f;
        wheel.wheelDampingRate = 1.8f;
        JointSpring spring = wheel.suspensionSpring;
        spring.spring = 36000f;
        spring.damper = 5200f;
        spring.targetPosition = 0.52f;
        wheel.suspensionSpring = spring;

        WheelFrictionCurve forward = wheel.forwardFriction;
        forward.extremumSlip = 0.4f;
        forward.extremumValue = 1f;
        forward.asymptoteSlip = 0.8f;
        forward.asymptoteValue = 0.72f;
        forward.stiffness = 1.35f;
        wheel.forwardFriction = forward;

        WheelFrictionCurve sideways = wheel.sidewaysFriction;
        sideways.extremumSlip = 0.22f;
        sideways.extremumValue = 1f;
        sideways.asymptoteSlip = 0.5f;
        sideways.asymptoteValue = 0.75f;
        sideways.stiffness = 1.55f;
        wheel.sidewaysFriction = sideways;

        GameObject visualObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        visualObject.name = objectName.Replace("WheelCollider", "WheelVisual");
        visualObject.transform.SetParent(parent, false);
        visualObject.transform.localPosition = wheelObject.transform.localPosition;
        visualObject.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
        visualObject.transform.localScale = new Vector3(0.82f, 0.18f, 0.82f);
        visualObject.GetComponent<Renderer>().sharedMaterial = tireMaterial;
        Object.DestroyImmediate(visualObject.GetComponent<Collider>());
        visual = visualObject.transform;
        return wheel;
    }

    private static void CreateBlock(string name, Vector3 position, Vector3 scale, Material material, Quaternion rotation = default)
    {
        GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
        block.name = name;
        block.transform.position = position;
        block.transform.rotation = rotation;
        block.transform.localScale = scale;
        block.GetComponent<Renderer>().sharedMaterial = material;
    }

    private static Material MakeMaterial(string name, Color color)
    {
        Shader shader = Shader.Find("Standard");
        if (shader == null)
            shader = Shader.Find("Universal Render Pipeline/Lit");
        var material = new Material(shader) { name = name, color = color };
        return material;
    }

    private static void SetObject(SerializedObject serializedObject, string propertyName, Object value)
    {
        SerializedProperty property = serializedObject.FindProperty(propertyName);
        if (property != null)
            property.objectReferenceValue = value;
    }
}
#endif
