using System;
using System.Collections;
using UnityEngine;

public class RoundsManager : MonoBehaviour
{
    public static RoundsManager instance;
    [SerializeField] private RoundSettings roundSettings;
    private PlayerMovement arrowKeyPlayer;
    private PlayerMovement WASDPlayer;
    private WeaponManager arrowKeyManager;
    private WeaponManager WASDManager;
    public GameObject loadScreen;

    public Round[] rounds;
    Round currentRound;
    private float roundTimeRemaining;
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

    private IEnumerator RoundLoop()
    {
        for (int i = 0; i < rounds.Length; i++)
        {
            Debug.Log(i);
            Round round = rounds[i];
            currentRound = round;
            round.isActive = true;
            SetRoundState(Round.RoundState.Load, round);
            loadScreen.SetActive(true);
            yield return new WaitForSeconds(2f);
            loadScreen.SetActive(false);
            SetRoundState(Round.RoundState.Play, round);

            roundTimeRemaining = roundSettings.roundDuration;
            while (roundTimeRemaining > 0)
            {
                if (!round.isActive)
                {
                    break; // Exit if the round is no longer active
                }
                roundTimeRemaining -= Time.deltaTime;
                yield return null; // Wait for the next frame
            }

            // End the round
            SetRoundState(Round.RoundState.Finish, round);
            if(round.isActive){
                if(WASDManager.healthSystem.health > arrowKeyManager.healthSystem.health){
                    round.winner = "WASD";
                }
                else{
                    currentRound.winner = "ArrowKeys";
                }
                PlayerSpawner.instance.TeleportPlayer( WASDPlayer.transform,  arrowKeyPlayer.transform);
            }
            WASDManager.healthSystem.SetMaxHealth();
            arrowKeyManager.healthSystem.SetMaxHealth();
            yield return new WaitForSeconds(2f);
            round.isActive = false;
        }
        EndGame();
    }
    public void DeclareDeath(String winner){
        if(winner.Equals(arrowKeyPlayer.tag)){  
            currentRound.winner = "WASD";
        }
        else{
            currentRound.winner = "ArrowKeys";
        }
        currentRound.isActive = false;
        PlayerSpawner.instance.TeleportPlayer( WASDPlayer.transform,  arrowKeyPlayer.transform);
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

    private void EndGame()
    {
        loadScreen.SetActive(true);
        Debug.Log("All rounds are complete!");
    }
}
