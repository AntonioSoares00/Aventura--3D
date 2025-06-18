using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    private float inputH;
    private float inputV;
    private Animator animator;
    private bool estaNoChao = true;
    private float velocidadeAtual;
    private bool contato = false;
    private bool morrer = true;
    private SistemaDeVida sVida;
    private Vector3 anguloRotacao = new Vector3(0, 90, 0);
    private bool temChave = false;
    private bool temMana = true;
    private int numeroChave = 0;
    [SerializeField] private float velocidadeAndar;
    [SerializeField] private float velocidadeCorrer;
    [SerializeField] private float forcaPulo;
    [SerializeField] private GameObject quebraPreFab;
    [SerializeField] private GameObject machadoPrefab;
    [SerializeField] private GameObject miraMachado;
    [SerializeField] private int forcaArremeco;
    private SistemaInterativo sInterativo;
    [SerializeField] private CinemachineCamera cineCamera;

    public object ForcaMode { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        sVida = GetComponent<SistemaDeVida>();
        velocidadeAtual = velocidadeAndar;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            contato = true;
        }

        if (sVida.EstaVivo())
        {
            
            Correr();
            //Atacar();
            Machado();
        }
        else if (!sVida.EstaVivo() && morrer)
        {
            Morrer();
        }
    }

    void FixedUpdate()
    {
       Andar();
       Girar();
       Pular();
    }

    private void ProcuraRefarencias()
    {
        
        if(cineCamera == null )
        {
            transform.position = GameObject.Find("StartPoint").transform.position;
            cineCamera = GameObject.Find("CinemachineCamera").GetComponent<CinemachineCamera>();
            cineCamera.Follow = this.gameObject.transform;
        }
    }

    private void Andar()
    {
        inputV = Input.GetAxis("Vertical");
        Vector3 moveDirection = transform.forward * inputV;
        Vector3 moveForward = rb.position + moveDirection * velocidadeAtual * Time.deltaTime;
        rb.MovePosition(moveForward);

        if (Input.GetKey(KeyCode.W))
        {
            animator.SetBool("Andar", true);
            animator.SetBool("AndarTras", false);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            animator.SetBool("AndarTras", true);
            animator.SetBool("Andar", false);
        }
        else
        {
            animator.SetBool("AndarTras", false);
            animator.SetBool("Andar", false);
        }
    }

    private void Girar()
    {
        inputH = Input.GetAxis("Horizontal");
        Quaternion deltaRotation =
            Quaternion.Euler(anguloRotacao * inputH * Time.deltaTime);
        rb.MoveRotation(rb.rotation * deltaRotation);

        if (Input.GetKey(KeyCode.A) ||
                    Input.GetKey(KeyCode.D) ||
                        Input.GetKey(KeyCode.LeftArrow) ||
                            Input.GetKey(KeyCode.RightArrow))
        {
            animator.SetBool("Andar", true);
        }
    }

    private void Pular()
    {
        if (Input.GetKeyDown(KeyCode.Space) && estaNoChao)
        {
            rb.AddForce(Vector3.up * forcaPulo, ForceMode.Impulse);
            animator.SetTrigger("Pular");
            StartCoroutine(TempoPulo());
        }
    }

    IEnumerator TempoPulo()
    {
        estaNoChao = false;
        yield return new WaitForSeconds(1.5f);
        estaNoChao = true;
    }

    private void Correr()
    {
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift))
        {
            velocidadeAtual = velocidadeCorrer;
            animator.SetBool("Correr", true);
        }
        else
        {
            velocidadeAtual = velocidadeAndar;
            animator.SetBool("Correr", false);
        }
    }

    private void Morrer()
    {
        morrer = false;
        animator.SetBool("EstaVivo", false);
        animator.SetTrigger("Morrer");
        rb.Sleep();
    }

    private void Interagir()
    {
        animator.SetTrigger("Interagir");
    }

    private void Pegar()
    {
        animator.SetTrigger("Pegar");
    }

    private int Atacar()
    {
        animator.SetTrigger("Atacar");
        Instantiate(quebraPreFab, miraMachado.transform.position, miraMachado.transform.rotation);
        contato = false;
        return 10;
    }

    private void Machado()
    {
        if (Input.GetMouseButtonDown(1))
        {
            StartCoroutine(LancarMachado(GetMiraMachado()));
            animator.SetTrigger("Machado");
        }
    }

    private GameObject GetMiraMachado()
    {
        return miraMachado;
    }

    IEnumerator LancarMachado(GameObject gameObject)
    {
        if (temMana)
        {
            yield return new WaitForSeconds(0.5f);
            GameObject machado = Instantiate(machadoPrefab, miraMachado.transform.position, miraMachado.transform.rotation);
            machado.transform.rotation *= Quaternion.Euler(0, -90, 0);
            Rigidbody rbMachado = machado.GetComponent<Rigidbody>();
            rbMachado.AddForce(miraMachado.transform.forward * forcaArremeco, ForceMode.Impulse);
            sVida.UsarMana();
        }
    }

    public void TemMana()
    {
        temMana = false;
    }

    public void ComandoMana()
    {
        temMana = true;
    }

    public void Hit()
    {
        animator.SetTrigger("Hit");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Quebra"))
        {
            Atacar();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            estaNoChao = true;
            animator.SetBool("EstaNoChao", true);
        }

        if (collision.gameObject.CompareTag("Quebra") && Input.GetMouseButtonDown(0))
        {
            Atacar();
            Destroy(collision.gameObject);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            estaNoChao = false;
            animator.SetBool("EstaNoChao", false);
        }
        if (collision.gameObject.CompareTag("Quebra"))
        {

            contato = false;
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Mana") && Input.GetKey(KeyCode.E))
        {
            Pegar();
            sVida.CargaMana(50);
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Vida") && Input.GetKey(KeyCode.E))
        {
            Pegar();
            sVida.CargaVida();
        }
        else if (other.CompareTag("Porta") && Input.GetKey(KeyCode.E))
        {
            if (other.gameObject.GetComponent<Porta>().EstaTrancada())
            {
                Interagir();
                other.gameObject.GetComponent<Porta>().AbrirPorta();
            }
            else if (!other.gameObject.GetComponent<Porta>().EstaTrancada())
            {
                Interagir();
                other.gameObject.GetComponent<Porta>().AbrirPorta();
            }
        }
        //else if (other.gameObject.CompareTag("Chave") && Input.GetKey(KeyCode.E)) ;
        //{
        //    Pegar();
        //    temChave = true;
        //    numeroChave = other.gameObject.GetComponent<Chave>().NumeroPorta();
        //    other.gameObject.GetComponent<Chave>().PegarChave();
        //}

        if(other.gameObject.CompareTag("Quebra"))
        {
            if(contato)
            {
                other.gameObject.GetComponent<ObjetoQuebra>().Quebrar(Atacar());
                    
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Chave"))
        {
            sInterativo.ExibirInteragir();
        }
    } 
}
