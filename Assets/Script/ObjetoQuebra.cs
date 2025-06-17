using System;
using UnityEngine;

public class ObjetoQuebra : MonoBehaviour
{
    [SerializeField] private int vidaObjeto;
    [SerializeField] private GameObject efeitoQuebrar;

    public void Quebrar(int dano)
    {
        vidaObjeto -= dano;

        if(vidaObjeto <= 0 )
        {
            Instantiate(efeitoQuebrar, transform.position, Quaternion.identity);
            Destroy(gameObject); 
        }
    }

    internal void Quebrar(object v)
    {
        throw new NotImplementedException();
    }
}
