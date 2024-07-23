using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TramsformRotator : MonoBehaviour
{
    public float speed;
    void FixedUpdate()
    {
        transform.Rotate(0, speed * Time.fixedDeltaTime, 0);
    }
}