using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ParticleTowardObject : MonoBehaviourPunCallbacks
{
    ParticleSystem particleSystem;
    float alpha = 0.5f;
    public GameObject target;
    void Start()
    {
        target = GameObject.Find("bellossom");
        particleSystem = GetComponent<ParticleSystem>();
        
    }
    private void Update()
    {
        if (particleSystem.isPlaying)
        {
            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleSystem.particleCount];
            var count = particleSystem.GetParticles(particles);

            for (int i = 0; i < count; i++)
            {
                var particle = particles[i];
                particle.position = Vector3.Lerp(particle.position, target.transform.position + new Vector3(0, .7f, 0), alpha);
                Debug.Log(particle.position);
                particles[i] = particle;
            }
            particleSystem.SetParticles(particles, count);
        }

        
    }

    public void Play()
    {
        particleSystem.Play();
    }

}
