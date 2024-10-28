using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("Sliders Configuration")]
    [SerializeField] private SliderConfig sliderConfig; // Reference to the ScriptableObject-like config

    // Dictionary for fast runtime lookup of sliders
    private Dictionary<string, Dictionary<string, Slider>> playerSliders = new Dictionary<string, Dictionary<string, Slider>>();

    private void Awake()
    {
        // Singleton setup
        if (instance == null)
        {
            instance = this;
            InitializeDictionary();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // Initialize the dictionary from sliderConfig data for fast access
    private void InitializeDictionary()
    {
        foreach (var playerSlider in sliderConfig.playerSliders)
        {
            var sliderDict = new Dictionary<string, Slider>();
            foreach (var sliderData in playerSlider.sliders)
            {
                sliderDict[sliderData.sliderName] = sliderData.slider;
            }
            playerSliders[playerSlider.identifier] = sliderDict;
        }
    }

    // Sets the value of a slider based on identifier and slider name
    public void SetSlider(string identifier, string sliderName, float value)
    {
        if (playerSliders.TryGetValue(identifier, out Dictionary<string, Slider> sliders) &&
            sliders.TryGetValue(sliderName, out Slider slider))
        {
            slider.value = value;
        }
    }

    // Internal ScriptableObject-like configuration class
    [Serializable]
    public class SliderConfig
    {
        public List<PlayerSliderData> playerSliders = new List<PlayerSliderData>();
    }

    // Serializable class to represent each player's set of sliders
    [Serializable]
    public class PlayerSliderData
    {
        public string identifier; // e.g., "Player1", "Player2"
        public List<SliderData> sliders = new List<SliderData>();
    }

    // Serializable class for individual slider data
    [Serializable]
    public class SliderData
    {
        public string sliderName; // e.g., "attack", "specialAttack"
        public Slider slider;     // Reference to the Slider component in the scene
    }
}
