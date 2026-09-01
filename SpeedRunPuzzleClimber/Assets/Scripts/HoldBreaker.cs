using UnityEngine;
using System.Collections;

public class HoldBreaker : MonoBehaviour
{
    private PlayerManager _playerManager;

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
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
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
                    Animator.SetBool("IsBShake", true);
                }
                else
                {
                    //Set Ani Bool active
                    Animator.SetBool("IsShake", true);
                }


                timerRunning = true;
                gripTimer = 0f;
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

    // Called by manager on all clients
    public void TriggerDisable()
    {
        if (!gameObject.activeInHierarchy)
            return;

        StartCoroutine(DisableRoutine());
    }

    private IEnumerator DisableRoutine()
    {
        isDisabled = true;
        timerRunning = false;

        Animator.SetBool("IsBShake", false);
        Animator.SetBool("IsShake", false);

        selfCollider.enabled = false;
        SetOpacity(fadedOpacity);

        

        if (hasCollided && _playerManager != null)
        {
            _playerManager.CanGripJug = false;
            _playerManager.CanGripCrimp = false;
            _playerManager.CanGripPocket = false;
        }

        yield return new WaitForSeconds(disableDuration);

        // re-enable
        SetOpacity(originalOpacity);
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
}
