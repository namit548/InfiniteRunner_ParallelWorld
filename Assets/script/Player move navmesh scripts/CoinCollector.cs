using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public sealed class CoinCollector : MonoBehaviour
{
    private const int BagCapacity = 3;
    private const int CoinsPerHousePiece = 3;
    private const string SafeZoneTag = "SafeZone";
    private static readonly int CoinAnimationState = Animator.StringToHash("Base Layer.Coin");
    [SerializeField] private Animator CoinAnimator;

    [SerializeField] private Transform coins;
    [SerializeField] private TMP_Text collectedText;
    [SerializeField] private TMP_Text bagText;
    [SerializeField] private GameObject[] housePieces = new GameObject[0];

    private int carriedCount;
    private int depositedCount;
    private int queuedAnimations;
    private Coroutine playingAnimations;

    private void Awake()
    {
        if (coins == null || collectedText == null || bagText == null)
        {
            Debug.LogError("Assign the coins container, collected text and bag text on CoinCollector.", this);
            enabled = false;
            return;
        }

        collectedText.text = "0";
        bagText.text = "0";
        UpdateHouse();

        if (CoinAnimator != null)
        {
            var image = CoinAnimator.GetComponent<Image>();
            if (image != null)
                image.raycastTarget = false;

            CoinAnimator.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActiveAndEnabled)
            return;

        if (other.CompareTag(SafeZoneTag))
        {
            DepositBag();
            return;
        }

        if (carriedCount >= BagCapacity || other.transform.parent != coins || !other.gameObject.activeSelf)
            return;

        // Stop additional trigger callbacks from counting this coin again.
        other.gameObject.SetActive(false);
        Destroy(other.gameObject);
        carriedCount++;
        bagText.text = carriedCount.ToString();
        QueuePickupAnimation();
    }

    private void QueuePickupAnimation()
    {
        if (!CanPlayPickupAnimation())
            return;

        queuedAnimations++;
        if (playingAnimations == null)
            playingAnimations = StartCoroutine(PlayPickupAnimations());
    }

    private bool CanPlayPickupAnimation()
    {
        if (CoinAnimator == null || !CoinAnimator.enabled)
            return false;

        var parent = CoinAnimator.transform.parent;
        return parent == null || parent.gameObject.activeInHierarchy;
    }

    private IEnumerator PlayPickupAnimations()
    {
        while (queuedAnimations > 0 && CanPlayPickupAnimation())
        {
            queuedAnimations--;
            // Activating the object starts its default Coin state from the beginning.
            CoinAnimator.gameObject.SetActive(true);
            yield return null;

            while (CoinAnimator != null && CoinAnimator.isActiveAndEnabled)
            {
                var state = CoinAnimator.GetCurrentAnimatorStateInfo(0);
                if (state.fullPathHash != CoinAnimationState || state.normalizedTime >= 1f)
                    break;

                yield return null;
            }

            if (CoinAnimator != null)
                CoinAnimator.gameObject.SetActive(false);

            if (queuedAnimations > 0)
                yield return null;
        }

        queuedAnimations = 0;
        playingAnimations = null;
    }

    private void OnDisable()
    {
        if (playingAnimations != null)
            StopCoroutine(playingAnimations);

        playingAnimations = null;
        queuedAnimations = 0;
        if (CoinAnimator != null)
            CoinAnimator.gameObject.SetActive(false);
    }

    private void DepositBag()
    {
        if (carriedCount == 0)
            return;

        depositedCount += carriedCount;
        carriedCount = 0;
        bagText.text = "0";
        collectedText.text = depositedCount.ToString();
        UpdateHouse();
    }

    private void UpdateHouse()
    {
        int unlockedPieces = depositedCount / CoinsPerHousePiece;
        for (int i = 0; i < housePieces.Length; i++)
        {
            if (housePieces[i] != null)
                housePieces[i].SetActive(i < unlockedPieces);
        }
    }
}
