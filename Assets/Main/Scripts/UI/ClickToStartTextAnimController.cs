using DG.Tweening;
using UnityEngine;
using TMPro;

public class ClickToStartTextAnimController : MonoBehaviour
{
    public float scaleAmount = 1.1f; // 拡大率（1.1 = 10%大きく）
    public float duration = 0.5f;    // アニメーションの時間（片道）

    private RectTransform textRect;

    private void Start()
    {
        textRect = GetComponent<RectTransform>();
        AnimateLoop();
    }

    private void AnimateLoop()
    {
        // 初期スケールを保存しておく
        Vector3 originalScale = textRect.localScale;

        // 無限の拡大縮小ループ
        textRect.DOScale(originalScale * scaleAmount, duration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }
}
