using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MovementMessage : Message
{
    [Header("Input Actions")]
    [SerializeField] private InputAction wAction;
    [SerializeField] private InputAction aAction;
    [SerializeField] private InputAction dAction;
    [SerializeField] private InputAction upAction;
    [SerializeField] private InputAction leftAction;
    [SerializeField] private InputAction rightAction;

    [Header("Key Images")]
    [SerializeField] private Image wKeyImage;
    [SerializeField] private Image aKeyImage;
    [SerializeField] private Image dKeyImage;
    [SerializeField] private Image upArrowImage;
    [SerializeField] private Image leftArrowImage;
    [SerializeField] private Image rightArrowImage;

    private Dictionary<InputAction, bool> keyPressedStates;

    private void Awake()
    {
        // Initialize keyPressedStates dictionary
        keyPressedStates = new Dictionary<InputAction, bool>
        {
            { wAction, false },
            { aAction, false },
            { dAction, false },
            { upAction, false },
            { leftAction, false },
            { rightAction, false }
        };

        // Enable input actions and subscribe to performed events
        foreach (var action in keyPressedStates.Keys)
        {
            action.Enable();
            action.performed += OnKeyPressed;
        }

        ResetVisuals();
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
        InputAction action = context.action;

        if (keyPressedStates.ContainsKey(action))
        {
            keyPressedStates[action] = true;
            UpdateVisual(action);
            CheckCompletion();
        }
    }

    private void UpdateVisual(InputAction action)
    {
        // Update the color of the associated image to green
        if (action == wAction) wKeyImage.color = Color.green;
        if (action == aAction) aKeyImage.color = Color.green;
        if (action == dAction) dKeyImage.color = Color.green;
        if (action == upAction) upArrowImage.color = Color.green;
        if (action == leftAction) leftArrowImage.color = Color.green;
        if (action == rightAction) rightArrowImage.color = Color.green;
    }

    private void ResetVisuals()
    {
        // Reset all key images to their default color (white)
        wKeyImage.color = Color.white;
        aKeyImage.color = Color.white;
        dKeyImage.color = Color.white;
        upArrowImage.color = Color.white;
        leftArrowImage.color = Color.white;
        rightArrowImage.color = Color.white;
    }

    private void CheckCompletion()
    {
        // Check if all actions have been performed
        foreach (var state in keyPressedStates.Values)
        {
            if (!state) return;
        }

        // If all keys are pressed, set isComplete to true and log a message
        isComplete = true;
        Debug.Log("All keys pressed! isComplete = true");
        Invoke(nameof(CompleteMessage), 2f); // Wait 2 seconds before marking the message as complete
    }

    private void CompleteMessage()
    {
        Debug.Log("Message completed after 2 seconds.");
    }
    [SerializeField] Animator warner;
    public override void Warn(){
        warner.SetTrigger("warn");
    }
}
