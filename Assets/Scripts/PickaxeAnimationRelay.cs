using UnityEngine;

public class PickaxeAnimationRelay : MonoBehaviour
{
    public PlayerMining playerMining;

    public void OnSwingHitFrame()
    {
        if (playerMining != null)
        {
            playerMining.PerformSwingHit();
        }
    }
}