using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }
    public Transform PlayerTransform { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        if (PlayerTransform == null)
        {
            Debug.LogError("여기에 플레이어 없음");
        }
    }

    public Vector3 GetPlayerPosition()
    {
        return PlayerTransform != null ? PlayerTransform.position : Vector3.zero;
    }
}