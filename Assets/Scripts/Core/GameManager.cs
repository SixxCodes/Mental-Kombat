using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; // Required for the new Input System

public class GameManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text questionText;
    public TMP_Text resultText;
    public Slider playerHealth;
    public Slider enemyHealth;
    public TMP_Text choiceAText;
    public TMP_Text choiceBText;
    public TMP_Text choiceCText;
    public TMP_Text choiceDText;

    [Header("Animators")]
    public Animator playerAnimator;
    public Animator enemyAnimator;

    [Header("AI Settings")]
    public bool isAI = true;       // true = Player 2 is AI
    public int aiDifficulty = 1;   // 1 = Easy, 2 = Medium, 3 = Hard

    private int correctAnswer;
    private bool questionActive = false;

    private int choiceA, choiceB, choiceC, choiceD;

    void Start()
    {
        resultText.text = "";
        StartNewRound();
    }

    void Update()
    {
        if (!questionActive) return;

        // ✅ Player 1 input (W/A/S/D)
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
        // Generate a simple math question
        int a = Random.Range(1, 10);
        int b = Random.Range(1, 10);
        correctAnswer = a + b;

        questionText.text = $"{a} + {b} = ?";

        // Generate 4 random choices including the correct answer
        choiceA = correctAnswer;
        choiceB = Random.Range(1, 20);
        choiceC = Random.Range(1, 20);
        choiceD = Random.Range(1, 20);

        // Shuffle choices (optional)
        ShuffleChoices();

        // Update UI
        choiceAText.text = choiceA.ToString();
        choiceBText.text = choiceB.ToString();
        choiceCText.text = choiceC.ToString();
        choiceDText.text = choiceD.ToString();

        questionActive = true;

        if (isAI)
            StartCoroutine(AIAnswer());
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
        if (!questionActive) return;

        if (answer == correctAnswer)
        {
            if (isPlayer1)
            {
                resultText.text = "Player 1 hit!";
                playerAnimator.SetTrigger("Kick");
                enemyHealth.value -= 10;
            }
            else
            {
                resultText.text = "Player 2 hit!";
                enemyAnimator.SetTrigger("Kick");
                playerHealth.value -= 10;
            }
        }
        else
        {
            resultText.text = "Miss!";
        }

        questionActive = false;
        Invoke(nameof(StartNewRound), 2f);
    }

    IEnumerator AIAnswer()
    {
        float aiTime = GetAITime();
        yield return new WaitForSeconds(aiTime);

        if (!questionActive) yield break;

        resultText.text = "Enemy hit!";
        enemyAnimator.SetTrigger("Kick");
        playerHealth.value -= 10;
        questionActive = false;
        Invoke(nameof(StartNewRound), 2f);
    }

    float GetAITime()
    {
        switch (aiDifficulty)
        {
            case 1: return Random.Range(5f, 10f);
            case 2: return Random.Range(3f, 6f);
            case 3: return Random.Range(1f, 3f);
        }
        return Random.Range(3f, 6f);
    }
}
