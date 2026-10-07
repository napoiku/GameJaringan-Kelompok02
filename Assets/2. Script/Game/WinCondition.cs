using UnityEngine;
using FishNet.Object;

public class WinCondition : NetworkBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServerInitialized) 
            return;

        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.PlayerReachedFinish(collision.gameObject);
        }
    }
}