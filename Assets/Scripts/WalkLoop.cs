using UnityEngine;

public class WalkLoop : MonoBehaviour
{
    public Animator animator;
    public float walkSpeed = 2f;
    public float walkDuration = 5f;

    private float timer = 0f;
    private bool facingForward = true;

    void Update()
    {
        // Always play walk animation
        animator.SetBool("isWalking", true);

        // Move forward
        transform.Translate(Vector3.forward * walkSpeed * Time.deltaTime);

        // Count time
        timer += Time.deltaTime;

        if (timer >= walkDuration)
        {
            // Rotate 180° around Y axis
            transform.Rotate(0f, 180f, 0f);

            // Reset timer
            timer = 0f;

            // Flip direction flag
            facingForward = !facingForward;
        }
    }
}
