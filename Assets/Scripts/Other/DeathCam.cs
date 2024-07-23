using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathCam : MonoBehaviour
{
    public CinemachineFreeLook freeLookCamera;
    public Transform playerTransform;

    // Orbit speed
    public float orbitSpeed = 30f;

    void Update()
    {
        // Calculate the normalized direction from the player to the camera's position
        Vector3 playerToCameraDirection = (freeLookCamera.transform.position - playerTransform.position).normalized;

        // Calculate the rotation based on the direction
        Quaternion lookRotation = Quaternion.LookRotation(playerToCameraDirection);

        // Set the y-axis rotation of the camera based on the player's movement
        freeLookCamera.m_XAxis.Value += Time.deltaTime * orbitSpeed;

        // Apply the rotation to the camera
        freeLookCamera.transform.rotation = lookRotation;
    }
}
