using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgmRegistry_Int : MonoBehaviour
{
    public static BgmRegistry_Int I { get; private set; }

    [Header("2D AudioSource (Loop=On)")]
    public AudioSource musicSource;          // 비워두면 자동 생성
    [Header("CSV BGM 인덱스와 동일한 순서로 배치")]
    public List<AudioClip> clips = new();    // 0,1,2,...

    [Range(0f, 1f)] public float defaultFade = 0.35f;
    public bool debugLog = true;

    int _current = -1;
    public int CurrentIndex => _current;

    void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        if (!musicSource)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.playOnAwake = false;
            musicSource.loop = true;        // BGM은 루프
            musicSource.spatialBlend = 0f;
            musicSource.volume = 1f;
        }
    }

    public void PlayByIndex(int index, float fade = -1f)
    {
        if (index < 0 || index >= clips.Count || clips[index] == null) {
            if (debugLog) Debug.LogWarning($"[BGM] invalid index {index}");
            return;
        }

        if (_current == index && musicSource.isPlaying) {
            if (debugLog) Debug.Log($"[BGM] same track {_current}, skip");
            return; // 같은 곡이면 아무 것도 안 함
        }

        if (fade < 0f) fade = defaultFade;
        StartCoroutine(Swap(clips[index], index, fade));
    }

    public void Stop(float fade = -1f)
    {
        if (fade < 0f) fade = defaultFade;
        StartCoroutine(FadeOutStop(fade));
        _current = -1;
    }

    IEnumerator Swap(AudioClip next, int nextIndex, float fade)
    {
        if (musicSource.isPlaying && fade > 0f)
            yield return FadeOut(fade);

        musicSource.clip = next;
        musicSource.time = 0f;
        musicSource.Play();
        musicSource.mute = false;              // 혹시라도 남아있을지 모르는 뮤트 방지
        musicSource.volume = 1f;               // 볼륨 강제 복구
        AudioListener.pause = false;           // 전역 일시정지 해제

        
        if (fade > 0f)
            yield return FadeIn(fade);

        _current = nextIndex;
        if (debugLog) Debug.Log($"[BGM] ▶ idx={_current}, clip={next.name}");
        StartCoroutine(_Probe());

        IEnumerator _Probe()
        {
            yield return new WaitForSeconds(0.1f);
            Debug.Log($"[BGM PROBE] isPlaying={musicSource.isPlaying}, vol={musicSource.volume}, mute={musicSource.mute}, sb={musicSource.spatialBlend}, output={(musicSource.outputAudioMixerGroup ? musicSource.outputAudioMixerGroup.name : "None")}");
        }
    }

    IEnumerator FadeOutStop(float t)
    {
        yield return FadeOut(t);
        musicSource.Stop();
    }

    IEnumerator FadeOut(float t)
    {
        float start = musicSource.volume;
        float time = 0f;
        while (time < t) {
            time += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(start, 0f, time / t);
            yield return null;
        }
        musicSource.volume = start; // 다음 재생을 위해 복원
    }

    IEnumerator FadeIn(float t)
    {
        float target = musicSource.volume;
        musicSource.volume = 0f;
        float time = 0f;
        while (time < t) {
            time += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(0f, target, time / t);
            yield return null;
        }
    }
}
