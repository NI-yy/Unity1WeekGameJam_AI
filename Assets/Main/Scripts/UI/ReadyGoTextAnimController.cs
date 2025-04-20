using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System;

public class ReadyGoTextAnimController : MonoBehaviour
{
    public RectTransform text1Rect;
    public CanvasGroup text1CanvasGroup;
    public RectTransform text2Rect;
    public CanvasGroup text2CanvasGroup;

    public Vector3 startPos;
    public Vector3 endPos;

    public Vector3 startScale;
    public Vector3 endScale;

    public async UniTask StartAnimation()
    {
        await AnimateTexts();
    }

    private async UniTask AnimateTexts()
    {
        // Text1 移動
        text1Rect.anchoredPosition = startPos;
        await text1Rect.DOAnchorPos(endPos, 0.5f).SetEase(Ease.OutCubic).AsyncWaitForCompletion();

        // Text1 フェードアウト
        await text1CanvasGroup.DOFade(0f, 0.1f).SetEase(Ease.Linear).AsyncWaitForCompletion();


        await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: destroyCancellationToken);

        text2CanvasGroup.gameObject.SetActive(true);

        // Text2 スケール変更
        text2Rect.localScale = startScale;
        await text2Rect.DOScale(endScale, 0.5f).SetEase(Ease.OutBack).AsyncWaitForCompletion();

        // Text2 フェードアウト
        await text2CanvasGroup.DOFade(0f, 0.1f).SetEase(Ease.Linear).AsyncWaitForCompletion();
    }
}
