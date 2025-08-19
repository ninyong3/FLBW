using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public static MusicManager I { get; private set; }
    public AudioSource source;

    void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        if (source == null)
        {
            source = gameObject.AddComponent<AudioSource>();
            source.loop = true;
            source.playOnAwake = false;
            source.spatialBlend = 0f;
            source.volume = 1f;
        }
    }

    public void PlayBgm(AudioClip clip, float fadeSec = 0.4f)
    {
        if (clip == null) return;
        if (source.clip == clip && source.isPlaying) return;

        StopAllCoroutines();
        if (fadeSec > 0f) StartCoroutine(FadeTo(clip, fadeSec));
        else { source.clip = clip; source.Play(); }
    }
        public void StopBgm()
    {
        if (source.isPlaying)
        {
            source.Stop();
        }
    }

    IEnumerator FadeTo(AudioClip next, float sec)
    {
        float t = 0f, start = source.volume;
        while (t < sec) { t += Time.unscaledDeltaTime; source.volume = Mathf.Lerp(start, 0f, t/sec); yield return null; }
        source.Stop(); source.clip = next; source.Play();
        t = 0f;
        while (t < sec) { t += Time.unscaledDeltaTime; source.volume = Mathf.Lerp(0f, start, t/sec); yield return null; }
    }
}
