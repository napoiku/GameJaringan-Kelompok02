using FishNet;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NetworkManagerUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private TextMeshProUGUI statusText;

    private void Awake()
    {
        // Menambahkan listener event pada tombol UI
        hostButton.onClick.AddListener(OnHostButtonClicked);
        clientButton.onClick.AddListener(OnClientButtonClicked);
    }

    private void OnHostButtonClicked()
    {
        // Host berarti menjalankan fungsi StartConnection untuk Server dan Client sekaligus.
        bool serverStarted = InstanceFinder.ServerManager.StartConnection();
        bool clientStarted = InstanceFinder.ClientManager.StartConnection();

        if (serverStarted && clientStarted)
        {
            UpdateUIStatus("Status: Connected as HOST");
        }
        else
        {
            UpdateUIStatus("Status: Failed to Start Host");
        }
    }

    private void OnClientButtonClicked()
    {
        // Menjalankan fungsi StartConnection khusus untuk Client.
        if (InstanceFinder.ClientManager.StartConnection())
        {
            UpdateUIStatus("Status: Connecting as CLIENT...");
        }
        else
        {
            UpdateUIStatus("Status: Failed to Start Client");
        }
    }

    private void UpdateUIStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }

        // Menyembunyikan tombol pilihan setelah role dipilih
        if (hostButton != null) hostButton.gameObject.SetActive(false);
        if (clientButton != null) clientButton.gameObject.SetActive(false);
    }
}