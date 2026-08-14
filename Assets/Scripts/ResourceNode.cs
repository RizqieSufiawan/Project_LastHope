using UnityEngine;
public class ResourceNode : MonoBehaviour
{
    public int chargesRemaining = -1;

    // Called by PlayerMining once per completed mining tick.
    // Returns false if the node is now depleted (caller should stop).
    public bool ConsumeCharge()
    {
        if (chargesRemaining == -1) return true; // infinite node

        chargesRemaining--;
        if (chargesRemaining <= 0)
        {
            Destroy(gameObject);
            return false;
        }
        return true;
    }

    public bool IsDepleted => chargesRemaining == 0;
}
