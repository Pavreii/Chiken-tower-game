using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class JumpScript : MonoBehaviour
{
    [SerializeField] private Inputs inputs;           // источник ввода
    [SerializeField] private float jumpHeight = 2f;   // высота прыжка
    [SerializeField] private float gravity = -9.81f;  // сила гравитации

    private CharacterController controller;
    private float verticalVelocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        bool grounded = controller.isGrounded;

        // Прыжок только если на земле и нажата кнопка
        if (grounded && inputs.jump)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            inputs.jump = false; // сразу сбрасываем, чтобы не копилось
        }

        // Если стоим на земле и скорость вниз — прижимаем
        if (grounded && verticalVelocity < 0)
            verticalVelocity = -2f;

        // Гравитация
        verticalVelocity += gravity * Time.deltaTime;

        // Движение только по вертикали
        Vector3 move = new Vector3(0, verticalVelocity, 0);
        controller.Move(move * Time.deltaTime);
    }
}
