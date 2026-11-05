using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using FishNet;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [SerializeField] private string baseURL = "http://localhost:5000/api";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private int GetFishNetUserId()
    {
        // Jika bertindak sebagai Server (Host), gunakan ID 1
        if (InstanceFinder.IsServerStarted) 
        {
            return 1;
        }
        // Jika bertindak murni sebagai Client, gunakan ID 2
        else if (InstanceFinder.IsClientStarted) 
        {
            return 2;
        }
        
        return -1; // Jika belum terkoneksi
    }

    // Method 1: Mengirim skor pertandingan terkini ke backend
    public void SaveCurrentScore(int finalScore, Action onComplete = null)
    {
        int activeUserId = GetFishNetUserId(); // Ganti PlayerPrefs dengan deteksi FishNet
        
        if (activeUserId == -1)
        {
            Debug.LogWarning("[ScoreManager] FishNet belum terkoneksi (Bukan Host/Client).");
            onComplete?.Invoke();
            return;
        }

        StartCoroutine(SendScoreRequest(activeUserId, finalScore, onComplete));
    }

    private IEnumerator SendScoreRequest(int userId, int score, Action onComplete)
    {
        SaveScoreRequest reqData = new SaveScoreRequest { userId = userId, score = score };
        string jsonPayload = JsonUtility.ToJson(reqData);

        using (UnityWebRequest request = new UnityWebRequest(baseURL + "/score/save", "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                SaveScoreResponse resp = JsonUtility.FromJson<SaveScoreResponse>(request.downloadHandler.text);
                Debug.Log($"[ScoreManager] Skor Berhasil Disimpan! Score ID: {resp.scoreId}");
            }
            else
            {
                Debug.LogError($"[ScoreManager] Gagal menyimpan skor: {request.error}");
            }

            onComplete?.Invoke();
        }
    }

    // Method 2: Meminta skor tertinggi pemain dari backend
    public void GetPersonalHighScore(Action<int> onCallback)
    {
        int activeUserId = GetFishNetUserId(); // Ganti PlayerPrefs dengan deteksi FishNet
        
        if (activeUserId == -1)
        {
            onCallback?.Invoke(0);
            return;
        }

        StartCoroutine(FetchHighScoreRequest(activeUserId, onCallback));
    }

    private IEnumerator FetchHighScoreRequest(int userId, Action<int> onCallback)
    {
        string url = $"{baseURL}/score/highscore/{userId}";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                HighScoreResponse resp = JsonUtility.FromJson<HighScoreResponse>(request.downloadHandler.text);
                Debug.Log($"[ScoreManager] High Score Terambil: {resp.highScore}");
                onCallback?.Invoke(resp.highScore);
            }
            else
            {
                Debug.LogError($"[ScoreManager] Gagal mengambil high score: {request.error}");
                onCallback?.Invoke(0);
            }
        }
    }
}