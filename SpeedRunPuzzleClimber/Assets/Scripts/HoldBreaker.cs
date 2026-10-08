using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CrackedColourChange))]
public class HoldBreaker : MonoBehaviour
{
    private CrackedColourChange _crackedColourChange;
    private PlayerManager _playerManager;
    [SerializeField] private GameObject _dustParticle;
    [SerializeField] private GameObject _crackSound;

    private int holdIndex = -1;

    [Header("Settings")]
    [SerializeField] private float gripTime = 2f;
    [SerializeField] private float disableDuration = 5f;
    [SerializeField] private float fadedOpacity = 0.4f;

    private SpriteRenderer spriteRenderer;
    private Collider selfCollider;
    private float originalOpacity;
    private Animator Animator;

    private bool isDisabled = false;
    private bool hasCollided = false;

    private bool timerRunning = false;
    private float gripTimer = 0f;

    private void Awake()
    {
        _crackedColourChange = GetComponent<CrackedColourChange>();
        spriteRenderer = GetComponentInParent<SpriteRenderer>();
        selfCollider = GetComponent<Collider>();
        originalOpacity = spriteRenderer.color.a;
        Animator = GetComponentInChildren<Animator>();

    }



    private void Update()
    {


        if (!timerRunning)
        {
            if (hasCollided && _playerManager.isGripping)
            {
                if (gripTime < 1f)
                {
                    //Set Ani Bool active
                    //Animator.SetBool("IsBShake", true);
                }
                else
                {
                    //Set Ani Bool active
                    //Animator.SetBool("IsShake", true);
                }


                timerRunning = true;
                gripTimer = 0f;

                if (_dustParticle != null) Instantiate(_dustParticle, this.transform.position, Quaternion.identity);
                if (_crackSound != null) Instantiate(_crackSound);
                _crackedColourChange.DimLights(gripTime, disableDuration);
            }
        }

        if (timerRunning)
        {
            gripTimer += Time.deltaTime;

            if (gripTimer >= gripTime)
            {
                StartCoroutine(DisableRoutine());
                timerRunning = false;
            }
        }
    }


    private IEnumerator DisableRoutine()
    {
        isDisabled = true;
        timerRunning = false;

        //Animator.SetBool("IsBShake", false);
        //Animator.SetBool("IsShake", false);

        selfCollider.enabled = false;

        yield return StartCoroutine(FadeOpacity(originalOpacity, fadedOpacity, 0.2f));

        if (hasCollided && _playerManager != null)
        {
            _playerManager.CanGripJug = false;
            _playerManager.CanGripCrimp = false;
            _playerManager.CanGripPocket = false;
        }

        yield return new WaitForSeconds(disableDuration);

        // re-enable
        yield return StartCoroutine(FadeOpacity(fadedOpacity, originalOpacity, 0.2f));

        selfCollider.enabled = true;

        isDisabled = false;
        hasCollided = false;
    }

    // Player reference
    public void GetPlayerReference(PlayerManager player)
    {
        _playerManager = player;
    }

    public void OnCollision() { if (!isDisabled) hasCollided = true; }

    public void EndLGrip() { hasCollided = false; }


    private void SetOpacity(float v)
    {
        Color c = spriteRenderer.color;
        c.a = v;
        spriteRenderer.color = c;
    }

    private IEnumerator FadeOpacity(float startOpacity, float targetOpacity, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / duration;
            SetOpacity(Mathf.Lerp(startOpacity, targetOpacity, t));

            yield return null;
        }

        SetOpacity(targetOpacity);
    }
}