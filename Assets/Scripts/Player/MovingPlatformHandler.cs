using UnityEngine;
using Guizzan.Extensions;
public class MovingPlatformHandler : MonoBehaviour
{
    public Transform platform;
    public Vector3 Velocity;
    public Vector3 AngleDelta;

    private Vector3 lastRotation;
    private Vector3 lastPosition;
    public Vector3 LinearDisplacement;
    public Vector3 AngularDisplacement;

    void FixedUpdate()
    {
        if (platform == null) return;
        AngleDelta = GetAngleDelta();
        AngularDisplacement = AngleDelta != Vector3.zero ? transform.position - transform.position.RotatePointWithPivot(platform.transform.position, AngleDelta.y) : Vector3.zero;
        AngularDisplacement.y = 0;
        LinearDisplacement = GetDisplacement();
        Velocity = (LinearDisplacement + AngularDisplacement) / Time.fixedDeltaTime;
    }
    public void SetPlatform(Transform Platform)
    {
        if (platform == Platform) return;
        ResetPlatform();
        platform = Platform;
        lastPosition = platform.position;
        lastRotation = platform.eulerAngles;
    }
    public void ResetPlatform()
    {
        platform = null;
        AngularDisplacement = Vector3.zero;
        LinearDisplacement = Vector3.zero;
        AngleDelta = Vector3.zero;
        AngularDisplacement = Vector3.zero;
        Velocity = Vector3.zero;
    } 
    private Vector3 GetDisplacement()
    {
        Vector3 lastPos = lastPosition;
        lastPosition = platform.position;
        return lastPosition - lastPos;
    }

    private Vector3 GetAngleDelta()
    {
        Vector3 lastRot = lastRotation;
        lastRotation = platform.eulerAngles;
        Vector3 angle = lastRotation - lastRot;
        return angle;
    }
}
