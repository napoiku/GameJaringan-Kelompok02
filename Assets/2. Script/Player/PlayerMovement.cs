using UnityEngine;
using FishNet.Object; 

public class PlayerMovement : NetworkBehaviour 
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 720f;

    public override void OnStartClient()
    {
        base.OnStartClient();

        // Mengubah warna berdasarkan kepemilikan
        if (IsOwner) {
            GetComponent<Renderer>().material.color = Color.green;
            float randomX = Random.Range(-3f, 3f);
            float randomZ = Random.Range(-3f, 3f);
            transform.position = new Vector3(randomX, 0.5f, randomZ);
        } 
        else {
            GetComponent<Renderer>().material.color = Color.red;
        }
    }

    void Update()
    {
        // Cegah client lain menggerakkan karakter ini
        if (!IsOwner) 
            return;

        HandleMovement2D();
    }

    private void HandleMovement2D()
    {
        // Membaca input keyboard (WASD / Panah)
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        // Pergerakan 2D menggunakan sumbu X dan Y
        Vector2 moveDirection = new Vector2(moveX, moveY).normalized;

        if (moveDirection.magnitude >= 0.1f)
        {
            // Memindahkan posisi karakter
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);

            // Memutar rotasi karakter 2D (menghadap ke arah gerakan pada sumbu Z)
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, moveDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}