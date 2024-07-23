using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformQueueLerp : MonoBehaviour
{
    public float speed;
    public List<Transform> LerpQueue = new List<Transform>();
    public int index = 0;
    void FixedUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, LerpQueue[index].position, Time.fixedDeltaTime * speed);
        if (Vector3.Distance(transform.position, LerpQueue[index].position) < 0.1f)
        {
            index++;
            if(index >= LerpQueue.Count)
            {
                index = 0;
            }
        }
    }

}
