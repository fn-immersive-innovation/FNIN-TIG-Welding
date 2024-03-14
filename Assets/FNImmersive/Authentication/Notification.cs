using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

namespace FNImmersiveInnovation
{
    public class Notification : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_ErrorText;

        [SerializeField] private Image m_NotificationBG;


        private void ShowState(bool show = true)
        {
            m_ErrorText.gameObject.SetActive(show);
            m_NotificationBG.enabled = show;
        }

        // Start is called before the first frame update
        private void OnEnable()
        {
            ShowState(false);
        }

        IEnumerator DelayTimer(float duration, UnityAction callback)
        {
            yield return new WaitForSeconds(duration);
            callback.Invoke();
        }

        public void ShowNotification(string text, bool isSuccess = true)
        {
            ShowState(true);

            m_ErrorText.text = text;

            print(text);

            StartCoroutine(DelayTimer(5.0f, () =>
            {
                ShowState(false);
            }));
        }
    }
}


