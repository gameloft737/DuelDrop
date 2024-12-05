using System;
using UnityEngine;
using UnityEngine.UI;

public class RoundUI : MonoBehaviour
{
    [SerializeField] private RoundSettings roundSettings; // Reference to round settings
    [SerializeField] private Slider rightSlider;
    [SerializeField] private GameObject right; // Parent for right-side icons
    [SerializeField] private Slider leftSlider;
    [SerializeField] private GameObject left; // Parent for left-side icons
    [SerializeField] private GameObject roundDividerPrefab; // Prefab for round icons
    [SerializeField] private Text roundTimer; // Timer text (optional for display)
    public static RoundUI instance;

    private GameObject[] leftIcons;  // Array for left-side icons
    private GameObject[] rightIcons; // Array for right-side icons

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        roundSettings = RoundsManager.instance.roundSettings;

        // Set slider max values
        
        int halfRounds = Mathf.CeilToInt(roundSettings.rounds / 2f);
        leftSlider.maxValue = halfRounds;
        rightSlider.maxValue = halfRounds;

        // Initialize arrays for icons
        int iconCount = Mathf.CeilToInt(halfRounds / 2f); // One icon for every two rounds
        leftIcons = new GameObject[iconCount];
        rightIcons = new GameObject[iconCount];

        // Get parent heights
        float leftHeight = left.GetComponent<RectTransform>().rect.height;
        float rightHeight = right.GetComponent<RectTransform>().rect.height;

        // Calculate spacing for vertical distribution
        float leftSpacing = leftHeight / iconCount;
        float rightSpacing = rightHeight / iconCount;

        // Create icons for the left side
        for (int i = 0; i < leftIcons.Length; i++)
        {
            GameObject icon = Instantiate(roundDividerPrefab, left.transform); // Parent to left GameObject
            RectTransform iconRect = icon.GetComponent<RectTransform>();
            iconRect.anchoredPosition = new Vector2(0, leftSpacing * (i + 0.5f) - leftHeight / 2); // Center icon vertically
            leftIcons[i] = icon;
        }

        // Create icons for the right side
        for (int i = 0; i < rightIcons.Length; i++)
        {
            GameObject icon = Instantiate(roundDividerPrefab, right.transform); // Parent to right GameObject
            RectTransform iconRect = icon.GetComponent<RectTransform>();
            iconRect.anchoredPosition = new Vector2(0, rightSpacing * (i + 0.5f) - rightHeight / 2); // Center icon vertically
            rightIcons[i] = icon;
        }
    }

    public void SetUI(string winner)
    {
        if(winner.Equals("WASD"))
        {
            leftSlider.value += 1;
        }
        else{
            rightSlider.value += 1;
        }
    }

    public static string GetColorName(string winner)
    {
        return winner == "WASD" ? "Blue" : "Red";
    }

    public void SetTimer(float roundTimeRemaining)
    {
        int minutes = Mathf.FloorToInt(roundTimeRemaining / 60);
        int seconds = Mathf.FloorToInt(roundTimeRemaining % 60);
        roundTimer.text = $"{minutes:00}:{seconds:00}";
    }
}
