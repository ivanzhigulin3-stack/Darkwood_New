using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Mover))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Dodge Settings")]
    [SerializeField] private float dodgeSpeed = 15f;    // Скорость рывка
    [SerializeField] private float dodgeDuration = 0.25f; // Длительность рывка
    [SerializeField] private float dodgeStaminaCost = 30f; // Стоимость стамины
    [SerializeField] private KeyCode dodgeKey = KeyCode.Space; // Кнопка

    private bool isDodging = false;

    public float normalSpeed = 5f;
    public float sprintSpeed = 8f;
    [SerializeField] private float sprintStaminaCost = 20f;

    private float currentSpeed;
    private Vector2 moveInput;

    private Mover mover;
    private PlayerStamina playerStamina;

    void Start()
    {
        mover = GetComponent<Mover>();
        playerStamina = GetComponent<PlayerStamina>();
        currentSpeed = normalSpeed;
    }

    void Update()
    {
        if (isDodging) return;

        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput = moveInput.normalized;

        if (Input.GetKey(KeyCode.LeftShift) && moveInput.magnitude > 0 && playerStamina != null)
        {
            if (playerStamina.UseStamina(sprintStaminaCost * Time.deltaTime))
            {
                currentSpeed = sprintSpeed;
            }
            else
            {
                currentSpeed = normalSpeed;
            }
        }
        else
        {
            currentSpeed = normalSpeed;
        }

        if (Input.GetKeyDown(dodgeKey) && !isDodging && moveInput.magnitude > 0 && playerStamina != null)
        {
            if (playerStamina.UseStamina(dodgeStaminaCost)) // Проверяем стамину
            {
                StartCoroutine(DodgeRoutine());
            }
        }

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mover.RotateTowards(mousePosition);
    }

    void FixedUpdate()
    {
        float staminaModifier = playerStamina != null ? playerStamina.GetSpeedMultiplier() : 1f;

        mover.Move(moveInput, currentSpeed * staminaModifier);
    }


    private IEnumerator DodgeRoutine()
    {
        isDodging = true;
        currentSpeed = dodgeSpeed;

        // Отключаем физические столкновения, чтобы пролетать сквозь врагов (если у тебя есть слои)
        gameObject.layer = LayerMask.NameToLayer("Invincible"); 

        yield return new WaitForSeconds(dodgeDuration); // Ждем время рывка

        // Возвращаем физику обратно
        gameObject.layer = LayerMask.NameToLayer("Player"); 

        isDodging = false;
        currentSpeed = normalSpeed; // Возвращаем скорость
    }
}