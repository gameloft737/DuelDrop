using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class AttackMessage : Message
{
    [Header("Input Actions")]
    [SerializeField] private InputAction wasdAction;
    [SerializeField] private InputAction arrowAction;
    
    [SerializeField] private WeaponManager wasd;
     [SerializeField] private WeaponManager arrowKeys;

     
    [SerializeField] private HealthSystem wasdHealth;
     [SerializeField] private HealthSystem arrowKeysHealth;

    [Header("Key Images")]
    [SerializeField] private Image arrowImage;
    [SerializeField] private Image wasdImage;
    [SerializeField] private int tutorialNum;

    private Dictionary<InputAction, bool> keyPressedStates;

    private void Awake()
    {
        // Initialize keyPressedStates dictionary
        keyPressedStates = new Dictionary<InputAction, bool>
        {
            { wasdAction, false },
            { arrowAction, false },
        };

        // Enable input actions and subscribe to performed events
        foreach (var action in keyPressedStates.Keys)
        {
            action.Enable();
            action.performed += OnKeyPressed;
        }

        ResetVisuals();
    }
    public override void EnableMessage(){
        wasd.isTutorial = tutorialNum;
        arrowKeys.isTutorial = tutorialNum;
        wasd.UnfreezeAll();
        arrowKeys.UnfreezeAll();
        wasdHealth.isFrozen = false;
        arrowKeysHealth.isFrozen = false;
        messageObj.SetActive(true);
    }
    private void OnDestroy()
    {
        // Disable input actions and unsubscribe from events
        foreach (var action in keyPressedStates.Keys)
        {
            action.performed -= OnKeyPressed;
            action.Disable();
        }
    }

    private void OnKeyPressed(InputAction.CallbackContext context)
    {
        if(!messageObj.activeSelf){return;}
        InputAction action = context.action;

        if (keyPressedStates.ContainsKey(action))
        {
            if(action == wasdAction && wasd.isTutorial == 0){ 
                if(tutorialNum != 3){ 
                    wasd.FreezeAll(); 
                } 
                else if (tutorialNum == 3)
                {
                    wasdHealth.isFrozen = true; 
                    Debug.Log("SIGMA" +wasdHealth.isFrozen); 
                    wasdHealth.SetMaxHealth();
                    
                    arrowKeysHealth.isFrozen = true; 
                    arrowKeysHealth.SetMaxHealth();
                }
                keyPressedStates[action] = true;
                UpdateVisual(action);
                CheckCompletion();
            }
            else if(action == arrowAction && arrowKeys.isTutorial == 0){
                if(tutorialNum != 3){
                    arrowKeys.FreezeAll(); } 
                else if (tutorialNum == 3) 
                {
                    wasdHealth.isFrozen = true; 
                    Debug.Log("SIGMA" +wasdHealth.isFrozen); 
                    wasdHealth.SetMaxHealth();
                    
                    arrowKeysHealth.isFrozen = true; 
                    arrowKeysHealth.SetMaxHealth();
                }
                keyPressedStates[action] = true;
                UpdateVisual(action);
                CheckCompletion();
            }
        }
    }

    private void UpdateVisual(InputAction action)
    {
        // Update the color of the associated image to green
        if (action == wasdAction) wasdImage.color = Color.green;
        if (action == arrowAction) arrowImage.color = Color.green;
    }

    private void ResetVisuals()
    {
        // Reset all key images to their default color (white)
        wasdImage.color = Color.white;
        arrowImage.color = Color.white;
    }

    private void CheckCompletion()
    {
        // Check if all actions have been performed
        foreach (var state in keyPressedStates.Values)
        {
            if (!state) return;
        }
        isComplete = true;
        // If all keys are pressed, set isComplete to true and log a message  
        Debug.Log("All keys pressed! isComplete = true");
        Invoke(nameof(CompleteMessage), 2f); // Wait 2 seconds before marking the message as complete
    }

    private void CompleteMessage()
    {
        proceedObj.SetActive(isComplete);
    }
    [SerializeField] Animator warner;
    public override void Warn(){
        warner.SetTrigger("warn");
    }
}
