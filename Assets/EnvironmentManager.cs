using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    public static EnvironmentManager instance;
    public float height = 20f;
    [SerializeField]private GameObject basePlatform;
    public Transform[] platforms;

    [SerializeField]private LayerMask groundLayer;
    private void Awake()
    {
        if(instance == null){
            instance = this;
        }
        else{
            Destroy(gameObject);
            return;
        }
        Transform[] allTransforms = GetComponentsInChildren<Transform>();
        List<Transform> childTransforms = new List<Transform>();

        foreach (Transform t in allTransforms)
        {
            if (t != transform && ((1 << t.gameObject.layer) & groundLayer) != 0) // Exclude the parent Transform
            {
                childTransforms.Add(t);
            }
        }

        platforms = childTransforms.ToArray(); // Convert back to an array if needed
    }

    public float GetBaseLength(float leeway = 0)
    {
        BoxCollider boxCollider = basePlatform.GetComponent<BoxCollider>();
        if (boxCollider != null)
        { 
            return boxCollider.bounds.size.x - leeway;
        }
        return 0;
    }
    public float GetLength(Transform platform)
    {
        BoxCollider boxCollider = platform.GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            return boxCollider.bounds.size.x;
        }
        if (boxCollider == null || !boxCollider.enabled || !platform.gameObject.activeInHierarchy)
        {
            Debug.LogWarning("BoxCollider is enabled:"+ boxCollider.enabled);
            Debug.LogWarning("BoxCollider is active:"+ platform.gameObject.activeInHierarchy);
        }
        return 0;
    }
    public Transform GetRandomPlatform()
    {
        return platforms[UnityEngine.Random.Range(0, platforms.Length)];
    }
}
