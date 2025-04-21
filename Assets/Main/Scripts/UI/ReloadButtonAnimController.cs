using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ReloadButtonAnimController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 originalScale;
    public float hoverScaleFactor = 1.1f;
    public float animationDuration = 0.2f;
    private Tween currentTween;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AnimateScale(originalScale * hoverScaleFactor);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        AnimateScale(originalScale);
    }

    private void AnimateScale(Vector3 targetScale)
    {
        // 既存のTweenをキャンセルして、新しいアニメーションを開始
        currentTween?.Kill();
        currentTween = transform.DOScale(targetScale, animationDuration).SetEase(Ease.OutBack);
    }
}
