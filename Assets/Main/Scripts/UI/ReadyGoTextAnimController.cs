using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System;
using DG.Tweening.Core.Easing;

public class ReadyGoTextAnimController : MonoBehaviour
{
    [SerializeField] private RectTransform text1Rect;
    [SerializeField] private CanvasGroup text1CanvasGroup;
    [SerializeField] private RectTransform text2Rect;
    [SerializeField] private CanvasGroup text2CanvasGroup;
    [SerializeField] private Vector3 startPos;
    [SerializeField] private Vector3 endPos;
    [SerializeField] private Vector3 startScale;
    [SerializeField] private Vector3 endScale;

    private SEManager seManager;

    public async UniTask StartAnimation()
    {
        await AnimateTexts();
    }

    private async UniTask AnimateTexts()
    {
        seManager = SEManager.Instance;

        // Text1 移動
        seManager.PlaySE_Ready();
        text1Rect.anchoredPosition = startPos;
        await text1Rect.DOAnchorPos(endPos, 0.5f).SetEase(Ease.OutCubic).AsyncWaitForCompletion();

        // Text1 フェードアウト
        await text1CanvasGroup.DOFade(0f, 0.1f).SetEase(Ease.Linear).AsyncWaitForCompletion();


        await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: destroyCancellationToken);

        text2CanvasGroup.gameObject.SetActive(true);

        // Text2 スケール変更
        seManager.PlayeSE_Go();
        text2Rect.localScale = startScale;
        await text2Rect.DOScale(endScale, 0.5f).SetEase(Ease.OutBack).AsyncWaitForCompletion();

        // Text2 フェードアウト
        await text2CanvasGroup.DOFade(0f, 0.1f).SetEase(Ease.Linear).AsyncWaitForCompletion();
    }
}
