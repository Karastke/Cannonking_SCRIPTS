using System.Collections;
using UnityEngine;
using Spine.Unity;

public class PlayerBulletExplosion : MonoBehaviour
{
    public GameObject explosionPrefab;
    public SkeletonDataAsset explosionSkeletonData;
    public string explosionAnimationName = "explosion";
    private Bounce_Controller bounceController;

    private void Start()
    {
        bounceController = GetComponent<Bounce_Controller>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemies") || (bounceController.bounceCount >= bounceController.maxBounceCount))
        {
            StartCoroutine(Explode());
        }
    }

    public IEnumerator Explode()
    {
        yield return new WaitForEndOfFrame();
        
        // Instantiate the explosion effect
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        SkeletonAnimation explosionAnimation = explosion.GetComponent<SkeletonAnimation>();
        
        // 오브젝트 풀로 반환
        ObjectPool.Instance.ReturnToPool(gameObject);

        // Set the Spine animation
        explosionAnimation.skeletonDataAsset = explosionSkeletonData;
        explosionAnimation.Initialize(true);
        explosionAnimation.AnimationState.SetAnimation(0, explosionAnimationName, false);

        // Add a listener for the animation complete event using a lambda expression
        explosionAnimation.AnimationState.Complete += (trackEntry) =>
        {
            // Destroy the explosion GameObject
            Destroy(explosion);
        };
    }
}
