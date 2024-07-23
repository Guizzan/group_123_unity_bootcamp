using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class FootIK : MonoBehaviour
{
    public LayerMask groundLayer;
    public float raycastDistance = 1.5f;
    public Vector3 footOffset;
    public float positionSmoothTime = 0.1f;
    public float rotationSmoothTime = 0.1f;
    public float footPitchLimit = 20f;
    public float footRollLimit = 20f;
    private Animator animator;
    private Vector3 leftFootPosition, rightFootPosition;
    private Quaternion leftFootRotation, rightFootRotation;
    private Vector3 FootPositionVelocity;

    private IEnumerator IKDelay;
    public float enableDelay = 0.5f;
    private bool _enableIK;
    private bool resetFootPosition;
    public bool EnableIK
    {
        get => _enableIK;
        set
        {
            if (_enableIK == value) return;
            if (value)
            {
                IKDelay = EnableIKAfterDelay();
                StartCoroutine(IKDelay);
            }
            else
            {
                _enableIK = false;
                if (IKDelay != null)
                {
                    StopCoroutine(IKDelay);
                    IKDelay = null;
                }
            }
        }
    }

    private IEnumerator EnableIKAfterDelay()
    {
        yield return new WaitForSeconds(enableDelay);
        _enableIK = true;
        resetFootPosition = true;
        IKDelay = null;
    }

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnAnimatorIK(int layerIndex)
    {
        print(EnableIK);
        if (animator && EnableIK)
        {
            if (resetFootPosition)
            {
                leftFootPosition = animator.GetIKPosition(AvatarIKGoal.LeftFoot);
                rightFootPosition = animator.GetIKPosition(AvatarIKGoal.RightFoot);
                leftFootRotation = animator.GetIKRotation(AvatarIKGoal.LeftFoot);
                rightFootRotation = animator.GetIKRotation(AvatarIKGoal.RightFoot);
                resetFootPosition = false;
            }
            UpdateFootTarget(ref leftFootPosition, ref leftFootRotation, AvatarIKGoal.LeftFoot);
            UpdateFootTarget(ref rightFootPosition, ref rightFootRotation, AvatarIKGoal.RightFoot);

            // Apply IK if the foot has a target position and rotation
            ApplyFootIK(AvatarIKGoal.LeftFoot, leftFootPosition, leftFootRotation);
            ApplyFootIK(AvatarIKGoal.RightFoot, rightFootPosition, rightFootRotation);
        }
    }

    void UpdateFootTarget(ref Vector3 footPos, ref Quaternion footRot, AvatarIKGoal foot)
    {
        RaycastHit hit;
        var footPosition = animator.GetIKPosition(foot);
        if (Physics.Raycast(footPosition + Vector3.up * raycastDistance, Vector3.down, out hit, raycastDistance * 2, groundLayer))
        {
            footPos = Vector3.SmoothDamp(footPos, hit.point + footOffset, ref FootPositionVelocity, positionSmoothTime);

            Vector3 playerForwardOnGround = Vector3.ProjectOnPlane(transform.forward, hit.normal).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(playerForwardOnGround, hit.normal);

            targetRotation = LimitRotation(targetRotation);

            footRot = Quaternion.Slerp(footRot, targetRotation, rotationSmoothTime);
        }
    }

    Quaternion LimitRotation(Quaternion rotation)
    {
        Vector3 euler = rotation.eulerAngles;
        euler.x = Mathf.Clamp(euler.x, -footPitchLimit, footPitchLimit);
        euler.z = Mathf.Clamp(euler.z, -footRollLimit, footRollLimit);

        return Quaternion.Euler(euler);
    }

    void ApplyFootIK(AvatarIKGoal foot, Vector3 position, Quaternion rotation)
    {
        animator.SetIKPositionWeight(foot, 1);
        animator.SetIKRotationWeight(foot, 1);
        animator.SetIKPosition(foot, position);
        animator.SetIKRotation(foot, rotation);
    }
}