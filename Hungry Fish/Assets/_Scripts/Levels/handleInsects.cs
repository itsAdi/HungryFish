using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class handleInsects : MonoBehaviour {
	private ParticleSystem insectParticles;

	private List<ParticleSystem.Particle> enter = new List<ParticleSystem.Particle>();
	void OnEnable(){
		insectParticles = GetComponent<ParticleSystem>();
	}

	void OnParticleTrigger(){
		int totalInsects = insectParticles.GetTriggerParticles(ParticleSystemTriggerEventType.Inside, enter);
		for (int i = 0; i < totalInsects; i++)
		{
			ParticleSystem.Particle p = enter[i];
			persistentData.Instance.increaseScore(10);
			p.remainingLifetime = 0f;
			enter[i] = p;
		}
		insectParticles.SetTriggerParticles(ParticleSystemTriggerEventType.Inside, enter);
	}
}
