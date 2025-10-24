// This code is only for animation testing of AI Enemy

using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyAIAnimation : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Keyboard.current.kKey.wasPressedThisFrame)
        {
            Debug.Log("Enemy: K key pressed!"); // Debug statement
            animator.SetTrigger("Kick"); // Trigger kick animation
        }
    }
}
