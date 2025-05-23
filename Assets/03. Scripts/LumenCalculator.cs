using UnityEngine;

public class LumenCalculator : MonoBehaviour
{
    [Header("Lumen")] 
    private int lumen = 0;
    
    public int Lumen => lumen;

    public void AddLumen(int amount)
    {
        lumen += amount;
    }

    public void RemoveLumen(int amount)
    {
        lumen -= amount;
    }
}
