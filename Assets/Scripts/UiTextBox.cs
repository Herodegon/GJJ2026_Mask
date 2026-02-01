using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class UiTextBox : MonoBehaviour
{
    [Header("Text Settings")]
    [SerializeField] private TextMeshProUGUI textDisplay;
    [SerializeField] private float typewriterSpeed = 0.05f; // Time between each character
    [SerializeField] private bool useTypewriterEffect = true;

    [Header("Continue Prompt")]
    [SerializeField] private GameObject continuePrompt; // Optional "Press [Space] to continue" text

    private Coroutine typewriterCoroutine;
    private Action onTextComplete;
    private bool isTyping = false;
    private bool canContinue = false;
    private string currentFullText = "";
    private InputAction continueAction;

    void Start()
    {
        if (continuePrompt != null)
        {
            continuePrompt.SetActive(false);
        }

        continueAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/space");
        continueAction.AddBinding("<Keyboard>/enter");
        continueAction.AddBinding("<Keyboard>/e");
        continueAction.AddBinding("<mouse>/leftButton");
        continueAction.Enable();

        continueAction.performed += ctx =>
        {
            if (isTyping)
            {
                // Skip typewriter effect and show full text
                SkipTypewriter();
            }
            else if (canContinue)
            {
                // Continue to next dialogue/action
                Continue();
            }
        };
    }

    public void DisplayText(string text, Action onComplete = null)
    {
        currentFullText = text;
        onTextComplete = onComplete;
        canContinue = false;

        if (continuePrompt != null)
        {
            continuePrompt.SetActive(false);
        }

        // Stop any existing typewriter effect
        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
        }

        if (useTypewriterEffect)
        {
            typewriterCoroutine = StartCoroutine(TypewriterEffect(text));
        }
        else
        {
            textDisplay.text = text;
            OnTypingComplete();
        }
    }

    private IEnumerator TypewriterEffect(string text)
    {
        isTyping = true;
        textDisplay.text = "";

        foreach (char letter in text)
        {
            textDisplay.text += letter;
            yield return new WaitForSeconds(typewriterSpeed);
        }

        OnTypingComplete();
    }

    private void SkipTypewriter()
    {
        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
        }
        textDisplay.text = currentFullText;
        OnTypingComplete();
    }

    private void OnTypingComplete()
    {
        isTyping = false;
        canContinue = true;

        if (continuePrompt != null)
        {
            continuePrompt.SetActive(true);
        }
    }

    private void Continue()
    {
        canContinue = false;
        
        if (continuePrompt != null)
        {
            continuePrompt.SetActive(false);
        }

        onTextComplete?.Invoke();
    }

    public void ClearText()
    {
        textDisplay.text = "";
        if (continuePrompt != null)
        {
            continuePrompt.SetActive(false);
        }
    }

    public void SetTypewriterSpeed(float speed)
    {
        typewriterSpeed = speed;
    }
}
