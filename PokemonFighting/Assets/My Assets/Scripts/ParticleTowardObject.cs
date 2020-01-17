using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ParticleTowardObject : MonoBehaviourPunCallbacks
{
    ParticleSystem particleSystem;
    public float alpha = 0.5f;
    private GameObject target = null;
    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        
    }
    private void Update()
    {
        target = GetComponentInParent<animationPKM>().target;
        if (target == null) return;
        if (particleSystem.isPlaying)
        {
            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleSystem.particleCount];
            var count = particleSystem.GetParticles(particles);

            for (int i = 0; i < count; i++)
            {
                var particle = particles[i];
                particle.position = Vector3.Lerp(particle.position, target.transform.position + new Vector3(0, .7f, 0), alpha);
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
