using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody m_Rigidbody;
    private AudioSource audio;
    private AudioClip bounce;

    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        audio = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Paddle")
        {
            // play collision sound - a bounce
            audio.Play();
        }
    }

    private void OnCollisionExit(Collision other)
    {
        var velocity = m_Rigidbody.velocity;
        
        //after a collision we accelerate a bit
        velocity += velocity.normalized * 0.01f;
        
        //check if we are not going totally vertically as this would lead to being stuck, we add a little vertical force
        if (Vector3.Dot(velocity.normalized, Vector3.up) < 0.1f)
        {
            velocity += velocity.y > 0 ? Vector3.up * .5f : Vector3.down * .5f;

            if (UnityEngine.Random.Range(0f,1f) > 0.5f)
            {
                velocity += velocity.x < 0.1 ? Vector3.right * .3f : Vector3.right * .3f; // added as standard project did get stuck vertically!
            }
            else
            {
                velocity += velocity.x < 0.1 ? Vector3.left * .3f : Vector3.left * .3f; // added as standard project did get stuck vertically!
            }
        }

        //max velocity
        if (velocity.magnitude > 3.0f)
        {
            velocity = velocity.normalized * 3.0f;
        }

        m_Rigidbody.velocity = velocity;
        
    }
}
