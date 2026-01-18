using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class Movement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private Transform towerCenter;
    [SerializeField] private float radius = 1.5f;
    [SerializeField] private float rotationSpeed = 45f; // скорость вращения вокруг башни
    [SerializeField] private float animationBlendSpeed = 5f;

    [Header("Stability")]
    [SerializeField, Tooltip("Игнорировать ввод меньший этого значения")] private float inputDeadzone = 0.05f;
    [SerializeField, Tooltip("Если вычисленный аниматорный speed < этого — установить 0")] private float snapThreshold = 0.01f;

    private CharacterController _controller;
    private Animator _animator;
    private float currentAngle = 0f;

    private float _currentSpeed = 0f;
    private int _speedHash;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();

        // Ищем аниматор на дочернем объекте
        _animator = GetComponentInChildren<Animator>();
        if (_animator == null)
            Debug.LogError("❌ Animator не найден в дочерних объектах! Добавь его на Chiken_animation.");

        _speedHash = Animator.StringToHash("speed");

        if (towerCenter != null)
            InitializePosition();
    }

    private void OnEnable() => moveAction.action.Enable();
    private void OnDisable() => moveAction.action.Disable();

    private void InitializePosition()
    {
        bool hadController = _controller != null && _controller.enabled;
        if (hadController) _controller.enabled = false;

        Vector3 directionToCenter = transform.position - towerCenter.position;
        directionToCenter.y = 0f;
        if (directionToCenter.sqrMagnitude < 0.0001f)
            directionToCenter = transform.forward;
        directionToCenter.Normalize();

        transform.position = new Vector3(
            towerCenter.position.x + directionToCenter.x * radius,
            towerCenter.position.y,
            towerCenter.position.z + directionToCenter.z * radius
        );

        currentAngle = Mathf.Atan2(directionToCenter.z, directionToCenter.x) * Mathf.Rad2Deg;

        if (hadController) _controller.enabled = true;
    }

    private void Update()
    {
        HandleMovement();
        UpdateAnimations();
    }

    private void HandleMovement()
    {
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        float horizontalInput = input.x;

        if (Mathf.Abs(horizontalInput) < inputDeadzone)
        {
            _currentSpeed = 0f;
            return;
        }

        _currentSpeed = 1f;

        float angleDelta = horizontalInput * rotationSpeed * Time.deltaTime;
        currentAngle += angleDelta;
        currentAngle = Mathf.Repeat(currentAngle, 360f);

        float angleRad = currentAngle * Mathf.Deg2Rad;
        Vector3 newPosition = new Vector3(
            towerCenter.position.x + radius * Mathf.Cos(angleRad),
            transform.position.y,
            towerCenter.position.z + radius * Mathf.Sin(angleRad)
        );

        Vector3 moveDelta = newPosition - transform.position;
        _controller.Move(moveDelta);

        Vector3 tangent = new Vector3(-Mathf.Sin(angleRad), 0f, Mathf.Cos(angleRad));
        if (tangent != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(tangent);
    }

    private void UpdateAnimations()
    {
        if (_animator == null) return; // safety check

        float currentAnimSpeed = _animator.GetFloat(_speedHash);
        float newSpeed = Mathf.MoveTowards(currentAnimSpeed, _currentSpeed, animationBlendSpeed * Time.deltaTime);

        if (Mathf.Abs(newSpeed) < snapThreshold)
            newSpeed = 0f;

        _animator.SetFloat(_speedHash, newSpeed);
    }
}

