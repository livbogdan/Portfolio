using UnityEngine;

public class ParticleTimeScaleManager : MonoBehaviour
{
    [SerializeField] private bool useUnscaledTime = true;
    
    private void Start()
    {
        SetParticleTimeScale();
    }
    
    public void SetParticleTimeScale()
    {
        ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();
        
        foreach (ParticleSystem particle in particles)
        {
            var main = particle.main;
            main.useUnscaledTime = useUnscaledTime;
        }
    }
}
