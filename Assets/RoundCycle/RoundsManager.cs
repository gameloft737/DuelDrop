using System;
using System.Collections;
using TMPro;
using Unity.Play.Publisher.Editor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoundsManager : MonoBehaviour
{
    public static RoundsManager instance;
    public RoundSettings roundSettings;
    private PlayerMovement arrowKeyPlayer;
    private PlayerMovement WASDPlayer;
    private WeaponManager arrowKeyManager;
    private WeaponManager WASDManager;
    //ui
    public GameObject loadScreen;
    public ParticleSystem sparkles;
    public Animator animator; 
    public Text roundEnd;
    //ui
    public Round[] rounds;
    int currentRoundNum;
    Round currentRound;
    private float roundTimeRemaining;
    
    public string endScreen; // The name of the scene to load
    private void Awake()
    {
        if(instance == null){
            instance = this;
        }
        else{
            Destroy(gameObject);
            return;
        }   
        rounds = new Round[roundSettings.rounds];
        for (int i = 0; i < rounds.Length; i++)
        {
            rounds[i] = new Round(); // Create a new Round instance
            rounds[i].duration = roundSettings.roundDuration;
        }

    }

    private void OnEnable()
    {
        // Subscribe to the OnPlayersSpawned event
        PlayerSpawner.OnPlayersSpawned += InitializePlayers;
    }

    private void OnDisable()
    {
        // Unsubscribe from the event when this script is disabled
        PlayerSpawner.OnPlayersSpawned -= InitializePlayers;
    }

    private void InitializePlayers()
    {
        // Initialize player components
        arrowKeyPlayer = GameObject.FindGameObjectWithTag("ArrowKeysPlayer").GetComponent<PlayerMovement>();
        WASDPlayer = GameObject.FindGameObjectWithTag("WASDPlayer").GetComponent<PlayerMovement>();
        
        arrowKeyManager = GameObject.FindGameObjectWithTag("ArrowKeysManager").GetComponent<WeaponManager>();
        WASDManager = GameObject.FindGameObjectWithTag("WASDManager").GetComponent<WeaponManager>();
        

        StartCoroutine(RoundLoop());
    }
    private int arrowKeysWins = 0; // Tracks ArrowKeys player wins
    private int wasdWins = 0; // Tracks WASD player wins

    private IEnumerator RoundLoop()
    {
        int majority = Mathf.CeilToInt(rounds.Length / 2f); // Majority threshold

        for (int i = 0; i < rounds.Length; i++)
        {
            Round round = rounds[i];
            currentRoundNum = i;
            currentRound = round;
            round.isActive = true;

            SetRoundState(Round.RoundState.Load, round);
            loadScreen.SetActive(true);
            EventCreation.instance.isFrozen = true;
            yield return new WaitForSeconds(2f);
            EventCreation.instance.isFrozen = false;
            loadScreen.SetActive(false);
            SetRoundState(Round.RoundState.Play, round);

            roundTimeRemaining = roundSettings.roundDuration;
            while (roundTimeRemaining > 0)
            {
                if (!round.isActive)
                {
                    break; // Exit if the round is no longer active
                }
                RoundUI.instance.SetTimer(roundTimeRemaining);
                roundTimeRemaining -= Time.deltaTime;
                yield return null; // Wait for the next frame
            }

            // End the round
            SetRoundState(Round.RoundState.Finish, round);
            if (round.isActive)
            {
                if (WASDManager.healthSystem.health > arrowKeyManager.healthSystem.health)
                {
                    round.winner = "WASD";
                    wasdWins++; // Increment WASD wins
                }
                else
                {
                    round.winner = "ArrowKeys";
                    arrowKeysWins++; // Increment ArrowKeys wins
                }
            }
            PlayerSpawner.instance.TeleportPlayer(WASDPlayer.transform, arrowKeyPlayer.transform);
            animator.SetTrigger("end");
            sparkles.Play();
            roundEnd.text = RoundUI.GetColorName(round.winner);

            WASDManager.healthSystem.SetMaxHealth();
            arrowKeyManager.healthSystem.SetMaxHealth();
            EventCreation.instance.isFrozen = true;
            EventCreation.instance.DestroyEvents();

            yield return new WaitForSeconds(2f);
            round.isActive = false;
            RoundUI.instance.SetUI(round.winner);

            // Check if one player has reached the majority
            if (wasdWins >= majority || arrowKeysWins >= majority)
            {
                EndGame(); // End game early if a player has won
                yield break; // Exit the coroutine
            }
        }

        EndGame(); // End game if all rounds are completed without a majority winner
    }

    private void EndGame()
    {
        loadScreen.SetActive(true);
        PlayerSpawner.instance.TeleportPlayer(WASDPlayer.transform, arrowKeyPlayer.transform);

        if (arrowKeysWins > wasdWins)
        {
            PlayerPrefs.SetInt("winner", PlayerPrefs.GetInt("selectedArrowKeys"));
            string name = arrowKeyPlayer.name;
            PlayerPrefs.SetString("winnerString", name.Remove(name.Length - 9));
        }
        else
        {
            PlayerPrefs.SetInt("winner", PlayerPrefs.GetInt("selectedWASD"));
            string name = WASDPlayer.name;
            PlayerPrefs.SetString("winnerString", name.Remove(name.Length - 4));
        }

        SceneManager.LoadScene(endScreen);
    }


    public void DeclareDeath(String winner){
        
        currentRound.isActive = false;
        PlayerSpawner.instance.TeleportPlayer(WASDPlayer.transform,  arrowKeyPlayer.transform);
        if(winner.Equals(arrowKeyPlayer.tag)){  
            currentRound.winner = "WASD";
            wasdWins++;
        }
        else{
            currentRound.winner = "ArrowKeys";
            arrowKeysWins++;
        }
    }

    private void SetRoundState(Round.RoundState newState, Round round)
    {
        if (newState == Round.RoundState.Play)
        {
            // Allow players to move
            arrowKeyPlayer.SetState(false);
            WASDPlayer.SetState(false);
            
            arrowKeyManager.UnfreezeAll();
            WASDManager.UnfreezeAll();
            
        }
        else
        {
            // Freeze players
            arrowKeyPlayer.SetState(true);
            WASDPlayer.SetState(true);

            arrowKeyManager.FreezeAll();
            WASDManager.FreezeAll();
        }
        
        round.currentState = newState;
    }
}