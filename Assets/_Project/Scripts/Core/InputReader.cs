using System;
using UnityEngine;

public class InputReader : MonoBehaviour
{
    private const string HorizontalAxis = "Horizontal";
    private const string VerticalAxis = "Vertical";
    private const string MouseX = "Mouse X";
    private const string MouseY = "Mouse Y";

    private const KeyCode CommandJump = KeyCode.Space;
    private const KeyCode CommandSprint = KeyCode.LeftShift;
    private const KeyCode CommandCrouch = KeyCode.LeftControl;
    private const KeyCode CommandEscape = KeyCode.Escape;

    public event Action<Vector2> MovementPressed;
    public event Action<Vector2> RotationPressed;
    public event Action JumpPressed;
    public event Action SprintPressed;
    public event Action SprintUnpressed;
    public event Action CrouchPressed;
    public event Action CrouchUnpressed;
    public event Action EscapePressed;

    private void Update()
    {
        ReadRotation();
        ReadDown(CommandJump, JumpPressed);
        ReadDown(CommandSprint, SprintPressed);
        ReadUp(CommandSprint, SprintUnpressed);
        ReadDown(CommandCrouch, CrouchPressed);
        ReadUp(CommandCrouch, CrouchUnpressed);
        ReadDown(CommandEscape, EscapePressed);
    }

    private void FixedUpdate() =>
        ReadMovement();

    private void ReadMovement()
    {
        float horizontal = Input.GetAxisRaw(HorizontalAxis);
        float vertical = Input.GetAxisRaw(VerticalAxis);
        Vector2 movementInput = new(horizontal, vertical);

        MovementPressed?.Invoke(movementInput);
    }

    private void ReadRotation()
    {
        float mouseX = Input.GetAxisRaw(MouseX);
        float mouseY = Input.GetAxisRaw(MouseY);

        Vector2 rotationInput = new(mouseX, mouseY);

        if (rotationInput != Vector2.zero)
            RotationPressed?.Invoke(rotationInput);
    }

    private void ReadDown(KeyCode key, Action action)
    {
        if (Input.GetKeyDown(key))
            action?.Invoke();
    }

    private void ReadUp(KeyCode key, Action action)
    {
        if (Input.GetKeyUp(key))
            action?.Invoke();
    }
}