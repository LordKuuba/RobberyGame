using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Seeker : NetworkBehaviour
{
    [Header("Seeker Settings")]
    [SerializeField] private FieldOfView m_enemyFieldOfView;
    [SerializeField] private VisionCone m_visualisedRay;
    [Space]
    [SerializeField] private float m_enemyViewFOW;
    [SerializeField] private float m_enemyViewDistance;
    [SerializeField] private float m_enemyblindZoneRadius;

    [SerializeField] private Transform m_warningSpawnPosition;
    [SerializeField] private GameObject m_warningPopup;
    [SerializeField] private float m_timeToAlarm;

    public WarningPopUp m_spawnedPopup { get; private set; }

    StealtManager m_stealthManager;

    bool m_isPlayerSpotted;

    float m_timeSeeingPlayer;

    protected bool m_warningNeedFollow = false;

    protected virtual void Start()
    {
        m_stealthManager = StealtManager.instance;

        if(m_visualisedRay != null)
            m_visualisedRay.SetUpVisionConeParams(m_enemyViewDistance, m_enemyViewFOW);

        m_enemyFieldOfView.SetupParameters(m_enemyViewDistance, m_enemyViewFOW, m_enemyblindZoneRadius);
    }

    protected void CheckForPlayer()
    {
        if (!m_isPlayerSpotted)
        {
            if (m_enemyFieldOfView.IsPlayerInRange)
            {
                if(m_timeSeeingPlayer > m_timeToAlarm)
                {
                    //Player has been spoted and stealth failed
                    CmdFailStealth();
                    m_spawnedPopup.ShowAlarm();
                    m_isPlayerSpotted = true;
                }
                else
                {
                    m_timeSeeingPlayer += 1 * Time.deltaTime;
                }
            }
            else
            {
                if(m_timeSeeingPlayer >= 0)
                {
                    m_timeSeeingPlayer -= 1 * Time.deltaTime;
                }
            }

            if(m_timeSeeingPlayer > 0)
            {
                if(m_spawnedPopup == null)
                {
                    SpawnWarning();
                }

                m_spawnedPopup.SetFillValue(m_timeSeeingPlayer / m_timeToAlarm);
            }
            else
            {
                if(m_spawnedPopup != null)
                {
                    Destroy(m_spawnedPopup.gameObject);
                    m_spawnedPopup = null;
                }
            }
        }
    }

    void SpawnWarning()
    {
        GameObject o_spawnedPopup = Instantiate(m_warningPopup);
        m_spawnedPopup = o_spawnedPopup.GetComponent<WarningPopUp>();
        m_spawnedPopup.gameObject.transform.position = m_warningSpawnPosition.transform.position;

        if (m_warningNeedFollow)
        {
            m_spawnedPopup.SetTracking(m_warningSpawnPosition);
        }
    }

    [Command(requiresAuthority = false)]
    void CmdFailStealth()
    {
        RpcFailStealth();
    }
    [ClientRpc]
    void RpcFailStealth()
    {
        m_stealthManager.FailStealth();
    }
}
