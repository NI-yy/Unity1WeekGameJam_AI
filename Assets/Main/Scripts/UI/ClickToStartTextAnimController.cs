using DG.Tweening;
using UnityEngine;
using TMPro;
using System.Threading;
using Cysharp.Threading.Tasks;

public class ClickToStartTextAnimController : MonoBehaviour
{
    [SerializeField] private float scaleAmount = 1.1f; // 拡大率（1.1 = 10%大きく）
    [SerializeField] private float duration = 0.5f;    // アニメーションの時間（片道）
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform textRect;

    private CancellationTokenSource cancellationTokenSource;

    private void Start()
    {
        AnimateLoop();
    }

    private void AnimateLoop()
    {
        cancellationTokenSource = new CancellationTokenSource();
        CancellationToken token = cancellationTokenSource.Token;

        // 初期スケールを保存しておく
        Vector3 originalScale = textRect.localScale;

        // 無限の拡大縮小ループ
        textRect.DOScale(originalScale * scaleAmount, duration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .ToUniTask(cancellationToken: token);
    }

    public void OnClicked()
    {
        cancellationTokenSource?.Cancel();

        Vector3 currentScale = textRect.localScale;
        Vector3 targetScale = currentScale * 1.5f;

        // スケールアップ & フェードアウトを同時に
        textRect.DOScale(targetScale, 0.1f).SetEase(Ease.OutCubic);
        canvasGroup.DOFade(0f, 0.1f).SetEase(Ease.Linear);
    }
}
