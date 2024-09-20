using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarningPopUp : MonoBehaviour
{
    [SerializeField] private Image m_warningImage;
    [SerializeField] private GameObject m_shockSign;

    private bool needToTrack;
    private Transform p_objectToTrack;

    private void Update()
    {
        if (needToTrack && p_objectToTrack != null)
        {
            transform.position = p_objectToTrack.position;
        }
    }

    public void ShowAlarm()
    {
        m_shockSign.SetActive(true);
    }

    public void SetFillValue(float value)
    {
        m_warningImage.fillAmount = value;
    }

    public void SetTracking(Transform objectToTrack)
    {
        needToTrack = true;
        p_objectToTrack = objectToTrack;
    }
}
