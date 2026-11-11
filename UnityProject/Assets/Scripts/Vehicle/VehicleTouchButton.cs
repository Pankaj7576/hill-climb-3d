using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Pointer-down/up adapter for the vehicle's on-screen touch controls.
/// Add this component to a UI Button with a raycastable Graphic.
/// </summary>
public sealed class VehicleTouchButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public enum ControlType { Accelerator, Reverse, Brake, SteerLeft, SteerRight, Reset }

    [SerializeField] private HillClimbVehicleController vehicle;
    [SerializeField] private ControlType control;

    public void Configure(HillClimbVehicleController target, ControlType type)
    {
        vehicle = target;
        control = type;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (vehicle == null) return;

        if (control == ControlType.Reset)
        {
            vehicle.ResetVehicle();
            return;
        }

        switch (control)
        {
            case ControlType.Accelerator: vehicle.SetThrottle(1f); break;
            case ControlType.Reverse: vehicle.SetThrottle(-1f); break;
            case ControlType.Brake: vehicle.SetBrake(true); break;
            case ControlType.SteerLeft: vehicle.SetSteering(-1f); break;
            case ControlType.SteerRight: vehicle.SetSteering(1f); break;
        }
    }

    public void OnPointerUp(PointerEventData eventData) => Release();
    public void OnPointerExit(PointerEventData eventData) => Release();

    private void OnDisable() => Release();

    private void Release()
    {
        if (vehicle == null) return;

        switch (control)
        {
            case ControlType.Accelerator:
            case ControlType.Reverse: vehicle.SetThrottle(0f); break;
            case ControlType.Brake: vehicle.SetBrake(false); break;
            case ControlType.SteerLeft:
            case ControlType.SteerRight: vehicle.SetSteering(0f); break;
        }
    }
}
