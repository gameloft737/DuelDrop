using UnityEngine;

public class CharacterCycler : MonoBehaviour
{
    public bool isLeftPlayer; // Set this true for the left player, false for the right player
    private Transform[] characters;
    public int currentIndex;

    void Start()
    {
        // Initialize characters array with child characters
        characters = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            characters[i] = transform.GetChild(i);
            characters[i].gameObject.SetActive(i == 0); // Set only the first character active
        }
        currentIndex = 0;
    }

    void Update()
    {
        // Determine key input based on the player side
        if (isLeftPlayer)
        {
            if (Input.GetKeyDown(KeyCode.A)) CycleCharacter(-1); // Cycle left
            if (Input.GetKeyDown(KeyCode.D)) CycleCharacter(1);  // Cycle right
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow)) CycleCharacter(-1); // Cycle left
            if (Input.GetKeyDown(KeyCode.RightArrow)) CycleCharacter(1); // Cycle right
        }
    }

    void CycleCharacter(int direction)
    {
        // Hide the current character
        characters[currentIndex].gameObject.SetActive(false);

        // Update the index and wrap around
        currentIndex = (currentIndex + direction + characters.Length) % characters.Length;

        // Show the new character
        characters[currentIndex].gameObject.SetActive(true);
    }
    public void CycleRight(){
        CycleCharacter(1);
    }
    public void CycleLeft(){
        CycleCharacter(-1);
    }
}
