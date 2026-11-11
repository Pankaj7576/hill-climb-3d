using UnityEngine;

/// <summary>Simple smooth chase camera used by the vehicle physics test scene.</summary>
public sealed class VehicleTestCameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(5.5f, 4.2f, -7.5f);
    [SerializeField, Min(0.1f)] private float followSpeed = 5f;
    [SerializeField, Min(0.1f)] private float lookHeight = 0.5f;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + target.rotation * offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        Vector3 lookPoint = target.position + Vector3.up * lookHeight + target.forward * 2f;
        Quaternion desiredRotation = Quaternion.LookRotation(lookPoint - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, followSpeed * Time.deltaTime);
    }
}
