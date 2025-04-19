using DG.Tweening;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class CarController : MonoBehaviour
{
    [SerializeField] private Vector3 startPos;
    [SerializeField] private Vector3 endPos;
    [SerializeField] private float time_to_move;

    private bool activated = false;
    private bool init = true;

    // Update is called once per frame
    private void Update()
    {
        if (activated && init)
        {
            init = false;
            transform.parent.gameObject.transform.DOLocalMove(endPos, time_to_move)
                    .SetEase(Ease.Linear)
                    .ToUniTask(cancellationToken: destroyCancellationToken).Forget();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            activated = true;
        }
    }
}
