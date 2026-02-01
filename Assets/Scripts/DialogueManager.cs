using System;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private UiTextBox textBox;
    [SerializeField] private GameObject choicePanel;
    [SerializeField] private UnityEngine.UI.Button barterButton;
    [SerializeField] private UnityEngine.UI.Button acceptButton;
    [SerializeField] private UnityEngine.UI.Button refuseButton;

    private ObjNPC currentNPC;
    private Action onDialogueEnd;
    private bool isWaitingForChoice = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Debug.Log($"Barter button: {barterButton != null}");
        Debug.Log($"Accept button: {acceptButton != null}");
        Debug.Log($"Refuse button: {refuseButton != null}");

        // Set up button listeners
        barterButton.onClick.AddListener(OnBarterClicked);
        acceptButton.onClick.AddListener(OnAcceptClicked);
        refuseButton.onClick.AddListener(OnRefuseClicked);

        // Hide UI at start
        textBox.gameObject.SetActive(false);
        choicePanel.SetActive(false);
    }

    public void StartDialogue(ObjNPC npc, Action onComplete)
    {
        currentNPC = npc;
        onDialogueEnd = onComplete;

        // Unlock cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // Show the text box
        textBox.gameObject.SetActive(true);
        
        // Display initial dialogue based on whether NPC is buying or selling
        string initialLine = npc.isBuyer ? npc.dialogueLines["Buying"] : npc.dialogueLines["Selling"];
        textBox.DisplayText(initialLine);
        ShowChoices();
    }

    private void ShowChoices()
    {
        isWaitingForChoice = true;
        choicePanel.SetActive(true);
    }

    private void HideChoices()
    {
        isWaitingForChoice = false;
        choicePanel.SetActive(false);
    }

    private void OnBarterClicked()
    {
        HideChoices();
        
        // Trigger NPC's barter logic
        currentNPC.BarterPriceChange();
        
        // Show NPC's response to bartering
        string response = currentNPC.dialogueLines.ContainsKey("Price Raised") 
            ? currentNPC.dialogueLines["Price Raised"] 
            : "Okay, I guess I can adjust my offer.";
        
        textBox.DisplayText(response, () => ShowChoices());
    }

    private void OnAcceptClicked()
    {
        HideChoices();
        
        // Show acceptance dialogue
        string acceptance = currentNPC.dialogueLines.ContainsKey("Acceptance") 
            ? currentNPC.dialogueLines["Acceptance"] 
            : "Deal!";
        
        textBox.DisplayText(acceptance, () => EndDialogue(true));
    }

    private void OnRefuseClicked()
    {
        HideChoices();
        
        // Show denial dialogue
        string denial = currentNPC.dialogueLines.ContainsKey("Denial") 
            ? currentNPC.dialogueLines["Denial"] 
            : "No thanks, I changed my mind.";
        
        textBox.DisplayText(denial, () => EndDialogue(false));
    }

    private void EndDialogue(bool dealAccepted)
    {
        // Hide UI
        textBox.gameObject.SetActive(false);
        choicePanel.SetActive(false);
        
        // Handle deal outcome
        if (dealAccepted)
        {
            // TODO: Handle money transaction, inventory changes, etc.
            Debug.Log($"Deal accepted! Price: {currentNPC.itemPrice}");
        }
        else
        {
            Debug.Log("Deal refused.");
        }

        // Lock cursor back
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Call completion callback
        onDialogueEnd?.Invoke();
        currentNPC = null;
    }

    public void DisplayMessage(string message, Action onComplete = null)
    {
        textBox.gameObject.SetActive(true);
        textBox.DisplayText(message, () => 
        {
            textBox.gameObject.SetActive(false);
            onComplete?.Invoke();
        });
    }
}
