using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Lander : MonoBehaviour
{
    // 角色主要逻辑
    private const float GRAVITY__NORMAL = 0.7f;
    public static Lander Instance { get; private set; }

    [SerializeField] private Rigidbody2D landerRigidbody2D;
    [SerializeField] private float upForce;
    [SerializeField] private float turnForce;
    private float fuelAmount = 10f;
    private float fuelAmountMax = 10f;

    public event EventHandler OnUpForce;
    public event EventHandler OnLeftForce;
    public event EventHandler OnRightForce;
    public event EventHandler OnBeforeForce;
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }
    public event EventHandler OnCoinPickup;
    public event EventHandler OnFuelPickup;
    public event EventHandler<OnLandedEventArgs> OnLanded;
    public class OnLandedEventArgs : EventArgs
    {
        public LandingType landingType;
        public int score;
        public float dotVector;
        public float landingSpeed;
        public float scoreMultiplier;
    }

    // 着陆可能结果
    public enum LandingType
    {
        Success,
        WrongLandingArea,
        TooSteepAngle,
        TooFastLanding,
    }

    // 角色状态
    private State state;
    public enum State
    {
        WaitingToStart,
        Normal,
        GameOver,
    }

    [SerializeField] private float gamePadDeadzon = .4f;

    private void Awake()
    {
        Instance = this;

        fuelAmount = fuelAmountMax;
        state = State.WaitingToStart;
        landerRigidbody2D.gravityScale = 0f;
    }

    private void FixedUpdate()
    {
        OnBeforeForce?.Invoke(this, EventArgs.Empty);

        // 移动时若角色状态为WaitingToStart则先启动重力、并修改角色状态、调用相关事件
        switch (state)
        {
            default:
            case State.WaitingToStart:
                //Keyboard.current.leftArrowKey.isPressed
                if (GameInput.Instance.IsUpActionPressed() ||
                    GameInput.Instance.IsLeftActionPressed() ||
                    GameInput.Instance.IsRightActionPressed() ||
                    GameInput.Instance.GetMovementInputVector2() != Vector2.zero)
                {
                    landerRigidbody2D.gravityScale = GRAVITY__NORMAL;
                    SetState(State.Normal);

                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                    {
                        state = state,
                    });
                }
                break;
            case State.Normal:
                if (fuelAmount <= 0)
                {
                    return;
                }

                if (GameInput.Instance.IsUpActionPressed() ||
                    GameInput.Instance.IsLeftActionPressed() ||
                    GameInput.Instance.IsRightActionPressed() ||
                    GameInput.Instance.GetMovementInputVector2() != Vector2.zero)
                {
                    ConsumeFuel();
                }

                if (GameInput.Instance.IsUpActionPressed() || GameInput.Instance.GetMovementInputVector2().y > gamePadDeadzon)
                {
                    landerRigidbody2D.AddForce(upForce * transform.up * Time.deltaTime);
                    OnUpForce?.Invoke(this, EventArgs.Empty);
                }
                if (GameInput.Instance.IsLeftActionPressed() || GameInput.Instance.GetMovementInputVector2().x < -gamePadDeadzon)
                {
                    landerRigidbody2D.AddTorque(turnForce * Time.deltaTime);
                    OnLeftForce?.Invoke(this, EventArgs.Empty);
                }
                if (GameInput.Instance.IsRightActionPressed()|| GameInput.Instance.GetMovementInputVector2().x > gamePadDeadzon)
                {
                    landerRigidbody2D.AddTorque(-turnForce * Time.deltaTime);
                    OnRightForce?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.GameOver:
                break;
        }

        
    }

    private void OnCollisionEnter2D(Collision2D collision2D)
    {
        // 撞上障碍
        if (!collision2D.gameObject.TryGetComponent(out LandingPad landingPad))
        {
            Debug.Log("撞击！");
            OnLanded?.Invoke(this, new OnLandedEventArgs
            {
                landingType = LandingType.WrongLandingArea,
                dotVector = 0,
                landingSpeed = 0,
                scoreMultiplier = 0,
                score = 0,
            });
            SetState(State.GameOver);
            return;
        }

        // 着陆速度太快
        float softLandingVelocityMagnitude = 4f;
        float relativaVelocityMagnitude = collision2D.relativeVelocity.magnitude;
        if (relativaVelocityMagnitude > softLandingVelocityMagnitude)
        {

            Debug.Log("着陆速度太快！");
            OnLanded?.Invoke(this, new OnLandedEventArgs
            {
                landingType = LandingType.TooFastLanding,
                dotVector = 0,
                landingSpeed = relativaVelocityMagnitude,
                scoreMultiplier = 0,
                score = 0,
            });
            SetState(State.GameOver);

            return;
        }

        // 着陆角度不对
        float dotVector = Vector2.Dot(Vector2.up, transform.up);
        float minDotVector  = 0.9f;
        if (dotVector < minDotVector)
        {

            Debug.Log("着陆角度太大！"); 
            OnLanded?.Invoke(this, new OnLandedEventArgs
            {
                landingType = LandingType.TooSteepAngle,
                dotVector = dotVector,
                landingSpeed = relativaVelocityMagnitude,
                scoreMultiplier = 0,
                score = 0,
            });
            SetState(State.GameOver);

            return;
        }

        Debug.Log("着陆成功！");

        // 分数计算
        float maxScoreAmountLandingAngle = 100;
        float scoreDotVectorMultiplier = 10f;
        float landingAngleScore = maxScoreAmountLandingAngle - Mathf.Abs(dotVector - 1f) * scoreDotVectorMultiplier * maxScoreAmountLandingAngle;

        float maxScoreAmountLandingSpeed = 100;
        float landingSpeedScore = (softLandingVelocityMagnitude - relativaVelocityMagnitude) * maxScoreAmountLandingSpeed;

        int score = Mathf.RoundToInt((landingAngleScore +  landingSpeedScore) * landingPad.GetScoreMultiplier());
        Debug.Log("Score: " + score);

        OnLanded?.Invoke(this, new OnLandedEventArgs
        {
            landingType = LandingType.Success,
            dotVector = dotVector,
            landingSpeed = relativaVelocityMagnitude,
            scoreMultiplier = landingPad.GetScoreMultiplier(),
            score = score,
        });
        SetState(State.GameOver);

    }

    private void OnTriggerEnter2D(Collider2D collision2D)
    {
        // 拾取燃料
        if (collision2D.gameObject.TryGetComponent(out FuelPickup fuelPickup))
        {
            float addFuelAmount = 10f;
            fuelAmount += addFuelAmount;
            if (fuelAmount > fuelAmountMax)
            {
                fuelAmount = fuelAmountMax;
            }
            OnFuelPickup?.Invoke(this, EventArgs.Empty);
            fuelPickup.DestroySelf();
        }

        // 拾取金币
        if (collision2D.gameObject.TryGetComponent(out CoinPickup coinPickup))
        {
            OnCoinPickup?.Invoke(this, EventArgs.Empty);
            coinPickup.DestroySelf();
        }
    }

    // 设置角色状态
    private void SetState(State state)
    {
        this.state = state;
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
        {
            state = state,
        });
    }

    // 燃料消耗
    private void ConsumeFuel()
    {
        float fuelConsumptionAmount = 1f;
        fuelAmount -= fuelConsumptionAmount * Time.deltaTime;
    }

    // 燃料值归一化
    public float GetFuelAmountNormalized()
    {
        return fuelAmount / fuelAmountMax;
    }

    // 获取燃料值
    public float GetFuel()
    {
        return fuelAmount;
    }

    // 获取速度
    public float GetSpeedX()
    {
        return landerRigidbody2D.linearVelocityX;
    }

    public float GetSpeedY()
    {
        return landerRigidbody2D.linearVelocityY;
    }
}
