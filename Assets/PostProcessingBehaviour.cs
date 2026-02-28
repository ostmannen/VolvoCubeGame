using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingBehaviour : MonoBehaviour
{
    [SerializeField] private Volume volume;
    private LensDistortion lensDistortion;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        volume.profile.TryGet(out lensDistortion);
    }
    public void LensDistortionChanged(bool initiate)
    {
        StopAllCoroutines();
        if (initiate)
        {
            StartCoroutine(ChangeLensDistortion(0.2f, 10,_defaultLensDist, _flyingLensDist));
        }
        else
        {
            StartCoroutine(ChangeLensDistortion(0.2f, 10, _flyingLensDist, _defaultLensDist));    
        }
      //  lensDistortion.intensity.value = -0.75f;
    }
    private float _defaultLensDist = 0.2f;
    private float _flyingLensDist = -0.75f;

    private IEnumerator ChangeLensDistortion(float timeInSeconds, float timeStep, float start, float end)
    {
        for (float i = 0; i <= timeInSeconds * timeStep; i++)
        {
            float t = i / (timeInSeconds * timeStep);
            lensDistortion.intensity.value = Mathf.Lerp(start, end, t);
            yield return new WaitForSeconds(1 / timeStep);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
