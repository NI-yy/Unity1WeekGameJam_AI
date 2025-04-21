using UnityEngine;

public class BGMManager : MonoBehaviour
{

    [HideInInspector] public float volume_value = 0.5f;

    private AudioSource audioSource;

    public static BGMManager Instance
    {
        get; private set;
    }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        audioSource.volume = volume_value;
    }
}
