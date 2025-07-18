using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SistemaDeVida : MonoBehaviour
{
    [SerializeField] private int vida = 100;
    [SerializeField] private int mana = 100;
    [SerializeField] private Slider manaIndicador;
    [SerializeField] private Slider vidaIndicador;
    private bool estaVivo = true;
    private bool levarDano = true;
    private PlayerMovement pMove;
    private bool podeRecarregarMana = true;
    private int carga;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ProcuraReferancias();

        pMove = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        ProcuraReferancias();
        VerificaMana();
    }
    

    private void ProcuraReferancias()
    {
        if (manaIndicador == null)
        {
            manaIndicador = GameObject.Find("Mana").GetComponent<Slider>();
            manaIndicador.maxValue = mana;
            manaIndicador.value = mana;
        }

        if (vidaIndicador == null)
        {
            vidaIndicador = GameObject.Find("Vida").GetComponent<Slider>();
            vidaIndicador.maxValue = vida;
            vidaIndicador.value = vida;
        }

    }


    public bool EstaVivo()
    {
        return estaVivo;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Fatal") && estaVivo && levarDano)
        {
            StartCoroutine(LevarDano(10));
        }
    }

    IEnumerator LevarDano(int dano)
    {
        levarDano = false;

        if (vida > 0)
        {
            pMove.Hit(); // Chama o m�todo Hit do PlayerMovement para executar a anima��o de dano
            vida -= dano;
            vidaIndicador.value = vida;
            VerificarVida();
            yield return new WaitForSeconds(0.5f);
            levarDano = true;
        }
    }

    private void VerificarVida()
    {
        if (vida <= 0)
        {
            vida = 0;
            estaVivo = false;
        }
    }

    public void VerificaMana()
    {
        if(mana > 1)
        {
            pMove.ComandoMana();
        }
    }

    public void UsarMana()
    {
        mana -= 10;
        VerificaMana();
        manaIndicador.value = mana;

        if (mana < 1)
        {
            mana = 0;
            pMove.TemMana();
        }


    }

    public int GetMana()
    {
        return mana;
    }

    IEnumerable RecarregaMana()
    {
        podeRecarregarMana = false;
        while (mana < 100)
        {
            mana += 5;
            manaIndicador.value = mana;
            yield return new WaitForSeconds(0.1f);
        }
        mana = 100;
        podeRecarregarMana = true;
    }

    internal void CargaMana(int v)
    {
        mana += v;
        manaIndicador.value = mana;
        if(mana > 100)
        {
            mana = 100;
            manaIndicador.value = mana;
        }
    }

    internal void CargaVida(int carga)
    {
        vida += carga;
        vidaIndicador.value = vida;
        if(vida > 100)
        {
            vida = 100;
            vidaIndicador.value = vida;
        }
    }
}