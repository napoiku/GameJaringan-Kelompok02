using UnityEngine;
using FishNet.Object;

public class EnemyAI : NetworkBehaviour
{
     [Header("Pengaturan Gerak")]
     public float moveSpeed = 2f;
     private int arahGerak = -1; // -1 = Kiri, 1 = Kanan

     [Header("Efek Pantulan (Bisa Diatur Lewat Inspector)")]
     [Tooltip("Tinggi pantulan pemain saat berhasil menginjak musuh dari atas")]
     public float kekuatanPantulAtas = 5f;
     
     [Tooltip("Dorongan ke samping saat pemain tertabrak musuh")]
     public float pentalanSamping = 3f;
     
     [Tooltip("Dorongan ke atas saat pemain tertabrak musuh")]
     public float pentalanAtas = 3f;

     private Collider2D enemyCollider;

     private void Awake()
     {
          enemyCollider = GetComponent<Collider2D>();
     }

     void Update()
     {
          // Hanya Server yang berhak menggerakkan musuh
          if (!IsServerInitialized) 
               return;

          transform.Translate(Vector2.right * arahGerak * moveSpeed * Time.deltaTime);
     }

     private void BerbalikArah()
     {
          // Balik arah (kiri jadi kanan, kanan jadi kiri)
          arahGerak *= -1; 
               
          // Balik orientasi hadap sprite (Flip X)
          transform.localScale = new Vector3(-arahGerak, 1f, 1f);
     }

     private void OnCollisionEnter2D(Collision2D collision)
     {
          // Hanya Server yang memproses tabrakan
          if (!IsServerInitialized) 
               return;

          Vector2 titikPantul = collision.contacts[0].normal;

          // 1. JIKA MUSUH MENYENTUH PEMAIN
          if (collision.gameObject.CompareTag("Player"))
          {
               PlayerStats statsPemain = collision.gameObject.GetComponent<PlayerStats>();
               Rigidbody2D rbPemain = collision.gameObject.GetComponent<Rigidbody2D>();

               // AMBIL DATA BATAS COLLIDER (Untuk menghindari Bug Mati dari Samping)
               // Batas paling atas fisik musuh
               float batasAtasMusuh = enemyCollider.bounds.max.y; 
               // Batas kaki/bawah fisik pemain
               float batasBawahPemain = collision.collider.bounds.min.y; 

               // DETEKSI ATAS: Kaki pemain harus benar-benar berada di atas kepala musuh,
               // DAN pemain harus sedang bergerak turun atau diam (bukan sedang melompat ke atas)
               if (batasBawahPemain >= batasAtasMusuh - 0.15f && rbPemain != null && rbPemain.linearVelocity.y <= 0.1f)
               {
                    // Tambah skor pemain
                    if (statsPemain != null)
                    {
                         statsPemain.IncreaseScore(5);
                    }

                    // Beri efek pantulan (bounce) ke pemain
                    rbPemain.linearVelocity = new Vector2(rbPemain.linearVelocity.x, kekuatanPantulAtas); 

                    Mati(); // Hapus musuh
               }
               // DETEKSI SAMPING: Pemain menabrak tubuh atau sisi musuh (Pemain Terluka)
               else
               {
                    if (statsPemain != null)
                    {
                         statsPemain.DecreaseHealth(10); 
                         
                         // Beri efek pentalan kecil ke belakang agar pemain tidak menempel terus
                         if (rbPemain != null)
                         {
                              float arahPental = (collision.transform.position.x < transform.position.x) ? -pentalanSamping : pentalanSamping;
                              rbPemain.linearVelocity = new Vector2(arahPental, pentalanAtas);
                         }

                         // BARU: Musuh langsung berbalik arah setelah melukai pemain 
                         // agar tidak terus-terusan berjalan menerobos pemain tersebut
                         BerbalikArah();
                    }
               }
          }
          // 2. JIKA MUSUH MENYENTUH DINDING
          else
          {
               // Jika terjadi tabrakan horizontal (X) dengan dinding/rintangan
               if (Mathf.Abs(titikPantul.x) > 0.5f)
               {
                    BerbalikArah();
               }
          }
     }

     private void Mati()
     {
          Debug.Log("Musuh terinjak dan mati!");
          ServerManager.Despawn(gameObject); // Hapus objek ini dari semua layar client
     }
}