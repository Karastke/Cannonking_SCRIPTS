using System.Collections;
using UnityEngine;
using Spine.Unity;

public class BallCollision : MonoBehaviour
{
    public SkeletonDataAsset explosionSkeletonData;
    [Tooltip("Name of the explosion animation.")]
    public string explosionAnimationName = "explosion";

    private Bounce_Controller bounceController;
    private ObjectPool objectPool;

    private void Start()
    {
        bounceController = GetComponent<Bounce_Controller>();
        objectPool = ObjectPool.Instance;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemies"))
        {
            HandleEnemyCollision();
            return;
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") || (bounceController.bounceCount >= bounceController.maxBounceCount))
        {
            if (gameObject.activeSelf)
            {
                StartCoroutine(Explode());
            }
        }
    }

    public void TriggerExplosion()
    {
        if (gameObject.activeSelf)
        {
            StartCoroutine(Explode());
        }
    }

    private void HandleEnemyCollision()
    {
        // TODO: Handle collision with enemies
        Debug.LogWarning("Collision with enemy detected. Needs handling.");
    }

    private IEnumerator Explode()
    {
        yield return new WaitForEndOfFrame();

        CreateExplosionEffect();
        objectPool.ReturnToPool(gameObject);
    }

    private void CreateExplosionEffect()
    {
        GameObject explosion = objectPool.GetFromPool("Effects", transform.position, Quaternion.identity);
        if (explosion == null) return;

        SkeletonAnimation explosionAnimation = explosion.GetComponent<SkeletonAnimation>();
        if (explosionAnimation == null)
        {
            Debug.LogError("SkeletonAnimation component is missing in the explosion prefab.");
            return;
        }

        SetAndPlayAnimation(explosion, explosionAnimation);
    }

    private void SetAndPlayAnimation(GameObject explosion, SkeletonAnimation explosionAnimation)
    {
        explosionAnimation.skeletonDataAsset = explosionSkeletonData;
        explosionAnimation.Initialize(true);
        explosionAnimation.AnimationState.SetAnimation(0, explosionAnimationName, false);

        explosionAnimation.AnimationState.Complete += (trackEntry) =>
        {
            objectPool.ReturnToPool(explosion);
        };
    }
}