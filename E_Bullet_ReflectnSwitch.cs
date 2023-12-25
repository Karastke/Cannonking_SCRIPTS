using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_Bullet_ReflectnSwitch : MonoBehaviour
{
    public GameObject newObjectPrefab; // 교체할 새 오브젝트의 프리팹

    public void ReplaceAndReflect(GameObject originalObject, bool isPerfect)
    {
        if (originalObject != null && newObjectPrefab != null)
        {
            // 새 오브젝트 생성
            GameObject newObj = Instantiate(newObjectPrefab, originalObject.transform.position, Quaternion.identity);

            Rigidbody2D rbOld = originalObject.GetComponent<Rigidbody2D>();
            Rigidbody2D rbNew = newObj.GetComponent<Rigidbody2D>();

            if (rbOld != null && rbNew != null)
            {
                Vector2 currentDirection = rbOld.velocity;
                Vector2 newDirection = currentDirection * -1; // 방향 반전
                float reflectionPower = isPerfect ? 2.0f : 1.5f; // 완벽한 반사는 더 강력함

                rbNew.velocity = newDirection * reflectionPower; // 새 오브젝트에 반사 속도 적용
            }

            Destroy(originalObject); // 기존 오브젝트 제거
        }
    }
}
