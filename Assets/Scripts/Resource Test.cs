using UnityEngine;
using UnityEngine.InputSystem;

public class DebugGiveResources : MonoBehaviour
{
    public int copperAmount = 20;
    public int ironAmount = 20;
    public int goldAmount = 20;
    public int diamondAmount = 20;

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.gKey.wasPressedThisFrame)
        {
            ResourceManager.Instance.Add("Copper", copperAmount);
            ResourceManager.Instance.Add("Iron", ironAmount);
            ResourceManager.Instance.Add("Gold", goldAmount);
            ResourceManager.Instance.Add("Diamond", diamondAmount);
            Debug.Log($"Gave {copperAmount} Copper, {ironAmount} Iron, {goldAmount} Gold, {diamondAmount} Diamond");
        }
    }
}