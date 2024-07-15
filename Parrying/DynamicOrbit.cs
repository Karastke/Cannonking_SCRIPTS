using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class DynamicOrbit : MonoBehaviour
{
    public GameObject player;
    public Camera mainCamera;
    public float minOrbitRadius;
    public float maxOrbitRadius;
    public float defaultOrbitRadius;
    public Vector3 initialOffset;
    public PlayerControl playerControls;
    public Collider2D barCollider;
    public Rigidbody2D barRigidbody;
    public Rigidbody2D projectileRigidbody;
    public GameObject playerBallPrefab;
    public float reflectionSpeed = 5.0f;
    public float maxHoldTime;
    public float minHoldTime;
    public float perfectParryingSpeed = 10f;
    public float normalParryingSpeed = 5f;
    public float parryingDurationOffset = 0.1f; 

    private SpriteRenderer spriteRenderer;
    private float orbitRadius;
    private bool isFireButtonPressed = false;
    private float fireButtonPressDuration = 0f;
    private float fireButtonPressStartTime;
    private ParryingState currentParryingState = ParryingState.None;
    private bool colorChanged = false;
    private Color originalColor;
    private ObjectPool objectPool;

    private enum ParryingState
    {
        None,
        Normal,
        Perfect
    }

    void Start()
    {
        DOTween.Init();
    }

    void Awake()
    {
        mainCamera = Camera.main;
        orbitRadius = defaultOrbitRadius;
        playerControls = new PlayerControl();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        objectPool = ObjectPool.Instance;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        int enemyBallLayer = LayerMask.NameToLayer("EnemyBall");
        
        if (collision.gameObject.layer == enemyBallLayer)
        {
            GameObject enemyBall = collision.gameObject;
            
            // EnemyBall을 풀로 반환
            ObjectPool.Instance.ReturnToPool(enemyBall);

            // PlayerBall 생성
            CreatePlayerBall(enemyBall.transform.position);
        }
    }

    private IEnumerator PerformParrying(float sequenceDuration, ParryingState state)
    {
        currentParryingState = state;
        barCollider.enabled = true;
        barRigidbody.isKinematic = false;

        float parryingDuration = sequenceDuration - parryingDurationOffset;
        yield return new WaitForSeconds(parryingDuration);

        barCollider.enabled = false;
        barRigidbody.isKinematic = true;
        currentParryingState = ParryingState.None;
    }

      private void CreatePlayerBall(Vector3 position)
    {
        GameObject playerBall = ObjectPool.Instance.GetFromPool("PlayerBall", position, Quaternion.identity);

        if (playerBall != null)
        {
            

            ReflectedBulletDamage reflectedBullet = playerBall.GetComponent<ReflectedBulletDamage>();
            if (reflectedBullet != null)
            {
                reflectedBullet.ResetBullet();
                SetPlayerBallVelocity(reflectedBullet);
            }
            else
            {
                Debug.LogWarning("ReflectedBulletDamage 컴포넌트 못찾음 ");
            }

            // 새로운 투사체 활성화 (이미 GetFromPool에서 활성화되었을 수 있지만, 확실히 하기 위해)
            playerBall.SetActive(true);
        }
        else
        {
            Debug.LogWarning("poolManager return 실패함.");
        }
    }

     private void SetPlayerBallVelocity(ReflectedBulletDamage reflectedBullet)
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - (Vector2)reflectedBullet.transform.position).normalized;
        float speed = (currentParryingState == ParryingState.Perfect) ? perfectParryingSpeed : normalParryingSpeed;
        reflectedBullet.SetVelocity(direction * speed);
    }

     private void ResetPlayerBall(GameObject playerBall)
    {
        ReflectedBulletDamage reflectedBullet = playerBall.GetComponent<ReflectedBulletDamage>();
        Bounce_Controller bounceController = playerBall.GetComponent<Bounce_Controller>();
        Rigidbody2D rb = playerBall.GetComponent<Rigidbody2D>();

        if (reflectedBullet != null)
        {
            reflectedBullet.ResetBullet();
        }

        if (bounceController != null)
        {
            bounceController.ResetState();
        }

        if (rb != null)
        {
            float speed = (currentParryingState == ParryingState.Perfect) ? perfectParryingSpeed : normalParryingSpeed;
            ReflectProjectile(rb, speed);
        }
    }

    void OnEnable()
    {
        barCollider.enabled = false;
        barRigidbody.isKinematic = true;

        playerControls.Enable();
        playerControls.Player.Fire.performed += ctx =>
        {
            isFireButtonPressed = true;
            fireButtonPressStartTime = Time.realtimeSinceStartup;
            colorChanged = false;
        };

        playerControls.Player.Fire.canceled += ctx =>
        {
            isFireButtonPressed = false;
            fireButtonPressDuration = Time.realtimeSinceStartup - fireButtonPressStartTime;
            Debug.Log($"버튼 누른 시간:{fireButtonPressDuration}");

            spriteRenderer.color = originalColor;

            if (fireButtonPressDuration < minHoldTime)
            {
                Debug.Log("Too short!");
                barCollider.enabled = false;
                barRigidbody.isKinematic = true;
                DOTween.To(() => orbitRadius, x => orbitRadius = x, defaultOrbitRadius, 0.3f).SetEase(Ease.Linear);
            }
            else if (fireButtonPressDuration >= 0.45f && fireButtonPressDuration <= 0.6f)
            {
                Debug.Log("Perfect Parrying!");
                float sequenceDuration = 0.51f; // 0.5f + 0.01f
                StartCoroutine(PerformParrying(sequenceDuration, ParryingState.Perfect));

                Sequence s = DOTween.Sequence();
                s.Append(DOTween.To(() => orbitRadius, x => orbitRadius = x, defaultOrbitRadius, 0.5f).SetEase(Ease.OutElastic, overshoot: 4));
                s.AppendInterval(0.01f);
                s.Play();
            }
            else
            {
                Debug.Log("Normal Parrying");
                float sequenceDuration = 0.61f;
                StartCoroutine(PerformParrying(sequenceDuration, ParryingState.Normal));

                Sequence s = DOTween.Sequence();
                s.Append(DOTween.To(() => orbitRadius, x => orbitRadius = x, defaultOrbitRadius, 0.6f).SetEase(Ease.OutElastic, overshoot: 2));
                s.AppendInterval(0.01f);
                s.Play();
            }
        };
    }

    void OnDisable()
    {
        barCollider.enabled = true;
        barRigidbody.isKinematic = false;
        playerControls.Disable();
    }

    void Update()
    {
        if (isFireButtonPressed)
        {
            fireButtonPressDuration = Time.realtimeSinceStartup - fireButtonPressStartTime;

            if (orbitRadius > minOrbitRadius)
            {
                orbitRadius -= Time.deltaTime;
            }

            if (fireButtonPressDuration >= 0.45f && fireButtonPressDuration <= 0.55f && !colorChanged)
            {
                spriteRenderer.DOColor(Color.red, 0.1f).SetLoops(2, LoopType.Yoyo);
                colorChanged = true;
            }
        }

        UpdateOrbitPosition();
    }

    void ReflectProjectile(Rigidbody2D projectileRigidbody, float speed)
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 directionToMouse = (mousePosition - projectileRigidbody.position).normalized;
        projectileRigidbody.velocity = directionToMouse * speed;
    }

    private void UpdateOrbitPosition()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        mousePosition = mainCamera.ScreenToWorldPoint(mousePosition);

        Vector2 direction = new Vector2(mousePosition.x - player.transform.position.x, mousePosition.y - player.transform.position.y);
        direction.Normalize();

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Vector2 orbitPosition = Quaternion.Euler(0, 0, angle) * new Vector2(orbitRadius, 0);
        transform.position = (Vector3)player.transform.position + (Vector3)orbitPosition + initialOffset;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}