using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 3))
        {
            if (hit.collider.name == "CheckPoint")
            {
                PlayerPrefs.SetFloat("CheckPointX", hit.transform.position.x);
                PlayerPrefs.SetFloat("CheckPointY", hit.transform.position.y + 1);
                PlayerPrefs.SetFloat("CheckPointZ", hit.transform.position.z);
            }
        }
    }
}
