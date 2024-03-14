using UnityEngine;
using UnityEngine.Events;

public class AudioEndDetector : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip audioClip;
    public UnityEvent onEnd;

    void Update()
    {
        if (audioSource != null && !audioSource.isPlaying && audioSource.time >= audioClip.length)
        {
            // The audio has reached its end
            onEnd.Invoke();
            Debug.Log("done");
        }
    }
}
