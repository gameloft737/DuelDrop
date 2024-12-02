using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundUI : MonoBehaviour
{
    [SerializeField] private RoundSettings roundSettings; // Reference to round settings
    [SerializeField] private GameObject roundIconPrefab; // Prefab for round icons
    [SerializeField] private Text roundTimer; // Timer text (optional for display)
    public static RoundUI instance;

    private GameObject[] icons; // Array to store instantiated icons
    private void Awake()
    {
        if(instance == null){
            instance = this;
        }
        else{
            Destroy(gameObject);
            return;
        }   
    }
    private void Start()
    {
        roundSettings = RoundsManager.instance.roundSettings;

        // Initialize arrays based on the number of rounds
        icons = new GameObject[roundSettings.rounds];

        // Get parent RectTransform width
        RectTransform parentRect = GetComponent<RectTransform>();
        float parentWidth = parentRect.rect.width;

        // Calculate spacing between icons
        float spacing = parentWidth / (icons.Length + 1);

        // Spawn icons evenly distributed across the parent container
        for (int i = 0; i < icons.Length; i++)
        {
            // Instantiate the icon as a child of the parent object
            GameObject icon = Instantiate(roundIconPrefab, transform);

            // Set the icon's position in local space
            RectTransform iconRect = icon.GetComponent<RectTransform>();
            float xPosition = spacing * (i + 1) - parentWidth / 2; // Center icons
            iconRect.anchoredPosition = new Vector2(xPosition, 0); // Adjust the y-position as needed

            // Store the instantiated icon
            icons[i] = icon;
        }
    }

    public void ChangeColors(string winner, int roundNumber)
    {

            if (icons[roundNumber] != null)
            {
                RawImage iconImage = icons[roundNumber].GetComponent<RawImage>();
                if (iconImage != null)
                {
                    iconImage.color = winner == "WASD" ? Color.blue : Color.red;
                }
            }
    }
    public static String GetColorName(string winner){
        return winner == "WASD" ? "Blue" : "Red";
    }
    
}
