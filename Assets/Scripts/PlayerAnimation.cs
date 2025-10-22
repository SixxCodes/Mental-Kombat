using UnityEngine;
using UnityEngine.InputSystem; // Required for the new Input System

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Check if Space key was pressed this frame using the new Input System
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Debug.Log("Space pressed!"); // Debug statement
            animator.SetTrigger("Kick"); // Trigger kick animation
        }
    }
}
