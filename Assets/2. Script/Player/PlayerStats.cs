using UnityEngine;
using UnityEngine.UI;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using TMPro;

public class PlayerStats : NetworkBehaviour
{    
     [SerializeField] private Slider healthSlider;
     [SerializeField] private TextMeshProUGUI scoreText;

     public readonly SyncVar<int> health = new SyncVar<int>(100);
     public readonly SyncVar<int> score = new SyncVar<int>(0);

     private void Awake()
     {
          health.OnChange += OnHealthChanged;
          score.OnChange += OnScoreChanged; // Daftarkan event skor
     }

     private void OnDestroy()
     {
          health.OnChange -= OnHealthChanged;
          score.OnChange -= OnScoreChanged; // Hapus event skor
     }

     public override void OnStartClient()
     {
          base.OnStartClient();
               
          healthSlider.value = health.Value;
          scoreText.text = "Score: 0";
     }

     private void OnHealthChanged(int oldHealth, int newHealth, bool asServer)
     {
          if (healthSlider != null)
          {
               healthSlider.value = newHealth;
          }

          if (newHealth <= 0)
          {
               Debug.Log("Pemain Mati!");
          }
     }

     private void OnScoreChanged(int oldScore, int newScore, bool asServer)
     {
          if (scoreText != null)
          {
               scoreText.text = "Score: " + newScore;
          }
     }

     [ServerRpc(RequireOwnership = false)]
     public void DecreaseHealth(int damage)
     {
          health.Value -= damage; 
     }

     [ServerRpc]
     public void IncreaseScore(int jumlah)
     {
          score.Value += jumlah;
     }

     private void Update()
     {
          if (!IsOwner) return;
          // Simulasi input keyboard untuk pengujian
          // Tekan 'K' untuk mengurangi HP
          // if (Input.GetKeyDown(KeyCode.K))
          // {
          //      DecreaseHealth(10);
          // }
          // // Tekan 'L' untuk menambah Score
          // if (Input.GetKeyDown(KeyCode.L))
          // {
          //      IncreaseScore(5);
          // }
     }
}