using UnityEngine;

/// <summary>
/// First Hill Climb 3D vehicle milestone:
/// stable four-wheel suspension and rear-wheel drive.
/// Keep this controller focused on driving physics; tilt, fuel, damage and
/// other gameplay systems will be added as separate milestones.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public sealed class HillClimbVehicleController : MonoBehaviour
{
    [Header("Wheel colliders")]
    [SerializeField] private WheelCollider frontLeft;
    [SerializeField] private WheelCollider frontRight;
    [SerializeField] private WheelCollider rearLeft;
    [SerializeField] private WheelCollider rearRight;

    [Header("Wheel visuals (optional but recommended)")]
    [SerializeField] private Transform frontLeftVisual;
    [SerializeField] private Transform frontRightVisual;
    [SerializeField] private Transform rearLeftVisual;
    [SerializeField] private Transform rearRightVisual;

    [Header("Driving")]
    [SerializeField, Min(0f)] private float motorTorque = 950f;
    [SerializeField, Min(0f)] private float brakeTorque = 1800f;
    [SerializeField, Range(0f, 1f)] private float throttleResponse = 4f;
    [SerializeField] private bool keyboardInputEnabled = true;

    [Header("Steering")]
    [SerializeField, Range(5f, 45f)] private float maximumSteerAngle = 28f;
    [SerializeField, Min(0f)] private float steeringResponse = 5f;
    [SerializeField, Min(0f)] private float highSpeedSteeringReduction = 0.45f;

    [Header("Stability")]
    [SerializeField] private Vector3 centerOfMassOffset = new Vector3(0f, -0.45f, 0f);
    [SerializeField, Min(0f)] private float frontAntiRoll = 4500f;
    [SerializeField, Min(0f)] private float rearAntiRoll = 5000f;

    private Rigidbody body;
    private float keyboardThrottle;
    private float mobileThrottle;
    private bool mobileAcceleratorHeld;
    private bool mobileReverseHeld;
    private bool mobileSteerLeftHeld;
    private bool mobileSteerRightHeld;
    private float currentThrottle;
    private float keyboardSteering;
    private float mobileSteering;
    private float currentSteering;
    private bool keyboardBrake;
    private bool mobileBrake;
    private Vector3 spawnPosition;
    private Quaternion spawnRotation;

    /// <summary>Call from a mobile UI button: 1 while pressed, 0 when released.</summary>
    public void SetThrottle(float value)
    {
        mobileThrottle = Mathf.Clamp(value, -1f, 1f);
    }

    public void SetAccelerator(bool pressed) => mobileAcceleratorHeld = pressed;
    public void SetReverse(bool pressed) => mobileReverseHeld = pressed;
    public void SetSteerLeft(bool pressed) => mobileSteerLeftHeld = pressed;
    public void SetSteerRight(bool pressed) => mobileSteerRightHeld = pressed;

    /// <summary>Call from mobile steering buttons or slider, using -1 to 1.</summary>
    public void SetSteering(float value)
    {
        mobileSteering = Mathf.Clamp(value, -1f, 1f);
    }

    /// <summary>Call from a mobile UI brake button.</summary>
    public void SetBrake(bool pressed)
    {
        mobileBrake = pressed;
    }

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        spawnPosition = body.position;
        spawnRotation = body.rotation;
        body.centerOfMass = centerOfMassOffset;

        if (body.mass < 500f)
            body.mass = 900f;

        ValidateWheelSetup();
    }

    private void Update()
    {
        keyboardThrottle = keyboardInputEnabled
            ? Input.GetAxisRaw("Vertical")
            : 0f;

        keyboardSteering = keyboardInputEnabled ? Input.GetAxisRaw("Horizontal") : 0f;
        keyboardBrake = keyboardInputEnabled && Input.GetKey(KeyCode.Space);
        UpdateWheelVisual(frontLeft, frontLeftVisual);
        UpdateWheelVisual(frontRight, frontRightVisual);
        UpdateWheelVisual(rearLeft, rearLeftVisual);
        UpdateWheelVisual(rearRight, rearRightVisual);
    }

    private void FixedUpdate()
    {
        float buttonThrottle = (mobileAcceleratorHeld ? 1f : 0f) - (mobileReverseHeld ? 1f : 0f);
        float requestedThrottle = Mathf.Abs(buttonThrottle) > 0.001f
            ? buttonThrottle
            : (Mathf.Abs(mobileThrottle) > 0.001f ? mobileThrottle : keyboardThrottle);

        currentThrottle = Mathf.MoveTowards(
            currentThrottle,
            requestedThrottle,
            throttleResponse * Time.fixedDeltaTime
        );

        ApplyDrive(rearLeft);
        ApplyDrive(rearRight);

        float buttonSteering = (mobileSteerRightHeld ? 1f : 0f) - (mobileSteerLeftHeld ? 1f : 0f);
        float requestedSteering = Mathf.Abs(buttonSteering) > 0.001f
            ? buttonSteering
            : (Mathf.Abs(mobileSteering) > 0.001f ? mobileSteering : keyboardSteering);
        currentSteering = Mathf.MoveTowards(currentSteering, requestedSteering, steeringResponse * Time.fixedDeltaTime);
        float speedFactor = Mathf.Clamp01(body.velocity.magnitude / 18f);
        float steerLimit = maximumSteerAngle * (1f - highSpeedSteeringReduction * speedFactor);
        float steerAngle = currentSteering * steerLimit;
        if (frontLeft != null) frontLeft.steerAngle = steerAngle;
        if (frontRight != null) frontRight.steerAngle = steerAngle;

        bool braking = mobileBrake || keyboardBrake;
        float appliedBrake = braking ? brakeTorque : 0f;

        ApplyBrake(frontLeft, appliedBrake);
        ApplyBrake(frontRight, appliedBrake);
        ApplyBrake(rearLeft, appliedBrake);
        ApplyBrake(rearRight, appliedBrake);

        ApplyAntiRoll(frontLeft, frontRight, frontAntiRoll);
        ApplyAntiRoll(rearLeft, rearRight, rearAntiRoll);
    }

    private void ApplyDrive(WheelCollider wheel)
    {
        if (wheel != null)
            wheel.motorTorque = currentThrottle * motorTorque;
    }

    private static void ApplyBrake(WheelCollider wheel, float torque)
    {
        if (wheel != null)
            wheel.brakeTorque = torque;
    }

    private void ApplyAntiRoll(WheelCollider left, WheelCollider right, float antiRoll)
    {
        if (left == null || right == null || antiRoll <= 0f)
            return;

        bool leftGrounded = left.GetGroundHit(out _);
        bool rightGrounded = right.GetGroundHit(out _);
        if (!leftGrounded && !rightGrounded)
            return;

        float travelLeft = GetSuspensionTravel(left);
        float travelRight = GetSuspensionTravel(right);
        float force = (travelLeft - travelRight) * antiRoll;

        if (leftGrounded)
            body.AddForceAtPosition(left.transform.up * -force, left.transform.position);
        if (rightGrounded)
            body.AddForceAtPosition(right.transform.up * force, right.transform.position);
    }

    private static float GetSuspensionTravel(WheelCollider wheel)
    {
        if (!wheel.GetGroundHit(out WheelHit hit))
            return 1f;

        Vector3 localHit = wheel.transform.InverseTransformPoint(hit.point);
        float suspensionRange = Mathf.Max(0.001f, wheel.suspensionDistance);
        float travel = (-localHit.y - wheel.radius) / suspensionRange;
        return Mathf.Clamp01(travel);
    }

    private static void UpdateWheelVisual(WheelCollider wheel, Transform visual)
    {
        if (wheel == null || visual == null)
            return;

        wheel.GetWorldPose(out Vector3 position, out Quaternion rotation);
        // Unity's primitive Cylinder has its axle along local Y; rotate it onto the WheelCollider axle.
        visual.SetPositionAndRotation(position, rotation * Quaternion.Euler(0f, 0f, 90f));
    }

    private void ValidateWheelSetup()
    {
        if (frontLeft == null || frontRight == null || rearLeft == null || rearRight == null)
            Debug.LogWarning(
                "HillClimbVehicleController: Assign all four WheelColliders in the Inspector.",
                this
            );
    }

    /// <summary>Optional reset hook for a future UI reset button.</summary>
    public void ResetVehicle()
    {
        body.velocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        currentThrottle = 0f;
        mobileThrottle = 0f;
        mobileAcceleratorHeld = false;
        mobileReverseHeld = false;
        mobileSteerLeftHeld = false;
        mobileSteerRightHeld = false;
        mobileSteering = 0f;
        currentSteering = 0f;
        mobileBrake = false;
        keyboardThrottle = 0f;
        keyboardSteering = 0f;
        keyboardBrake = false;
        body.position = spawnPosition + Vector3.up * 0.5f;
        body.rotation = spawnRotation;
        if (frontLeft != null)
        {
            frontLeft.steerAngle = 0f;
            frontLeft.motorTorque = 0f;
            frontLeft.brakeTorque = 0f;
        }
        if (frontRight != null)
        {
            frontRight.steerAngle = 0f;
            frontRight.motorTorque = 0f;
            frontRight.brakeTorque = 0f;
        }
        if (rearLeft != null)
        {
            rearLeft.motorTorque = 0f;
            rearLeft.brakeTorque = 0f;
        }
        if (rearRight != null)
        {
            rearRight.motorTorque = 0f;
            rearRight.brakeTorque = 0f;
        }

        body.WakeUp();
    }
}
