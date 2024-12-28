using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowProjector : MonoBehaviour
{
    private Transform shadow;
    private RaycastHit hit;
    private float offset = 0.1f; // Height adjustment offset for the shadow
    public LayerMask groundLayer; // Assign the "Ground" layer in the Inspector

    private void Start()
    {
        if (shadow == null)
        {
            shadow = transform.GetChild(0); // Assuming the shadow is the first child
        }
        offset = EnvironmentManager.instance.platformOffset;
    }

   private Vector3 smoothShadowPosition;

    private void Update()
    {
        Ray downRay = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(downRay, out hit, Mathf.Infinity, groundLayer))
        {
            // Smoothly interpolate shadow position
            Vector3 targetPosition = hit.point + Vector3.up * offset;
            smoothShadowPosition = Vector3.Lerp(smoothShadowPosition, targetPosition, 0.2f);
            shadow.transform.position = smoothShadowPosition;
        }
    }


}
