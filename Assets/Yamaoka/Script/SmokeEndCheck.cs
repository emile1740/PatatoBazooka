using UnityEngine;
using System.Collections;

public class SmokeEndCheck : MonoBehaviour {
    private ParticleSystem particle;

    // Use this for initialization
    void Start()
    {
        particle = GetComponent<ParticleSystem>();
    }

    void OnEnable()
    {
        particle.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (particle != null && particle.particleCount == 0)
        {
            gameObject.SetActive(false);
        }
    }
}
