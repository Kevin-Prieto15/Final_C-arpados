using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [Header("Modelos del michi animadooo")]
    [SerializeField] private GameObject Front;
    [SerializeField] private GameObject Back;
    [SerializeField] private GameObject SideRight;
    [SerializeField] private GameObject SideLeft;

    private GameObject currentModel;
    private Animator currentAnimator;
    private AudioSource stepAudio;
    public GameObject EfectoCaminar;
    private bool wasWalking = false;
    private float particulaCollDown=0;

    private Vector2 input;
    private Vector2 lastDirection;

    [Header("Velocidad del michi uwu")]
    public float speed = 5f;


    [Header("Combate")]
    public bool tieneArma = true; // puedes cambiar a false si quieres desactivarlo
    public Text textoDebug; // referencia al texto en pantalla para mostrar teclas


    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        SetDirection("Front");
        lastDirection = Vector2.down;
        stepAudio = GetComponent<AudioSource>();
    }

    void Update()
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        bool isWalking = input != Vector2.zero;

        if (isWalking)
        {
            lastDirection = input;

            if (input.x > 0)
                SetDirection("SideRight");
            else if (input.x < 0)
                SetDirection("SideLeft");
            else if (input.y > 0)
                SetDirection("Back");
            else if (input.y < 0)
                SetDirection("Front");

            SetWalking(true);
            if (particulaCollDown>=0.3f)
            {
                Instantiate(EfectoCaminar, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 0.7f), Quaternion.Euler(-90, 0, 0));
                particulaCollDown = 0;
            }
            else
            {
                particulaCollDown += Time.deltaTime;
            }

        }
        else
        {
            SetWalking(false);
        }

        if (isWalking && !wasWalking)
        {
            stepAudio.Play();
        }
        else if (!isWalking && wasWalking)
        {
            stepAudio.Stop();
        }

        wasWalking = isWalking; // Recordamos para la próxima uwu

        if (Input.anyKeyDown)
        {
            foreach (KeyCode kcode in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(kcode))
                {
                    if (textoDebug != null)
                    {
                        textoDebug.text = $"Presionaste: {kcode}";
                        CancelInvoke("BorrarTextoDebug");
                        Invoke("BorrarTextoDebug", 2f);
                    }

                    if (kcode == KeyCode.F)
                    {
                        if (tieneArma)
                        {
                            Debug.Log("Ataque activado");
                            textoDebug.text = $"Presionaste: {kcode} (ataque activado)";
                        }
                        else
                        {
                            Debug.Log("Intento de ataque sin arma");
                            textoDebug.text = $"Presionaste: {kcode} (sin arma)";
                        }

                        CancelInvoke("BorrarTextoDebug");
                        Invoke("BorrarTextoDebug", 2f);
                    }

                    break;
                }
            }
        }

    }

    void BorrarTextoDebug()
    {
        if (textoDebug != null)
            textoDebug.text = "";
    }


    void FixedUpdate()
    {
        rb.MovePosition(rb.position + input * speed * Time.fixedDeltaTime);
    }

    void SetDirection(string direction)
    {
        GameObject newModel = null;

        switch (direction)
        {
            case "Front":
                newModel = Front;
                break;
            case "Back":
                newModel = Back;
                break;
            case "SideRight":
                newModel = SideRight;
                break;
            case "SideLeft":
                newModel = SideLeft;
                break;
        }

        // ¡Si ya está activo ese modelo, no lo cambies nyaa~!
        if (newModel == currentModel)
            return;

        // Apagar todos los michis primero
        Front.SetActive(false);
        Back.SetActive(false);
        SideRight.SetActive(false);
        SideLeft.SetActive(false);

        // Activar el nuevo modelo
        newModel.SetActive(true);
        currentModel = newModel;
        currentAnimator = currentModel.GetComponent<Animator>();
    }


    void SetWalking(bool isWalking)
    {
        if (currentAnimator != null)
        {
            currentAnimator.SetBool("IsWalking", isWalking);
           
        }
    }
}
