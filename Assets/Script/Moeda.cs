using UnityEngine;

public class Moeda : MonoBehaviour
{
    [SerializeField] private int numeroMoeda;

    public int NumeroPorta()
    {
        return numeroMoeda;
    }

    public void PegarChave()
    {
        Destroy(gameObject);
    }
}