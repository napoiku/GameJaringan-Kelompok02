using FishNet;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject connectionPanel; // Masukkan objek UI Panel yang berisi tombol ke sini
    public Button hostButton;
    public Button clientButton;

    private void Awake()
    {
        hostButton.onClick.AddListener(OnHostButtonClicked);
        clientButton.onClick.AddListener(OnClientButtonClicked);
    }

    private void OnHostButtonClicked()
    {
        // Jalankan Server dan Client sekaligus untuk Host
        bool serverStarted = InstanceFinder.ServerManager.StartConnection();
        bool clientStarted = InstanceFinder.ClientManager.StartConnection();

        if (serverStarted && clientStarted)
        {
            // Matikan UI Koneksi, biarkan GameManager (Waiting Panel) mengambil alih
            if (connectionPanel != null) connectionPanel.SetActive(false);
        }
    }

    private void OnClientButtonClicked()
    {
        // Jalankan Client saja
        if (InstanceFinder.ClientManager.StartConnection())
        {
            if (connectionPanel != null) connectionPanel.SetActive(false);
        }
    }
}