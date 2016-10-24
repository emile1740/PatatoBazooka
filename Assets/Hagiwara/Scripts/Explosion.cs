using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

    private ParticleSystem particle;

	// Use this for initialization
	void Start () {
        particle = GetComponent<ParticleSystem>();
    }
	
	// Update is called once per frame
	void Update () {
	    if(particle != null && particle.particleCount == 0) {
            Destroy(gameObject);
        }
	}
}
