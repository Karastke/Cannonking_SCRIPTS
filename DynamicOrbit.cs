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
    public float reflectionSpeed = 5.0f; // Set your desired speed

    private float orbitRadius;
    private bool isFireButtonPressed = false;
    private float fireButtonPressDuration = 0f;
    private float fireButtonPressStartTime;
    public float maxHoldTime; // 최대 유지 시간
    public float minHoldTime; // 최소 유지 시간
    private ParryingState currentParryingState = ParryingState.None;
    private Rigidbody2D projectileToReflect;
    private bool shouldReflectProjectile = false;
    public float perfectParryingSpeed = 10f; // Speed for perfect parrying
    public float normalParryingSpeed = 5f;   // Speed for normal parrying

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
        playerControls = new PlayerControl(); // Initialize playerControls here
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject projectile = collision.gameObject;
        if (projectile.CompareTag("EnemyBullet"))
        {
            projectile.SetActive(false);
            GameObject newProjectile = Instantiate(playerBallPrefab, projectile.transform.position, Quaternion.identity);
            Rigidbody2D newProjectileRigidbody = newProjectile.GetComponent<Rigidbody2D>();

            if (newProjectileRigidbody != null)
            {
                float speed = (currentParryingState == ParryingState.Perfect) ? perfectParryingSpeed : normalParryingSpeed;
                ReflectProjectile(newProjectileRigidbody, speed);
            }
        }
    }

    IEnumerator ReenablePhysics(Rigidbody2D rb)
    {
        yield return new WaitForSeconds(0.1f); // Adjust delay as needed
        rb.isKinematic = false;
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
        };

        playerControls.Player.Fire.canceled += ctx =>
        {
            isFireButtonPressed = false;
            fireButtonPressDuration = Time.realtimeSinceStartup - fireButtonPressStartTime;
            Debug.Log($"버튼 누른 시간:{fireButtonPressDuration}");

            if (fireButtonPressDuration < minHoldTime)
            {
                Debug.Log("Too short!");
                barCollider.enabled = false;
                barRigidbody.isKinematic = true;
                DOTween.To(() => orbitRadius, x => orbitRadius = x, defaultOrbitRadius, 0.3f).SetEase(Ease.Linear);
            }

            else if (fireButtonPressDuration >= 0.5f && fireButtonPressDuration <= 0.6f)
            {
                Debug.Log("Perfect Parrying!");
                currentParryingState = ParryingState.Perfect;
                barCollider.enabled = true;
                barRigidbody.isKinematic = false;
                ReflectProjectile(projectileRigidbody, perfectParryingSpeed);

                Sequence s = DOTween.Sequence();
                s.Append(DOTween.To(() => orbitRadius, x => orbitRadius = x, defaultOrbitRadius, 0.5f).SetEase(Ease.OutElastic, overshoot: 4));
                s.AppendInterval(0.01f); // Adjust this delay as needed
                s.OnComplete(() =>
                {
                    barCollider.enabled = false;
                    barRigidbody.isKinematic = true;
                });
                s.Play();
            }

            else
            {
                Debug.Log("Normal Parrying");
                currentParryingState = ParryingState.Normal;
                barCollider.enabled = true;
                barRigidbody.isKinematic = false;
                ReflectProjectile(projectileRigidbody, normalParryingSpeed);


                Sequence s = DOTween.Sequence();
                s.Append(DOTween.To(() => orbitRadius, x => orbitRadius = x, defaultOrbitRadius, 0.6f).SetEase(Ease.OutElastic, overshoot: 2));

                // Add a delay and then disable components
                s.AppendInterval(0.01f); // Adjust this delay as needed
                s.OnComplete(() =>
                {
                    barCollider.enabled = false;
                    barRigidbody.isKinematic = true;
                });
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
                orbitRadius -= Time.deltaTime; // Adjust the rate of decrease here
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
