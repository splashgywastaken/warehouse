using UnityEngine;

public interface IInputReader
{
    event System.Action<Vector2> OnMove;
    event System.Action OnJumpPressed;
    event System.Action OnJumpReleased;
    event System.Action OnDashPressed;
    event System.Action OnAttackPressed;
    event System.Action OnAttackReleased;

    void Enable();
    void Disable();
}
