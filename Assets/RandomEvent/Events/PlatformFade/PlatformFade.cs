using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlatformFade : EventAction
    {
    [SerializeField] GameObject effect;
    [SerializeField] Transform environment;
    [SerializeField] int count; // The number of platforms to select.
    [SerializeField] List<Transform> platforms = new List<Transform>();

    
    public override void EventTrigger(){
        for (int i = 0; i < count; i++)
        {
            Transform selectedPlatform = EnvironmentManager.instance.GetRandomPlatform();
            ParticleSystem pEffect = Instantiate(effect, selectedPlatform.transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
            var shape = pEffect.shape;

            // Get the x size (width) of the selected platform
            float platformWidth = EnvironmentManager.instance.GetLength(selectedPlatform);
            Debug.Log(platformWidth);
            shape.radius = platformWidth / 2f; // Divide by 2 if you want the radius to cover the entire width of the platform.
            platforms.Add(selectedPlatform);
        }
        StartCoroutine(FadeAfterDelay());
    }

    private IEnumerator FadeAfterDelay()
    {
        
        yield return new WaitForSeconds(2.5f);

        // Deactivate each platform after the delay.
        foreach (var platform in platforms)
        {
            platform.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(3f);
        
        foreach (var platform in platforms)
        {
            platform.gameObject.SetActive(true);
        }
        Destroy(gameObject);
    }

}
