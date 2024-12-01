using System.Collections;
using UnityEngine;

public class MoveVelocity : MonoBehaviour
{
    public Vector3 velocity = new Vector3(0, 1, 0); // Default to moving up along the Y-axis
    public float duration = 0; // 0 means infinite duration

    [SerializeField]private bool isMoving = false;
    private float elapsedTime = 0;

    void Start()
    {
        // Start the movement when the script is loaded.
        StartMovement();
    }

    public void StartMovement()
    {
        isMoving = true;
        elapsedTime = 0;
        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        while (isMoving)
        {
            // Move the object based on the velocity and time.
            transform.position += velocity * Time.deltaTime;

            // If duration is greater than 0, track elapsed time.
            if (duration > 0)
            {
                elapsedTime += Time.deltaTime;
                if (elapsedTime >= duration)
                {
                    isMoving = false;
                }
            }

            yield return null; // Wait for the next frame.
        }
    }
}
