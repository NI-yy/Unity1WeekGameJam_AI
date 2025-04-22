using UnityEngine;
using UnityEngine.UI;

public class SoundVolumeSetting : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider_BGM;
    [SerializeField] private Slider volumeSlider_SE;

    public static float volume_bgm_value = 0.5f;
    public static float volume_se_value = 1.0f;
    private BGMManager bgmManager;
    private SEManager SEManager;


    private void Awake()
    {
        volumeSlider_BGM.value = volume_bgm_value;
        volumeSlider_SE.value = volume_se_value;
    }

    private void Start()
    {
        bgmManager = BGMManager.Instance;
        SEManager = SEManager.Instance;
    }

    private void Update()
    {
        volume_bgm_value = volumeSlider_BGM.value;
        volume_se_value = volumeSlider_SE.value;

        bgmManager.volume_value = volume_bgm_value;
        SEManager.volume_value = volume_se_value;
    }
}
