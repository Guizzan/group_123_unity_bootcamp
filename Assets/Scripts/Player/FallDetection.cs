using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallDetection : MonoBehaviour
{
    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        player = GameObject.FindWithTag("Player");
        
        if(player.transform.position.y < -10)
        {
            player.transform.position = new Vector3(
                PlayerPrefs.GetFloat("CheckPointX"), 
                PlayerPrefs.GetFloat("CheckPointY"), 
                PlayerPrefs.GetFloat("CheckPointZ"));
            player.GetComponent<CharacterController>().enabled = false;
            player.GetComponent<CharacterController>().enabled = true;
        }
    }
}
