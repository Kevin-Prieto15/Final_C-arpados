using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    private bool esInvisible = false;

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
    private float particulaCollDown = 0;

    private Vector2 input;
    private Vector2 lastDirection;

    [Header("Velocidad del michi 7u7")]
    public float speed = 5f;

    [Header("Combate")]
    public bool tieneArma = true;
    public Text textoDebug;

    private Rigidbody2D rb;

    [Header("Estamina")]
    public Slider staminaSlider;
    public float maxStamina = 8f;
    public float regenDelay = 2f;
    public float regenDuration = 4f;
    public float runSpeed = 20f;

    private float currentStamina;
    private float regenTimer = 0f;
    private bool isRunning = false;
    private bool isRegenerating = false;
    private float defaultSpeed;

    [Header("Hambre y Sed")]
    public TextMeshProUGUI textoHambre;
    public TextMeshProUGUI textoSed;

    private float hambre = 0f; // 0% al 100%
    private float sed = 0f;

    private float timerHambre = 0f;
    private float timerSed = 0f;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        SetDirection("Front");
        lastDirection = Vector2.down;
        stepAudio = GetComponent<AudioSource>();

        defaultSpeed = speed;
        currentStamina = maxStamina;

        if (staminaSlider != null)
        {
            staminaSlider.maxValue = maxStamina;
            staminaSlider.value = currentStamina;
        }

        staminaSlider.maxValue = maxStamina;
        staminaSlider.minValue = 0;
        staminaSlider.value = currentStamina;

    }
    void Update()
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        bool isWalking = input != Vector2.zero;

        if (isWalking)
        {
            lastDirection = input;

            if (input.x > 0) SetDirection("SideRight");
            else if (input.x < 0) SetDirection("SideLeft");
            else if (input.y > 0) SetDirection("Back");
            else if (input.y < 0) SetDirection("Front");

            SetWalking(true);

            if (particulaCollDown >= 0.3f)
            {
                Instantiate(EfectoCaminar, new Vector3(transform.position.x, transform.position.y - 0.7f), Quaternion.Euler(-90, 0, 0));
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

        if (isWalking && !wasWalking) stepAudio.Play();
        else if (!isWalking && wasWalking) stepAudio.Stop();

        wasWalking = isWalking;

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

            if (esInvisible)
                StartCoroutine(FadeInvisibilidad(true));
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            esInvisible = !esInvisible;
            StartCoroutine(FadeInvisibilidad(esInvisible));
        }

        // --- Lógica de correr y estamina ---
        bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        bool wantsToRun = shift && input != Vector2.zero;
        bool canRun = currentStamina > 0f;

        isRunning = wantsToRun && canRun;
        Debug.Log("Corriendo: " + isRunning + " | Stamina: " + currentStamina);

        if (isRunning)
        {
            currentStamina -= Time.deltaTime;
            if (currentStamina < 0f) currentStamina = 0f;

            regenTimer = 0f;
            isRegenerating = false;
        }
        else
        {
            if (currentStamina < maxStamina)
            {
                regenTimer += Time.deltaTime;

                if (regenTimer >= regenDelay)
                {
                    isRegenerating = true;
                    currentStamina += (maxStamina / regenDuration) * Time.deltaTime;
                    if (currentStamina > maxStamina) currentStamina = maxStamina;
                }
            }
            else
            {
                isRegenerating = false;
                regenTimer = 0f;
            }
        }

        if (staminaSlider != null)
        {
            staminaSlider.value = currentStamina;

            Image fillImage = staminaSlider.fillRect.GetComponent<Image>();
            if (fillImage != null)
            {
                fillImage.color = Color.Lerp(Color.red, Color.cyan, currentStamina / maxStamina);
            }
        }

        if (Input.GetKey(KeyCode.RightShift))
            Debug.Log("RightShift PRESIONADO");

        // Aumentar hambre cada 10 segundos
        timerHambre += Time.deltaTime;
        if (timerHambre >= 10f)
        {
            hambre = Mathf.Min(hambre + 1f, 100f);
            timerHambre = 0f;
        }

        // Aumentar sed cada 5 segundos
        timerSed += Time.deltaTime;
        if (timerSed >= 5f)
        {
            sed = Mathf.Min(sed + 1f, 100f);
            timerSed = 0f;
        }

        // Actualizar textos
        if (textoHambre != null) textoHambre.text = $"Hambre {hambre}%";
        if (textoSed != null) textoSed.text = $"Sed {sed}%";

        //Kevin estubo aqui
        //Final update
    }


    void FixedUpdate()
    {
        float moveSpeed = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && currentStamina > 0f
            ? runSpeed
            : defaultSpeed;

        rb.MovePosition(rb.position + input * moveSpeed * Time.fixedDeltaTime);
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

        if (newModel == currentModel)
            return;

        Front.SetActive(false);
        Back.SetActive(false);
        SideRight.SetActive(false);
        SideLeft.SetActive(false);

        newModel.SetActive(true);
        currentModel = newModel;
        currentAnimator = currentModel.GetComponent<Animator>();

        if (esInvisible)
            StartCoroutine(FadeInvisibilidad(true));
    }

    void SetWalking(bool isWalking)
    {
        if (currentAnimator != null)
        {
            currentAnimator.SetBool("IsWalking", isWalking);
        }
    }

    IEnumerator FadeInvisibilidad(bool invisible)
    {
        float targetAlpha = invisible ? 0f : 1f;
        float duration = 0.4f;
        float time = 0f;

        List<SpriteRenderer> renderers = new List<SpriteRenderer>();
        renderers.AddRange(Front.GetComponentsInChildren<SpriteRenderer>(true));
        renderers.AddRange(Back.GetComponentsInChildren<SpriteRenderer>(true));
        renderers.AddRange(SideRight.GetComponentsInChildren<SpriteRenderer>(true));
        renderers.AddRange(SideLeft.GetComponentsInChildren<SpriteRenderer>(true));

        Dictionary<SpriteRenderer, float> startAlphas = new Dictionary<SpriteRenderer, float>();
        foreach (var sr in renderers)
        {
            if (sr != null)
                startAlphas[sr] = sr.color.a;
        }

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            foreach (var sr in renderers)
            {
                if (sr == null) continue;
                Color c = sr.color;
                c.a = Mathf.Lerp(startAlphas[sr], targetAlpha, t);
                sr.color = c;
            }

            yield return null;
        }

        foreach (var sr in renderers)
        {
            if (sr == null) continue;
            Color c = sr.color;
            c.a = targetAlpha;
            sr.color = c;
        }
    }

    void SetAlpha(GameObject obj, float alpha)
    {
        if (obj == null) return;

        SpriteRenderer[] renderers = obj.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (SpriteRenderer sr in renderers)
        {
            if (sr == null) continue;
            Color c = sr.color;
            c.a = alpha;
            sr.color = c;
        }
    }

    void BorrarTextoDebug()
    {
        if (textoDebug != null)
            textoDebug.text = "";
    }
}
