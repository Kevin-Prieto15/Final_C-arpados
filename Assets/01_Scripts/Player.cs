using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    public static Player Instance;
    // ╔═ Campos de Dirección y Modelos ═══════════════════════════════════════╗
    private enum Direction { Front, Back, SideRight, SideLeft }

    [Header("Modelos del michi animadooo")]
    [SerializeField] private GameObject frontModel;
    [SerializeField] private GameObject backModel;
    [SerializeField] private GameObject sideRightModel;
    [SerializeField] private GameObject sideLeftModel;

    private GameObject currentModel;
    private Animator currentAnimator;

    // ╔═ Salud ═════════════════════════════════════════════════════════════╗
    public float maxHealth = 100f;
    public float currentHealth;
    private bool isDead = false;

    // ╔═ Audio y Efectos ═══════════════════════════════════════════════╗
    private AudioSource stepAudio;
    public GameObject efectoCaminar;
    private bool wasWalking = false;
    private float particulaCooldown = 0f;

    // ╔═ Movimiento ═════════════════════════════════════════════════╗
    private Rigidbody2D rb;
    private Vector2 input;
    private Vector2 lastDirection;
    [Header("Velocidad del michi 7u7")]
    public float speed = 5f;
    private bool primeraVez = true;

    // ╔═ Invisibilidad ════════════════════════════════════════════╗
    private bool esInvisible = false;

    // ╔═ Combate ═════════════════════════════════════════════════╗
    [Header("Combate")]
    public bool tieneArma = true;
    public Text textoDebug;

    // ╔═ Estamina ═════════════════════════════════════════════════╗
    [Header("Estamina")]
    public UnityEngine.UI.Slider staminaSlider;
    public float maxStamina = 8f;
    public float regenDelay = 2f;
    public float regenDuration = 4f;
    public float runSpeed = 20f;

    private float currentStamina;
    private float regenTimer = 0f;
    private bool isRunning = false;
    private bool isRegenerating = false;
    private float defaultSpeed;

    // ╔═ Hambre y Sed ════════════════════════════════════════════╗
    [Header("Hambre y Sed")]
    public TextMeshProUGUI textoHambre;
    public TextMeshProUGUI textoSed;

    private float hambre = 0f; // 0% al 100%
    private float sed = 0f;    // 0% al 100%
    private float timerHambre = 0f;
    private float timerSed = 0f;

    // ╔═ Herramientas ════════════════════════════════════════════╗
    [Header("Herramientas")]
    public bool isAxe = false;
    public bool isPickaxe = false;
    public bool isLance = false;
    Collider2D currentWeapon=null;
    public float ToolDuration=1;
    float ToolTimer = 0;
    bool ToolAttack=false;


    [Header("Audio de Daño y Muerte")]
    public AudioClip hit1Clip;
    public AudioClip hit2Clip;
    public AudioClip deathClip;
    private AudioSource damageAudio;




    // ╔═ Unity Callbacks ══════════════════════════════════════════╗
    private void Awake()
    {
        Instance = this;
        // Obtener componentes
        rb = GetComponent<Rigidbody2D>();
        stepAudio = GetComponent<AudioSource>();
        damageAudio = gameObject.AddComponent<AudioSource>();



        stepAudio.Stop();

        // Inicializar dirección por defecto
        SetDirection(Direction.Front);
        lastDirection = Vector2.down;

        // Guardar valores iniciales de velocidad y estamina
        defaultSpeed = speed;
        currentHealth = maxHealth;
        currentStamina = maxStamina;

        // Configurar slider de estamina si existe
        if (staminaSlider != null)
        {
            staminaSlider.maxValue = maxStamina;
            staminaSlider.minValue = 0f;
            staminaSlider.value = currentStamina;
        }
        tieneArma = true;
        EquipWeapon(0); // Asegura que isAxe=true y asigna el collider del arma



    }

    private void Update()
    {
        LeerInputMovimiento();
        ManejarMovimientoYAnimaciones();

        // Lanzar ataque según dirección
        if (Input.GetKeyDown(KeyCode.F) && tieneArma)
        {
            if (currentAnimator != null)
            {
                currentAnimator.SetTrigger("Attack");

                if (lastDirection.y > 0) currentAnimator.SetInteger("Direction", 1); // Back
                else if (lastDirection.y < 0) currentAnimator.SetInteger("Direction", 0); // Front
                else if (lastDirection.x > 0) currentAnimator.SetInteger("Direction", 3); // Right
                else if (lastDirection.x < 0) currentAnimator.SetInteger("Direction", 2); // Left
            }
        }

        ManejarEntradaGeneral();
        ManejarCorrerYEstamina();
        ActualizarIndicadoresHambreSed();

        if (ToolAttack)
        {
            ToolTimer += Time.deltaTime;

            if (ToolTimer >= ToolDuration)
            {
                ToolTimer = 0f;
                ToolAttack = false;
                if (currentWeapon != null)
                    currentWeapon.enabled = false;
            }
        }

    }



    private void FixedUpdate()
    {
        MoverAlrigido();
    }

    // ╔═ Métodos de Input y Movimiento ═══════════════════════════════╗
    private void LeerInputMovimiento()
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
    }

    private void ManejarMovimientoYAnimaciones()
    {
        bool isWalking = input != Vector2.zero;

        if (isWalking)
        {
            lastDirection = input;
            Direction nuevaDir = ObtenerDireccionDesdeInput(input);
            SetDirection(nuevaDir);
            SetWalking(true);
            
            

            // Partícula al caminar cada 0.3s
            if (particulaCooldown >= 0.3f)
            {
                Vector3 posParticula = new Vector3(transform.position.x, transform.position.y - 0.7f);
                Instantiate(efectoCaminar, posParticula, Quaternion.Euler(-90, 0, 0));
                particulaCooldown = 0f;
            }
            else
            {
                particulaCooldown += Time.deltaTime;
            }
        }
        else
        {
            SetWalking(false);
        }

        // Sonido de pasos
        if (isWalking && !wasWalking) stepAudio.Play();
        else if (!isWalking && wasWalking) stepAudio.Stop();
        wasWalking = isWalking;
    }

    private Direction ObtenerDireccionDesdeInput(Vector2 dir)
    {
        if (dir.x > 0) return Direction.SideRight;
        else if (dir.x < 0) return Direction.SideLeft;
        else if (dir.y > 0) return Direction.Back;
        else return Direction.Front;
    }

    private void MoverAlrigido()
    {
        bool shiftPresionado = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float velocidadActual = (shiftPresionado && currentStamina > 0f) ? runSpeed : defaultSpeed;
        Vector2 nuevaPos = rb.position + input * velocidadActual * Time.fixedDeltaTime;
        rb.MovePosition(nuevaPos);
    }

    // ╔═ Métodos de Entrada General (Teclas) ═════════════════════════╗
    private void ManejarEntradaGeneral()
    {
        if (Input.anyKeyDown)
        {
            // Detectar cualquier tecla y mostrar en debug
            foreach (KeyCode tecla in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(tecla))
                {
                    if (textoDebug != null)
                    {
                        textoDebug.text = $"Presionaste: {tecla}";
                        CancelInvoke(nameof(BorrarTextoDebug));
                        Invoke(nameof(BorrarTextoDebug), 2f);
                    }

                    // Manejar ataque con F
                    if (tecla == KeyCode.F && (isAxe || isPickaxe || isLance))
                    {
                        if (currentWeapon == null)
                        {
                            Debug.Log("Intento de ataque sin arma");
                            return;
                        }

                        // Activar animación de ataque según arma equipada
                        if (isAxe)
                            currentAnimator.SetTrigger("AxeAttack");
                        else if (isLance)
                            currentAnimator.SetTrigger("LanceAttack");
                        else if (isPickaxe)
                            currentAnimator.SetTrigger("PickaxeAttack");

                        // Activar collider del arma
                        currentWeapon.enabled = true;

                        // Detectar colisiones del arma
                        Bounds bounds = currentWeapon.bounds;
                        Vector2 center = bounds.center;
                        Vector2 size = bounds.size;
                        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, 0f);

                        foreach (Collider2D hit in hits)
                        {
                            if (hit == null) continue;

                            Spider spider = hit.GetComponent<Spider>();
                            if (spider != null)
                            {
                                spider.TakeDamageFromPlayer(1f); // Aplica daño
                            }
                        }

                        ToolTimer = 0f;
                        ToolAttack = true;

                        CancelInvoke(nameof(BorrarTextoDebug));
                        Invoke(nameof(BorrarTextoDebug), 2f);
                    }
                    break;

                }
            }

            // Si estaba invisible y presiona algo, volver a aparecer
            if (esInvisible)
            {
                esInvisible = false;
                StartCoroutine(FadeInvisibilidad(esInvisible));
            }
        }

        // Alternar invisibilidad con N
        if (Input.GetKeyDown(KeyCode.N))
        {
            esInvisible = !esInvisible;
            StartCoroutine(FadeInvisibilidad(esInvisible));
        }

        // Solo debug extra cuando se presiona RightShift
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            Debug.Log("RightShift PRESIONADO");
        }
    }

    // ╔═ Métodos de Correr y Estamina ═══════════════════════════════╗
    private void ManejarCorrerYEstamina()
    {
        bool shiftPresionado = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        bool quiereCorrer = shiftPresionado && input != Vector2.zero;
        bool puedeCorrer = currentStamina > 0f;

        isRunning = quiereCorrer && puedeCorrer;
        //Debug.Log($"Corriendo: {isRunning} | Stamina: {currentStamina}");

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
            UnityEngine.UI.Image fillImage = staminaSlider.fillRect.GetComponent<UnityEngine.UI.Image>();
            if (fillImage != null)
            {
                fillImage.color = Color.Lerp(Color.red, Color.cyan, currentStamina / maxStamina);
            }
        }
    }

    // ╔═ Métodos de Hambre y Sed ═══════════════════════════════════╗
    private void ActualizarIndicadoresHambreSed()
    {
        // Aumentar hambre cada 10 segundos
        timerHambre += Time.deltaTime;
        if (timerHambre >= 10f)
        {
            if (esInvisible)
            {
                hambre = Mathf.Min(hambre + (1f) * 2, 100f);
            }
            else
            {
                hambre = Mathf.Min(hambre + 1f, 100f);
            }
            timerHambre = 0f;
        }

        // Aumentar sed cada 5 segundos
        timerSed += Time.deltaTime;
        if (timerSed >= 5f)
        {
            if (esInvisible)
            {
                sed = Mathf.Min(sed + (1f) * 2, 100f);
            }
            else
            {
                sed = Mathf.Min(sed + 1f, 100f);
            }
            timerSed = 0f;
        }

        // Actualizar textos en UI
        if (textoHambre != null)
            textoHambre.text = $"Hambre {hambre}%";
        if (textoSed != null)
            textoSed.text = $"Sed {sed}%";
    }

    // ╔═ Métodos de Modelos y Animaciones ════════════════════════════╗


    private void SetDirection(Direction dir)
    {
        GameObject nextModel = dir switch
        {
            Direction.Front => frontModel,
            Direction.Back => backModel,
            Direction.SideRight => sideRightModel,
            Direction.SideLeft => sideLeftModel,
            _ => frontModel
        };

        if (nextModel == currentModel) return;

        if (!primeraVez)
        {
            currentAnimator.Rebind();
        }
        else
        {
            primeraVez = false;
        }

        frontModel.SetActive(false);
        backModel.SetActive(false);
        sideRightModel.SetActive(false);
        sideLeftModel.SetActive(false);

        nextModel.SetActive(true);
        currentModel = nextModel;
        currentAnimator = currentModel.GetComponent<Animator>();

        // SOLO reconfigura los colliders si NO estás atacando owo~
        if (!ToolAttack && (isPickaxe || isLance || isAxe))
        {
            Transform[] children = currentModel.GetComponentsInChildren<Transform>(true);

            foreach (Transform child in children)
            {
                string name = child.name.ToLower();

                if (name.Equals("pickaxe"))
                {
                    child.gameObject.SetActive(isPickaxe);
                    if (isPickaxe)
                    {
                        currentWeapon = child.gameObject.GetComponent<Collider2D>();
                        currentWeapon.enabled = false;
                    }
                }
                else if (name.Equals("axe"))
                {
                    child.gameObject.SetActive(isAxe);
                    if (isAxe)
                    {

                        currentWeapon = child.gameObject.GetComponent<Collider2D>();
                        currentWeapon.enabled = false;
                    }
                }
                else if (name.Equals("lance"))
                {
                    child.gameObject.SetActive(isLance);
                    if (isLance) 
                    {

                        currentWeapon = child.gameObject.GetComponent<Collider2D>();
                        currentWeapon.enabled = false;
                    }
                }
            }
        }
        else if (!isAxe && !isLance && !isPickaxe) {
            Transform[] children = currentModel.GetComponentsInChildren<Transform>(true);

            foreach (Transform child in children)
            {
                string name = child.name.ToLower();

                if (name.Equals("pickaxe"))
                {
                    child.gameObject.SetActive(false);
                }
                else if (name.Equals("axe"))
                {
                    child.gameObject.SetActive(false);
                }
                else if (name.Equals("lance"))
                {
                    child.gameObject.SetActive(false);
                }
            }
        }

        if (esInvisible)
            StartCoroutine(FadeInvisibilidad(true));
    }




    private void SetWalking(bool walking)
    {
        if (currentAnimator != null)
        {
            currentAnimator.SetBool("IsWalking", walking);
        }
    }

    // ╔═ Métodos de Invisibilidad ═══════════════════════════════════╗
    private IEnumerator FadeInvisibilidad(bool invisible)
    {
        float targetAlpha = invisible ? 0f : 1f;
        float duration = 0.4f;
        float time = 0f;

        // Reunir todos los SpriteRenderer de los modelos
        List<SpriteRenderer> renderers = new List<SpriteRenderer>();
        renderers.AddRange(frontModel.GetComponentsInChildren<SpriteRenderer>(true));
        renderers.AddRange(backModel.GetComponentsInChildren<SpriteRenderer>(true));
        renderers.AddRange(sideRightModel.GetComponentsInChildren<SpriteRenderer>(true));
        renderers.AddRange(sideLeftModel.GetComponentsInChildren<SpriteRenderer>(true));

        // Guardar alfas iniciales
        Dictionary<SpriteRenderer, float> startAlphas = new Dictionary<SpriteRenderer, float>();
        foreach (var sr in renderers)
        {
            if (sr != null)
                startAlphas[sr] = sr.color.a;
        }

        // Interpolar alfas durante la duración
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

        // Asegurar valor final
        foreach (var sr in renderers)
        {
            if (sr == null) continue;
            Color c = sr.color;
            c.a = targetAlpha;
            sr.color = c;
        }
    }

    // ╔═ Métodos Auxiliares ══════════════════════════════════════════╗
    private void BorrarTextoDebug()
    {
        if (textoDebug != null)
            textoDebug.text = string.Empty;
    }

    // ╔═ Métodos Herramientas ══════════════════════════════════════════╗
    public void EquipWeapon(int weaponType)
    {
        isPickaxe = false;
        isAxe = false;
        isLance = false;

        switch (weaponType)
        {
            case 0: isAxe = true; break;
            case 1: isPickaxe = true; break;
            case 2: isLance = true; break;
        }

        if(isPickaxe || isLance || isAxe)
        {
            Transform[] children = currentModel.GetComponentsInChildren<Transform>(true);

            foreach (Transform child in children)
            {
                string name = child.name.ToLower();

                if (name.Equals("pickaxe"))
                {
                    child.gameObject.SetActive(isPickaxe);
                    if (isPickaxe)
                    {
                        currentWeapon = child.gameObject.GetComponent<Collider2D>();
                        currentWeapon.enabled = false; // Desactivar el collider nwn
                    }
                }
                else if (name.Equals("axe"))
                {
                    child.gameObject.SetActive(isAxe);

                    if (isAxe)
                    {
                        currentWeapon = child.gameObject.GetComponent<Collider2D>();
                        currentWeapon.enabled = false; // Desactivar el collider nwn
                    }
                }
                else if (name.Equals("lance"))
                {
                    child.gameObject.SetActive(isLance);

                    if (isLance)
                    {
                        currentWeapon = child.gameObject.GetComponent<Collider2D>();
                        currentWeapon.enabled = false; // Desactivar el collider nwn
                    }
                }
            }
        }
        else
        {
            Transform[] children = currentModel.GetComponentsInChildren<Transform>(true);

            foreach (Transform child in children)
            {
                string name = child.name.ToLower();

                if (name.Equals("pickaxe"))
                {
                    child.gameObject.SetActive(false);
                }
                else if (name.Equals("axe"))
                {
                    child.gameObject.SetActive(false);
                }
                else if (name.Equals("lance"))
                {
                    child.gameObject.SetActive(false);
                }
            }
        }
    }
    public bool EstaInvisible()
    {
        return esInvisible;
    }



    public void TakeDamage(float damage)
    {
        Debug.Log("TakeDamage ejecutado");

        if (esInvisible || isDead)
        {
            Debug.Log("Pero el jugador está invisible o muerto, así que no recibe daño.");
            return;
        }

        currentHealth -= damage;
        Debug.Log($"Recibió {damage} de daño. Vida restante: {currentHealth}");

        // Reproducir sonido aleatorio de daño
        if (damageAudio != null)
        {
            AudioClip clip = (Random.value > 0.5f) ? hit1Clip : hit2Clip;
            damageAudio.PlayOneShot(clip);
        }

        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            Die();
        }
    }



    private void Die()
    {
        isDead = true;
        Debug.Log("🐱 El michi ha muerto.");
        rb.velocity = Vector2.zero;
        enabled = false;

        if (damageAudio != null && deathClip != null)
        {
            damageAudio.PlayOneShot(deathClip);
        }

    }

    // ╔═ Métodos de Efectos de Estado (Slow) ═══════════════════════════╗
    private bool isSlowed = false;
    private float slowTimer = 0f;
    private float originalSpeed;
    private float originalRunSpeed;

    public void ApplySlow(float factor, float duration)
    {
        if (isSlowed || isDead) return;

        isSlowed = true;
        originalSpeed = speed;
        originalRunSpeed = runSpeed;

        speed *= factor;
        runSpeed *= factor;

        Debug.Log($"Jugador ralentizado: factor {factor}, duración {duration}s");

        StartCoroutine(RemoveSlowAfter(duration));
    }

    private IEnumerator RemoveSlowAfter(float duration)
    {
        yield return new WaitForSeconds(duration);

        speed = originalSpeed;
        runSpeed = originalRunSpeed;
        isSlowed = false;

        Debug.Log("Ralentización eliminada. Velocidad restaurada.");
    }


}
