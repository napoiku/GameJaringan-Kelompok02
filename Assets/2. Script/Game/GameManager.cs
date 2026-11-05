using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using TMPro;

// Mendefinisikan status permainan
public enum GameState { Waiting, Playing, GameWon, GameOver }

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance; // Singleton agar mudah diakses script lain

    [Header("UI Panels")]
    public GameObject waitPanel;
    public GameObject winPanel;
    public GameObject losePanel;

    [Header("Score UI")]
    public TextMeshProUGUI matchScoreText;
    public TextMeshProUGUI highScoreText;

    [Header("Game State")]
    public readonly SyncVar<GameState> currentState = new SyncVar<GameState>(GameState.Waiting);

    // Variabel khusus Server untuk melacak pemain
    private int connectedPlayers = 0;
    private HashSet<GameObject> finishedPlayers = new HashSet<GameObject>(); 

    private void Awake()
    {
        if (Instance == null) Instance = this;
        
        currentState.OnChange += OnStateChanged;
    }

    private void OnDestroy()
    {
        currentState.OnChange -= OnStateChanged;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        UpdateUI(currentState.Value);
    }

    private void OnStateChanged(GameState oldState, GameState newState, bool asServer)
    {
        UpdateUI(newState);
    }

    private void UpdateUI(GameState state)
    {
        if (waitPanel) waitPanel.SetActive(state == GameState.Waiting);
        if (winPanel) winPanel.SetActive(state == GameState.GameWon);
        if (losePanel) losePanel.SetActive(state == GameState.GameOver);

        if ((state == GameState.GameWon || state == GameState.GameOver) && IsClientInitialized)
        {
            ProcessAndDisplayScores();
        }
    }

    private void ProcessAndDisplayScores()
    {
        // Pastikan ScoreManager tersedia
        if (ScoreManager.Instance == null) return;

        // Ambil PlayerStats milik Client lokal menggunakan InstanceFinder dari FishNet
        PlayerStats localPlayer = FishNet.InstanceFinder.ClientManager.Connection.FirstObject.GetComponent<PlayerStats>();

        if (localPlayer != null)
        {
            int currentMatchScore = localPlayer.score.Value;

            if (matchScoreText != null)
                matchScoreText.text = $"Match Score: {currentMatchScore}";
                matchScoreText.gameObject.SetActive(true);

            // 1. Simpan skor match terkini ke backend
            ScoreManager.Instance.SaveCurrentScore(currentMatchScore, () =>
            {
                // 2. Setelah simpan selesai, ambil skor tertinggi terbaru
                ScoreManager.Instance.GetPersonalHighScore((retrievedHighScore) =>
                {
                    if (highScoreText != null)
                    {
                        highScoreText.text = $"Personal High Score: {retrievedHighScore}";
                        highScoreText.gameObject.SetActive(true);
                    }
                });
            });
        }
    }

    public void RegisterPlayer()
    {
        if (!IsServerInitialized) return;
        
        connectedPlayers++;

        if (connectedPlayers >= 2 && currentState.Value == GameState.Waiting)
        {
            currentState.Value = GameState.Playing;
        }
    }

    public void PlayerReachedFinish(GameObject player)
    {
        if (!IsServerInitialized) return;
        if (currentState.Value != GameState.Playing) return;

        if (finishedPlayers.Add(player)) 
        {
            if (finishedPlayers.Count >= 2)
            {
                StartCoroutine(WinRoutine());
            }
        }
    }

    private IEnumerator WinRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        
        if (currentState.Value == GameState.Playing) 
        {
            currentState.Value = GameState.GameWon;
        }
    }

    public void PlayerDied()
    {
        if (!IsServerInitialized) return;
        
        if (currentState.Value == GameState.Playing)
        {
            currentState.Value = GameState.GameOver;
        }
    }
}