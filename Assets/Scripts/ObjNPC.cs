using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

enum NPCState
{
    Idle,
    Walking,
    WaitingAtCounter,
    Talking
}

public class ObjNPC : MonoBehaviour
{
    [Header("NPC Behavior Settings")]
    [SerializeField] public Vector3 targetPosition = new Vector3(0f, 0f, 0f);
    [SerializeField] public Transform targetTransform;
    [SerializeField] public float waitTimeAtCounter = 2.0f; //Time in seconds the NPC will wait before leaving
    [SerializeField] public float itemPrice = 10.0f; // The price the NPC is willing to pay or sell for
    [SerializeField] public bool isBuyer = true; // Is this NPC buying or selling? (Determines dialogue)

    [SerializeField] public Dictionary<string, string> dialogueLines = new Dictionary<string, string>()
    {
        {"Buying", "This thing looks pretty cool... I'll take it."},
        {"Selling", "Can I have this? It reminds me of how much I hate my mother."},
        {"Price Raised", "Okay, I guess I can pay a bit more."},
        {"Price Lowered", "You make a fair point. I'll give you a better offer."},
        {"Acceptance", "Deal! Here's the money."},
        {"Denial", "No thanks, I changed my mind."}
    };
    
    private Vector3 exitPosition;
    private NPCState currState;
    private NavMeshAgent agent;
    private PatienceTimer patienceTimer;
    private bool timerExpired = false;
    private bool isLeaving = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currState = NPCState.Idle;
        if (targetTransform != null)
        {
            targetPosition = targetTransform.position;
        }
        exitPosition = transform.position;
        agent = GetComponent<NavMeshAgent>();

        patienceTimer = GetComponentInChildren<PatienceTimer>();
        patienceTimer.OnPatienceExpired += () => timerExpired = true;
        patienceTimer.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        StateMachine();
    }

    public void BarterPriceChange()
    {
        // Logic to handle price increase
    }

    private void StateMachine()
    {
        switch(currState)
        {
            case NPCState.Idle:
                // Handle waiting behavior
                if (Vector3.Distance(transform.position, targetPosition) > 0.1f)
                {
                    currState = NPCState.Walking;
                }
                break;
            case NPCState.Walking:
                // Handle walking behavior
                agent.SetDestination(targetPosition);
                if (Vector3.Distance(transform.position, targetPosition) <= 0.1f)
                {
                    if (isLeaving)
                    {
                        Destroy(gameObject); // NPC leaves the scene
                        return;
                    }
                    patienceTimer.gameObject.SetActive(true);
                    patienceTimer.StartCoroutine("StartPatienceTimer", waitTimeAtCounter);
                    currState = NPCState.WaitingAtCounter;
                }
                break;
            case NPCState.WaitingAtCounter:
                // Handle waiting at counter behavior
                if (timerExpired)
                {
                    targetPosition = exitPosition;
                    currState = NPCState.Walking;
                    timerExpired = false;
                    isLeaving = true;
                }
                break;
            case NPCState.Talking:
                // Handle talking behavior
                break;
        }
    }
}
