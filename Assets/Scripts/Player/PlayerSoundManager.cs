using System.Collections.Generic;
using UnityEngine;
public class PlayerSoundManager : BaseSoundEvent
{
    public Transform SoundPosTransform;

    public List<SurfaceSound> WalkingSounds = new();
    private Dictionary<int, List<int>> _usedWalkingSounds = new();
    public float MinWalkingStepInterval;
    private float _lastWalkingStepTime;

    public List<SurfaceSound> RunningSounds = new();
    private Dictionary<int, List<int>> _usedRunningSounds = new();
    public float MinRunningStepInterval;
    private float _lastRunningStepTime;

    public List<SurfaceSound> CrouchingSounds = new();
    private Dictionary<int, List<int>> _usedCrouchingSounds = new();
    public float MinCrouchingStepInterval;
    private float _lastCrouchingStepTime;

    public List<SurfaceSound> LandingSounds = new();
    private Dictionary<int, List<int>> _usedLandingSounds = new();
    public float MinLandingInterval;
    private float _lastLandingTime;

    private PlayerController _controller;
    private void Awake()
    {
        _controller = GetComponent<PlayerController>();
    }
    public override void SoundEvent(string type) //Animation Event Trigger
    {
        if (_controller == null) return;
        if (type != "Landing" && (_controller._isJumping || _controller._isFalling)) return;
        if (type == "WalkingStep") type = _controller._isRunning ? "RunningStep" : _controller._isCrouching ? "CrouchingStep" : type;
        int surfaceLayer = _controller.SurfaceHit.collider != null ? _controller.SurfaceHit.collider.gameObject.layer : 0;

        switch (type)
        {
            case "WalkingStep":
                MakeSound(ref SoundPosTransform, surfaceLayer, ref WalkingSounds, ref _usedWalkingSounds, ref MinWalkingStepInterval, ref _lastWalkingStepTime, 0.3f);
                break;
            case "RunningStep":
                MakeSound(ref SoundPosTransform, surfaceLayer, ref RunningSounds, ref _usedRunningSounds, ref MinRunningStepInterval, ref _lastRunningStepTime, 0.8f);
                break;
            case "CrouchingStep":
                MakeSound(ref SoundPosTransform, surfaceLayer, ref CrouchingSounds, ref _usedCrouchingSounds, ref MinCrouchingStepInterval, ref _lastCrouchingStepTime, 0.1f);
                break;
            case "Landing":
                MakeSound(ref SoundPosTransform, surfaceLayer, ref LandingSounds, ref _usedLandingSounds, ref MinLandingInterval, ref _lastLandingTime, 1f);
                break;
        }
    }
}
