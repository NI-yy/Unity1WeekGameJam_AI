using UnityEngine;
using UnityEngine.UI;

public class SoundVolumeSetting : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider_BGM;
    [SerializeField] private Slider volumeSlider_SE;

    private float volume_bgm_value;
    private float volume_se_value;
    private BGMManager bgmManager;
    private SEManager SEManager;

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
