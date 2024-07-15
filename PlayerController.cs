using UnityEngine;
using UnityEngine.InputSystem;
using Spine.Unity;

[RequireComponent(typeof(Player))]
public class PlayerController : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;
    public AnimationReferenceAsset idleAnimation;
    public AnimationReferenceAsset moveAnimation;
    public GameObject dashEffectPrefab;
    private GameObject dashEffectInstance;
    public Rigidbody2D rb;

    public float dashDuration = 0.5f;
    public float dashSpeed = 10.0f;
    public float dashCooldown = 1.0f;

    private bool isDashing = false;
    public Vector2 inputVec { get; private set; }

    private Player player;
    private string currentAnimationName;
    private bool canDash = true;

    public Vector3 dashEffectOffset = new Vector3(0f, -0.5f, 0f);

    void Start()
    {
        player = GetComponent<Player>();
        SetIdleAnimation();
    }

    public void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();
    }

    public void OnDash(InputValue value)
    {
        if (value.isPressed && inputVec != Vector2.zero && canDash && !isDashing)
        {
            StartCoroutine(PerformDash());
        }
    }

    System.Collections.IEnumerator DestroyDashEffectAfterAnimation(GameObject effectInstance, float duration)
    {
        yield return new WaitForSeconds(duration);
        Destroy(effectInstance);
    }

    System.Collections.IEnumerator PerformDash()
    {
        isDashing = true;


        Vector2 initialVelocity = rb.velocity;

        Vector3 dashDirection = new Vector3(inputVec.x, inputVec.y, 0).normalized;


        float angleOffset = 180f;
        float angle = Mathf.Atan2(dashDirection.y, dashDirection.x) * Mathf.Rad2Deg + angleOffset;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);


        Vector3 dashEffectPosition = transform.position + dashDirection * -dashEffectOffset.magnitude;
        dashEffectInstance = Instantiate(dashEffectPrefab, dashEffectPosition, rotation);


        Animator dashEffectAnimator = dashEffectInstance.GetComponent<Animator>();
        if (dashEffectAnimator)
        {
            dashEffectAnimator.SetTrigger("DoDash");
            StartCoroutine(DestroyDashEffectAfterAnimation(dashEffectInstance, dashEffectAnimator.GetCurrentAnimatorStateInfo(0).length));
        }


        float dashEndTime = Time.time + dashDuration;


        rb.velocity = dashDirection * dashSpeed;

        while (Time.time < dashEndTime)
        {
            yield return null;
        }


        rb.velocity = initialVelocity;

        isDashing = false; 


        yield return new WaitForSeconds(dashCooldown);
        canDash = true; 
    }

    void Update()
    {
        if (inputVec.x > 0.01f)
        {
            skeletonAnimation.skeleton.ScaleX = 1f;
        }
        else if (inputVec.x < -0.01f)
        {
            skeletonAnimation.skeleton.ScaleX = -1f;
        }

        if (Mathf.Abs(inputVec.x) > 0.01f || Mathf.Abs(inputVec.y) > 0.01f)
        {
            if (currentAnimationName != moveAnimation.name)
            {
                SetMoveAnimation();
            }
        }
        else if (currentAnimationName != idleAnimation.name)
        {
            SetIdleAnimation();
        }
    }

    void FixedUpdate()
    {
        if (!isDashing)
        {
            Vector2 movement = inputVec.normalized * player.MoveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + movement);
        }
    }

    void SetIdleAnimation()
    {
        skeletonAnimation.AnimationState.SetAnimation(0, idleAnimation, true);
        currentAnimationName = idleAnimation.name;
    }

    void SetMoveAnimation()
    {
        skeletonAnimation.AnimationState.SetAnimation(0, moveAnimation, true);
        currentAnimationName = moveAnimation.name;
    }
}
