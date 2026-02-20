using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class WordGuessGame : MonoBehaviour
{
    [Header("Game Data")]
    public string correctWord;
    public string[] words;
    public string lettersGuessed = "";
    public int attemptsRemaining = 3;

    [Header("UI References (TextMeshPro)")]
    public TMP_Text wordText;
    public TMP_Text attemptsText;
    public TMP_Text guessedText;
    public TMP_InputField guessInput;

    private bool gameOver = false;

    
    void Start()
    {
        LoadWords();
        ResetGame();
    }

    // Read from a text file and put all words into an array.
    public void LoadWords()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "words.txt");

        if (!File.Exists(path))
        {
            Debug.LogError("words.txt not found at: " + path);
            words = new string[] { "STUFF", "UNITY", "PUZZLE" }; // fallback
            return;
        }

        // Read all lines, trim, ignore blanks
        List<string> list = new List<string>();
        foreach (var line in File.ReadAllLines(path))
        {
            string w = line.Trim();
            if (!string.IsNullOrEmpty(w))
                list.Add(w.ToUpper());
        }

        words = list.ToArray();

        if (words.Length == 0)
        {
            Debug.LogError("words.txt is empty!");
            words = new string[] { "STUFF", "UNITY", "PUZZLE" }; // fallback
        }
    }

    // Pick a random word from an array of words, and it becomes the correct word.
    public void ChooseWord()
    {
        correctWord = words[Random.Range(0, words.Length)].ToUpper();
    }

    // Should call ChooseWord, set attemptsRemaining to 3, remove all previous guesses
    public void ResetGame()
    {
        ChooseWord();
        attemptsRemaining = 3;
        lettersGuessed = "";
        gameOver = false;

        guessInput.text = "";
        guessInput.interactable = true;

        UpdateText();
    }

    // Called when the submit button is pressed
    public void SubmitGuess()
    {
        if (gameOver) return;

        string raw = guessInput.text.Trim().ToUpper();

        // Ensure exactly 1 character
        if (raw.Length != 1)
        {
            guessInput.text = "";
            return;
        }

        char guessedChar = raw[0];

        // Only allow A-Z letters (optional but helpful)
        if (guessedChar < 'A' || guessedChar > 'Z')
        {
            guessInput.text = "";
            return;
        }

        // If already guessed, ignore
        if (lettersGuessed.Contains(guessedChar.ToString()))
        {
            guessInput.text = "";
            return;
        }

        lettersGuessed += guessedChar;

        // If wrong guess, lose an attempt
        if (!correctWord.Contains(guessedChar.ToString()))
        {
            attemptsRemaining--;
        }

        guessInput.text = "";
        UpdateText();
        CheckWinLose();
    }

    // Update onscreen text and show guessed letters
    public void UpdateText()
    {
        // Build display like S--FF
        char[] display = new char[correctWord.Length];
        for (int i = 0; i < correctWord.Length; i++)
        {
            char c = correctWord[i];
            display[i] = lettersGuessed.Contains(c.ToString()) ? c : '-';
        }

        wordText.text = new string(display);
        attemptsText.text = "Attempts Remaining: " + attemptsRemaining;

        if (guessedText != null)
            guessedText.text = "Guessed: " + string.Join(" ", lettersGuessed.ToCharArray());
    }

    private void CheckWinLose()
    {
        // Win if no dashes remain
        if (!wordText.text.Contains("-"))
        {
            gameOver = true;
            attemptsText.text = "You win! Press Reset to play again.";
            guessInput.interactable = false;
            return;
        }

        // Lose if attempts hit 0
        if (attemptsRemaining <= 0)
        {
            gameOver = true;
            wordText.text = correctWord; // reveal the word
            attemptsText.text = "You lose! The word was " + correctWord + ". Press Reset.";
            guessInput.interactable = false;
        }
    }
}

