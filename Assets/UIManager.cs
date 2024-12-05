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
    [SerializeField] Slider arrowKeysHeath;
    [SerializeField] Slider arrowKeysHeathImg;
    [SerializeField] Slider WASDHeath;
    [SerializeField] Slider WASDHeathImg;
    
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
        value *= 0.5f;
        if (playerSliders.TryGetValue(identifier, out Dictionary<string, Slider> sliders) &&
            sliders.TryGetValue(sliderName, out Slider slider))
        {
            slider.value = value;
        }
        else if(sliderName.Equals("Health")){
            if(identifier.Equals("WASDPlayer")){
                WASDHeath.value = value;
                
            }
            else if(identifier.Equals("ArrowKeysPlayer")){
                arrowKeysHeath.value = value;
            }
    
        }
    }
    public void SetMaxSlider(string identifier, string sliderName, float value){
        if (playerSliders.TryGetValue(identifier, out Dictionary<string, Slider> sliders) &&
            sliders.TryGetValue(sliderName, out Slider slider))
        {
            slider.maxValue = value * 0.5f;
        }
        if(sliderName.Equals("Health")){
            if(identifier.Equals("WASDPlayer")){
                WASDHeath.maxValue = value;
            }
            else if(identifier.Equals("ArrowKeysPlayer")){
                arrowKeysHeath.maxValue = value;
            }
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
