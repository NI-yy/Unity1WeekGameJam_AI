using UnityEngine;

public class SEManager : MonoBehaviour
{
    [SerializeField] private AudioClip SE_click_to_start;
    [SerializeField] private AudioClip SE_ready;
    [SerializeField] private AudioClip SE_go;
    [SerializeField] private AudioClip SE_parry;
    [SerializeField] private AudioClip SE_dash;

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

    public void PlaySE_Parry(float volume = 1.0f)
    {
        audioSource.PlayOneShot(SE_parry, volume);
    }

    public void PlaySE_Dash()
    {
        audioSource.PlayOneShot(SE_dash);
    }
}
