using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    private AudioSource comece; 
    [SerializeField] private AudioClip som; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        comece = GetComponent<AudioSource>();
    }

    public void Jogar()
    {
        SceneManager.LoadScene("Historia"); 
    }

    public void Creditos()
    {
        SceneManager.LoadScene("Creditos");
    }

    public void Voltar()
    {
        SceneManager.LoadScene("MenuInicial");
    }    

    public void Continuar()
    {
        SceneManager.LoadScene("Jogo");
    }

    public void Historia()
    {
        SceneManager.LoadScene("Historia");
    }
}

