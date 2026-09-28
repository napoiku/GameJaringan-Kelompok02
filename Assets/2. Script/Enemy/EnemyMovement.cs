using UnityEngine;
using FishNet.Object;

public class EnemyAI : NetworkBehaviour
{
     public float moveSpeed = 2f;
     private int arahGerak = -1; // -1 = Kiri, 1 = Kanan

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
          // Sesuaikan dengan orientasi default sprite Anda (jika terbalik, ganti -arahGerak menjadi arahGerak)
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
               // Ambil script PlayerStats dari objek pemain yang bersentuhan
               PlayerStats statsPemain = collision.gameObject.GetComponent<PlayerStats>();

               // DETEKSI ATAS: Pemain menginjak kepala musuh
               if (collision.transform.position.y > transform.position.y + 0.2f)
               {
                    // Tambah skor pemain
                    if (statsPemain != null)
                    {
                         statsPemain.IncreaseScore(5);
                    }

                    // Beri efek pantulan (bounce) ke pemain
                    Rigidbody2D rbPemain = collision.gameObject.GetComponent<Rigidbody2D>();
                    if (rbPemain != null)
                    {
                         rbPemain.linearVelocity = new Vector2(rbPemain.linearVelocity.x, 10f); 
                    }

                    Mati(); // Hapus musuh
               }
               else
               {
                    if (statsPemain != null)
                    {
                         statsPemain.DecreaseHealth(10); // Kurangi 20 HP
                         
                         // Opsional: Beri efek pentalan kecil ke belakang agar pemain tidak nempel terus
                         Rigidbody2D rbPemain = collision.gameObject.GetComponent<Rigidbody2D>();
                         if (rbPemain != null)
                         {
                         // Memantulkan pemain sedikit ke atas dan berlawanan arah dari musuh
                         float arahPental = (collision.transform.position.x < transform.position.x) ? -5f : 5f;
                         rbPemain.linearVelocity = new Vector2(arahPental, 5f);
                         }
                    }
               }
          }
          // 2. JIKA MUSUH MENYENTUH DINDING
          else
          {
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
