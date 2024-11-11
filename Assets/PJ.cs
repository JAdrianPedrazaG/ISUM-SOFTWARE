using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PJ : MonoBehaviour
{

    // CONTROL DE MOVIMIENTO
    [Header("Configuracion Movimiento Horizontal")]
    [SerializeField] private float walkSpeed = 10;

    [Header("Configuracion Movimiento Vertical")]
    [SerializeField] private float jumpForce = 12f;
    private int jumpBufferCounter = 0;
    [SerializeField] private int jumpBufferFrames;
    private float coyoteTimeCounter = 0;
    [SerializeField] private float coyoteTime;
    private int airJumpCounter = 0;
    [SerializeField] private int maxAirJumps;


    // CONTROL DE SUELO
    [Header("Configuracion Verificacion Terreno")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckY = 0.2f;
    [SerializeField] private float groundCheckX = 0.5f;
    [SerializeField] private LayerMask whatIsGround;

    // INICIALIZAR VARIABLES
    private PlayerStateList pState;
    private Rigidbody2D rb;
    private float xAxis;
    private Animator anim;
    private SpriteRenderer spritR;
    private Collider2D enSuelo;

    // CONEXION CON EL MOVIMIENTO DE CAMARA
    public static PJ Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    
    void Start()
    {
        pState = GetComponent<PlayerStateList>(); // Hacer referencia a los estados del jugador
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spritR = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        GetInputs();
        UpdateJumpVariables();
        Flip();
        Move();
        Jump();
        UpdateAnimations();
    }

    void GetInputs()
    {
        // Obtener entrada horizontal
        xAxis = Input.GetAxisRaw("Horizontal");
    }

    private void Move()
    {
        // Movimiento horizontal
        rb.velocity = new Vector2(walkSpeed * xAxis, rb.velocity.y);
    }

    private void Jump()
    {
        // Permite al jugador cancelar el salto cuando suelta la tecla
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            pState.jumping = false;
        }

        // Salto
        if (!pState.jumping)
        {
            if (jumpBufferCounter > 0 && coyoteTimeCounter > 0) // el JBC debe ser mayor a 0 y el jugador debe estar en el suelo para saltar
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                pState.jumping = true;
            }
            else if(!Grounded() && airJumpCounter < maxAirJumps && Input.GetButtonDown("Jump"))
            {
                pState.jumping = true;

                airJumpCounter++;

                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
        }

        // Inicia animacion de salto
        anim.SetBool("enSuelo", !Grounded());
    }

    void UpdateJumpVariables()
    {
        if (Grounded())
        {
            pState.jumping = false;
            coyoteTimeCounter = coyoteTime;
            airJumpCounter = 0;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
        
        // Establecer JBC en el JBF al maximo cuando el jugador oprime saltar
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferFrames;
        }
        else
        {
            jumpBufferCounter--;
        }
    }

    private void UpdateAnimations()
    {
        // Verificar si el personaje está en el suelo
        enSuelo = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckY, whatIsGround);

        // Animaciones de movimiento
        anim.SetBool("caminando", xAxis != 0);

        // Animaciones de salto y caída
        anim.SetFloat("velocidadY", rb.velocity.y);
        anim.SetBool("enSuelo", enSuelo);
    }

    private bool Grounded()
    {
        // Verificación de si el jugador está en el suelo
        return Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround);
    }

    private void Flip()
    {
        // Voltear al personaje según la dirección de movimiento
        spritR.flipX = xAxis < 0;
    }

    private void OnDrawGizmosSelected()
    {
        // Dibujar el área de verificación del suelo
        Gizmos.DrawSphere(groundCheckPoint.position, groundCheckY);
    }
}