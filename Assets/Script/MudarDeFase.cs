using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MudarDeFase : MonoBehaviour
{
    [SerializeField] private string nomeDaProximaFase;
    [SerializeField] private float tempoDeTransicao;
    [SerializeField] private GameObject efeitofade;
    private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = efeitofade.GetComponent<Animator>();
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("player"))
        {
            if(!string.IsNullOrEmpty(nomeDaProximaFase))
            {
                StartCoroutine(TransicaoParaProximaFase());
            }
        }
    }

    IEnumerator TransicaoParaProximaFase()
    {
        animator.SetTrigger("Mudarfase");
        yield return new WaitForSeconds(tempoDeTransicao);
        SceneManager.LoadScene(nomeDaProximaFase);
    }
}
