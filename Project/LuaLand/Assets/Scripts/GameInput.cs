using UnityEngine;
using System;

public class GameInput : MonoBehaviour
{
    // 调用由新输入系统生成的InputAction来返回是否有相应输入，据此来执行相应代码
    public static GameInput Instance {  get; private set; }

    public event EventHandler OnMenuButtonPressed;

    private InputActions inputActions;

    private void Awake()
    {
        Instance = this;

        inputActions = new InputActions();

        // ***重中之重，漏写无法输入
        inputActions.Enable();

        inputActions.Player.Menu.performed += Menu_performed;
    }

    private void Menu_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnMenuButtonPressed?.Invoke(this, EventArgs.Empty);
    }

    private void OnDestroy()
    {
        inputActions.Disable();
    }

    public bool IsUpActionPressed()
    {
        return inputActions.Player.LanderUp.IsPressed();
    }

    public bool IsLeftActionPressed()
    {
        return inputActions.Player.LanderLeft.IsPressed();
    }

    public bool IsRightActionPressed()
    {
        return inputActions.Player.LanderRight.IsPressed();
    }

    public Vector2 GetMovementInputVector2()
    {
        return inputActions.Player.Movement.ReadValue<Vector2>();
    }
}
