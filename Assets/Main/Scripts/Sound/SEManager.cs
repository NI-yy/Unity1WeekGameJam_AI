using UnityEngine;

public class SEManager : MonoBehaviour
{
    [SerializeField] private AudioClip SE_click_to_start;
    [SerializeField] private AudioClip SE_ready;
    [SerializeField] private AudioClip SE_go;

    private AudioSource audioSource;

    public static SEManager Instance
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

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySE_ClickToStart()
    {
        audioSource.PlayOneShot(SE_click_to_start);
    }

    public void PlaySE_Ready()
    {
        audioSource.PlayOneShot(SE_ready);
    }

    public void PlayeSE_Go()
    {
        audioSource.PlayOneShot(SE_go);
    }
}
