using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundOnCollision : MonoBehaviour
{
    public string TagFilter;
    public string Sound;
    public float DestroyAfter;
    private float timer;
    private bool hit;
    private void Update()
    {
        if (hit) return;
        timer += Time.deltaTime;
        if(timer >= DestroyAfter)
        {
            Destroy(this);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (TagFilter == null)
        {
            hit = true;
            SoundManager.PlaySound(Sound, collision.contacts[0].point);
            return;
        }
        if (collision.gameObject.CompareTag(TagFilter))
        {
            hit = true;
            SoundManager.PlaySound(Sound, collision.contacts[0].point);
        }
    }
}
