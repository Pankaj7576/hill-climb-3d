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

    [Header("Stability")]
    [SerializeField] private Vector3 centerOfMassOffset = new Vector3(0f, -0.45f, 0f);
    [SerializeField, Min(0f)] private float frontAntiRoll = 4500f;
    [SerializeField, Min(0f)] private float rearAntiRoll = 5000f;

    private Rigidbody body;
    private float keyboardThrottle;
    private float mobileThrottle;
    private float currentThrottle;
    private bool keyboardBrake;
    private bool mobileBrake;

    /// <summary>Call from a mobile UI button: 1 while pressed, 0 when released.</summary>
    public void SetThrottle(float value)
    {
        mobileThrottle = Mathf.Clamp(value, -1f, 1f);
    }

    /// <summary>Call from a mobile UI brake button.</summary>
    public void SetBrake(bool pressed)
    {
        mobileBrake = pressed;
    }

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
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

        keyboardBrake = keyboardInputEnabled && Input.GetKey(KeyCode.Space);
        UpdateWheelVisual(frontLeft, frontLeftVisual);
        UpdateWheelVisual(frontRight, frontRightVisual);
        UpdateWheelVisual(rearLeft, rearLeftVisual);
        UpdateWheelVisual(rearRight, rearRightVisual);
    }

    private void FixedUpdate()
    {
        float requestedThrottle = Mathf.Abs(mobileThrottle) > 0.001f
            ? mobileThrottle
            : keyboardThrottle;

        currentThrottle = Mathf.MoveTowards(
            currentThrottle,
            requestedThrottle,
            throttleResponse * Time.fixedDeltaTime
        );

        ApplyDrive(rearLeft);
        ApplyDrive(rearRight);

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

        float travelLeft = GetSuspensionTravel(left);
        float travelRight = GetSuspensionTravel(right);
        float force = (travelLeft - travelRight) * antiRoll;

        body.AddForceAtPosition(left.transform.up * -force, left.transform.position);
        body.AddForceAtPosition(right.transform.up * force, right.transform.position);
    }

    private static float GetSuspensionTravel(WheelCollider wheel)
    {
        if (!wheel.GetGroundHit(out WheelHit hit))
            return 1f;

        Vector3 localHit = wheel.transform.InverseTransformPoint(hit.point);
        float compression = (-localHit.y - wheel.radius) / wheel.suspensionDistance;
        return Mathf.Clamp01(1f - compression);
    }

    private static void UpdateWheelVisual(WheelCollider wheel, Transform visual)
    {
        if (wheel == null || visual == null)
            return;

        wheel.GetWorldPose(out Vector3 position, out Quaternion rotation);
        visual.SetPositionAndRotation(position, rotation);
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
        mobileBrake = false;
        body.position += Vector3.up * 0.5f;
        body.rotation = Quaternion.identity;
    }
}
