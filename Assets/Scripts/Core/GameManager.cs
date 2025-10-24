/* TO-DOS:
 * 1. Player Animations [DONE]
 * 2. Core Gameplay Mechanics [DONE]
 * 3. Main Menu and Game Over Screens [DONE]
 * 4. BGM & SFX
 * 5. GUI Improvements
 * PUBLISHING
 * 
 * Update:
 * 6. 2nd Player
 * 7. Pick Characters
 * 8. Difficulty pick:
 * -- For AI
 * -- For Questions
*/

// ============== IMPORTS  ==================
using System.Collections;
using TMPro; // TextMeshPro
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; // Required for the new Input System
using UnityEngine.SceneManagement; 

public class GameManager : MonoBehaviour
{
    // ============== VARIABLES  ==================
    [Header("UI Elements")]
    public TMP_Text questionText;

    public TMP_Text resultText;

    public Slider playerHealth;
    public Slider enemyHealth;

    public TMP_Text choiceAText; // Choices
    public TMP_Text choiceBText;
    public TMP_Text choiceCText;
    public TMP_Text choiceDText;

    public GameObject gameOverPanel; // UI panel for "Game Over" message

    [Header("Audio")]
    public AudioSource sfxSource;
    public AudioClip kickSound;
    public AudioClip hitSound;
    public AudioClip missSound; // find other sound kay pang lose jud ni sya
    public AudioClip winSound;

    [Header("Animators")]
    public Animator playerAnimator;
    public Animator enemyAnimator;

    [Header("AI Settings")]
    public bool isAI = true;       // true = Player 2 is AI
    public int aiDifficulty = 2;   // 1 = Easy, 2 = Medium (Default), 3 = Hard

    // Internal variables
    private int correctAnswer;
    private int questionNumber = 0;
    private bool questionActive = false;
    private bool gameOver = false;

    private int choiceA, choiceB, choiceC, choiceD;

    private Coroutine aiRoutine;

    // ============== CORE GAME  ==================
    void Start()
    {
        resultText.text = ""; // wla pay result sa start
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        StartNewRound();
    }

    // naa dri tanan: input sa players
    void Update() 
    {
        if (!questionActive || gameOver) return;

        // Player 1 input (W/A/S/D)
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.qKey.wasPressedThisFrame) CheckAnswer(choiceA, true);
        if (keyboard.aKey.wasPressedThisFrame) CheckAnswer(choiceB, true);
        if (keyboard.wKey.wasPressedThisFrame) CheckAnswer(choiceC, true);
        if (keyboard.sKey.wasPressedThisFrame) CheckAnswer(choiceD, true);

        // Player 2 input (Arrow keys) if not AI
        if (!isAI)
        {
            if (keyboard.uKey.wasPressedThisFrame) CheckAnswer(choiceA, false);
            if (keyboard.jKey.wasPressedThisFrame) CheckAnswer(choiceB, false);
            if (keyboard.iKey.wasPressedThisFrame) CheckAnswer(choiceC, false);
            if (keyboard.kKey.wasPressedThisFrame) CheckAnswer(choiceD, false);
        }
    }
    void StartNewRound()
    {

        if (gameOver) return; // stop if game ended

        // stop any leftover AI coroutine from previous round
        if (aiRoutine != null)
        {
            StopCoroutine(aiRoutine);
            aiRoutine = null;
        }

        questionNumber++; // count each round. Start 0

        // Generate a simple math question (addition for now)
        int a = Random.Range(1, 10);
        int b = Random.Range(1, 10);
        correctAnswer = a + b;
        questionText.text = $"{questionNumber}.  {a} + {b} = ?";

        // Generate 4 random choices including the correct answer
        choiceA = correctAnswer;
        choiceB = Random.Range(1, 20);
        choiceC = Random.Range(1, 20);
        choiceD = Random.Range(1, 20);

        // Shuffle para dli permi A ang correct answer
        ShuffleChoices();

        // Update UI
        choiceAText.text = choiceA.ToString();
        choiceBText.text = choiceB.ToString();
        choiceCText.text = choiceC.ToString();
        choiceDText.text = choiceD.ToString();

        questionActive = true;

        // start na ang think if AI, base sa difficulty
        if (isAI)
            aiRoutine = StartCoroutine(AIAnswer());
    }

    void ShuffleChoices()
    {
        int[] arr = { choiceA, choiceB, choiceC, choiceD };
        for (int i = 0; i < arr.Length; i++)
        {
            int r = Random.Range(0, arr.Length);
            int temp = arr[i];
            arr[i] = arr[r];
            arr[r] = temp;
        }
        choiceA = arr[0];
        choiceB = arr[1];
        choiceC = arr[2];
        choiceD = arr[3];
    }

    void CheckAnswer(int answer, bool isPlayer1)
    {
        if (!questionActive || gameOver) return;

        if (answer == correctAnswer)
        {
            if (isPlayer1)
            {
                resultText.text = "Player 1 hit!"; // result text
                playerAnimator.SetTrigger("Kick"); // trigger kick animation
                enemyHealth.value -= 10; // reduce enemy health
                sfxSource.PlayOneShot(kickSound); // play kick sound
            }
            else
            {
                resultText.text = "Player 2 hit!";
                enemyAnimator.SetTrigger("Kick");
                playerHealth.value -= 10;
                sfxSource.PlayOneShot(kickSound);
            }
        }
        else
        {
            resultText.text = "Miss!";
            sfxSource.PlayOneShot(missSound);
        }

        questionActive = false;
        CheckGameOver(); // check if someone’s health is 0
        if (!gameOver)
            Invoke(nameof(StartNewRound), 1f);
    }

    // ============== AI ENEMY LOGIC  ==================
    float GetAITime()
    {
        switch (aiDifficulty)
        {
            case 1: return Random.Range(5f, 10f); // Easy
            case 2: return Random.Range(3f, 6f); // Medium
            case 3: return Random.Range(1f, 3f); // Hard
        }
        return Random.Range(3f, 6f); // Default to Medium
    }

    IEnumerator AIAnswer()
    {
        float aiTime = GetAITime();
        yield return new WaitForSeconds(aiTime);

        if (!questionActive) yield break;

        resultText.text = "Enemy hit!";
        enemyAnimator.SetTrigger("Kick");
        playerHealth.value -= 10;
        sfxSource.PlayOneShot(kickSound);

        questionActive = false;
        CheckGameOver(); // check if someone’s health is 0
        if (!gameOver)
            Invoke(nameof(StartNewRound), 1f);
    }

    // ============== GAME OVER PANEL  ==================
    void CheckGameOver()
    {
        if (playerHealth.value <= 0)
        {
            EndGame("Player 1 Lost! Enemy Wins!");
            sfxSource.PlayOneShot(missSound);
        }
        else if (enemyHealth.value <= 0)
        {
            EndGame("Player 1 Wins!");
            sfxSource.PlayOneShot(winSound);
        }
    }

    void EndGame(string message)
    {
        gameOver = true;
        resultText.text = "";
        questionText.text = "";
        choiceAText.text = "";
        choiceBText.text = "";
        choiceCText.text = "";
        choiceDText.text = "";

        // Add stop game music
        // Add Win or Lose sound effect

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            var text = gameOverPanel.GetComponentInChildren<TMP_Text>();
            if (text != null) text.text = message;
        }
    }

    public void RetryGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenuScene"); // Load Assets/Scenes/MainMenuScene.unity
    }
}
