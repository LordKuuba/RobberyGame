using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PickingTask : Task
{
    [Space]
    [SerializeField] private Image m_pickingProgress;
    [SerializeField] private RectTransform m_boostZone;
    [SerializeField] private RectTransform m_pickingCursor;

    [SerializeField] private float m_speedPenaltyDuration = 1f;

    [SerializeField] private float m_speedBoostDuration = 2.0f; // Duration of the speed boost
    [SerializeField] private float m_speedBoostMultiplier = 2.0f; // Multiplier for the fill speed boost
    [SerializeField] private float m_pickingCursorSpeed;

    [SerializeField] private Vector2 m_degreeDifference;

    [SerializeField] private float m_startTimeToPick;

    [SerializeField] private KeyCode m_keyToInteract;


    private bool m_taskInProgress = true;
    private bool m_isSpeedBoostActive = false; // Track if the speed boost is active
    private float m_remainingTime = 0;
    private float m_fillingProgress = 0;
    private float m_originalFillingSpeed; // Store the original filling speed
    private bool m_penaltyActivated = false;

    private void Start()
    {
        m_remainingTime = m_startTimeToPick;
        m_originalFillingSpeed = 1.0f; // Save the original filling speed, assumed as 1 unit per second
        ReplaceBoostZone();
    }

    public override void CallFunction()
    {
        base.CallFunction();
        this.gameObject.SetActive(false);
    }

    private void Update()
    {
        ProgressPicking();
    }

    void ProgressPicking()
    {
        if (m_taskInProgress)
        {
            m_pickingCursor.Rotate(0, 0, m_pickingCursorSpeed * Time.deltaTime);

            m_pickingProgress.fillAmount = (m_fillingProgress / m_remainingTime);
            m_fillingProgress += m_originalFillingSpeed * Time.deltaTime;


            if (Input.GetKeyDown(m_keyToInteract))
            {
                if (m_pickingCursor.eulerAngles.z < m_boostZone.eulerAngles.z + m_degreeDifference.x && m_pickingCursor.eulerAngles.z > m_boostZone.eulerAngles.z + m_degreeDifference.y)
                {
                    print("Yes");
                    StartCoroutine(ActivateSpeedBoost()); // Start the speed boost coroutine
                    ReplaceBoostZone();
                }
                else
                {
                    if(!m_penaltyActivated)
                        StartCoroutine(ActivatePenalty());
                }
            }

            if (m_fillingProgress > m_remainingTime)
            {
                CallFunction();
                m_taskInProgress = false;
            }
        }
    }

    IEnumerator ActivateSpeedBoost()
    {
        m_isSpeedBoostActive = true;
        float boostedFillingSpeed = 1 * m_speedBoostMultiplier;

        float originalFillingSpeed = 1;
        m_originalFillingSpeed = boostedFillingSpeed; // Apply the fill speed boost

        yield return new WaitForSeconds(m_speedBoostDuration); // Wait for the boost duration

        m_originalFillingSpeed = originalFillingSpeed; // Revert to the original fill speed
        m_isSpeedBoostActive = false;
    }

    IEnumerator ActivatePenalty()
    {
        m_penaltyActivated = true;
        float lastCursorSpeed = m_pickingCursorSpeed;
        m_pickingCursorSpeed = 0;
        yield return new WaitForSeconds(m_speedPenaltyDuration);
        m_pickingCursorSpeed = lastCursorSpeed;
        m_penaltyActivated = false;
    }

    void ReplaceBoostZone()
    {
        int boostZoneRotation = Random.Range(0, 360);

        m_boostZone.eulerAngles = new Vector3(0, 0, boostZoneRotation);
    }
}
