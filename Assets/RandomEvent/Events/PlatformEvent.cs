using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlatformEvent : EventAction
    {
    [SerializeField] GameObject effect;
    [SerializeField] Transform environment;
    [SerializeField] int count; // The number of platforms to select.
    [SerializeField] List<GameObject> platforms = new List<GameObject>();

    protected override IEnumerator CreateEvent()
    {
        environment = GameObject.FindGameObjectWithTag("Environment").transform;
        for (int i = 0; i < count; i++)
        {
            GameObject selectedPlatform = environment.GetChild(UnityEngine.Random.Range(0, environment.childCount)).gameObject;
            ParticleSystem pEffect = Instantiate(effect, selectedPlatform.transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
            var shape = pEffect.shape;

            // Get the x size (width) of the selected platform
            float platformWidth = selectedPlatform.GetComponent<Renderer>().bounds.size.x;

            // Set the radius of the particle system shape to match the platform width
            shape.radius = platformWidth / 2f; // Divide by 2 if you want the radius to cover the entire width of the platform.
            platforms.Add(selectedPlatform);
        }
        StartCoroutine(FadeAfterDelay());
        yield break;
    }

    private IEnumerator FadeAfterDelay()
    {
        
        yield return new WaitForSeconds(2.5f);

        // Deactivate each platform after the delay.
        foreach (var platform in platforms)
        {
            platform.SetActive(false);
        }

        yield return new WaitForSeconds(3f);
        
        foreach (var platform in platforms)
        {
            platform.SetActive(true);
        }
        Destroy(gameObject);
    }

}
