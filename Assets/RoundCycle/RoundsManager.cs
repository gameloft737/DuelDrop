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

    public RoundState currentState;
    public enum RoundState { 
        Load, 
        Play, 
        Finish 
    }

    private int currentRound;
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
        for (currentRound = 1; currentRound <= roundSettings.rounds; currentRound++)
        {
            SetRoundState(RoundState.Load);
            loadScreen.SetActive(true);
            yield return new WaitForSeconds(2f); // brief loading period
            loadScreen.SetActive(false);
            SetRoundState(RoundState.Play);
            roundTimeRemaining = roundSettings.roundDuration;

            while (roundTimeRemaining > 0)
            {
                roundTimeRemaining -= Time.deltaTime;
                yield return null;
            }

            SetRoundState(RoundState.Finish);
            yield return new WaitForSeconds(2f);
        }

        EndGame();
    }

    private void SetRoundState(RoundState newState)
    {
        if (newState == RoundState.Play)
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
        
        currentState = newState;
    }

    private void EndGame()
    {
        Debug.Log("All rounds are complete!");
    }
}
