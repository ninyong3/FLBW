using UnityEngine;
using System.Collections;

public class SceneMusicTrigger : MonoBehaviour
{
    [Header("이 씬에서 재생할 BGM")]
    [SerializeField] private AudioClip sceneBgm;
    [SerializeField] private float playFadeSeconds = 0.35f;
    [SerializeField] private float stopFadeSeconds = 0.0f;   // 이전 BGM 끌 때 (0=즉시)

    [Header("씬 떠날 때 정리 옵션")]
    [SerializeField] private bool stopOnSceneExit = true;     // 씬 나갈 때 끄기
    [SerializeField] private bool stopOnDisable   = false;    // 오브젝트 비활성 시도 끄기
    [SerializeField] private bool stopOnlyIfOwned = true;     // 내가 켠 것만 끄기

    // MusicManager가 없을 때 쓰는 임시 소스(DDOL)
    static AudioSource _fallbackSrc;
    static AudioListener _listener;

    // 이 트리거가 실제로 재생을 시작했는지(소유권 플래그)
    bool _ownsPlayback = false;

    void Start()
    {
        StartCoroutine(BootstrapAndPlay());
    }

    IEnumerator BootstrapAndPlay()
    {
        // 출력 보장
        EnsureListener();
        AudioListener.pause = false;

        // 이전 BGM 모두 정지
        TryStopAll();

        // 경합 제거
        yield return null;

        // 이 씬 BGM 재생
        if (sceneBgm != null)
        {
            if (TryPlayViaMusicManager(sceneBgm))
            {
                _ownsPlayback = true;
                Probe("[MM]");
                yield break;
            }
            PlayViaFallback(sceneBgm);
            _ownsPlayback = true;
            Probe("[FALLBACK]");
        }
        else
        {
            Debug.LogWarning("[SceneMusicTrigger] sceneBgm is null.");
        }
    }

    // ====== 씬 나갈 때 / 비활성 시 정리 ======
    void OnDisable()
    {
        if (stopOnDisable) TryStopOnExit();
    }

    void OnDestroy()
    {
        if (stopOnSceneExit) TryStopOnExit();
    }

    void TryStopOnExit()
    {
        // 내 소유만 끌지 여부
        if (stopOnlyIfOwned && !_ownsPlayback) return;

        // 가능한 모든 경로 정지 (안전)
        if (MusicManager.I != null)
        {
            var mm = MusicManager.I;
            try { mm.StopBgm(); }
            catch
            {
                if (mm.source != null)
                {
                    if (stopFadeSeconds <= 0f) mm.source.Stop();
                    else StartCoroutine(FadeOutAndStop(mm.source, stopFadeSeconds));
                }
            }
        }

        if (BgmRegistry_Int.I != null)
            BgmRegistry_Int.I.Stop(stopFadeSeconds);

        if (_fallbackSrc != null && _fallbackSrc.isPlaying)
        {
            if (stopFadeSeconds <= 0f) _fallbackSrc.Stop();
            else StartCoroutine(FadeOutAndStop(_fallbackSrc, stopFadeSeconds));
        }

        _ownsPlayback = false;
    }

    // ====== 정지(입장 시) ======
    void TryStopAll()
    {
        if (BgmRegistry_Int.I != null)
            BgmRegistry_Int.I.Stop(stopFadeSeconds);

        if (MusicManager.I != null)
        {
            var mm = MusicManager.I;
            try { mm.StopBgm(); }
            catch
            {
                if (mm.source != null)
                {
                    if (stopFadeSeconds <= 0f) mm.source.Stop();
                    else StartCoroutine(FadeOutAndStop(mm.source, stopFadeSeconds));
                }
            }
        }

        if (_fallbackSrc != null && _fallbackSrc.isPlaying)
        {
            if (stopFadeSeconds <= 0f) _fallbackSrc.Stop();
            else StartCoroutine(FadeOutAndStop(_fallbackSrc, stopFadeSeconds));
        }
    }

    // ====== 재생 (MusicManager 경유) ======
    bool TryPlayViaMusicManager(AudioClip clip)
    {
        if (MusicManager.I == null) return false;

        // PlayBgm(clip)이 있으면 사용
        try
        {
            MusicManager.I.PlayBgm(clip);
            return true;
        }
        catch { /* 시그니처 없을 수 있음 */ }

        // 없으면 AudioSource 직접 재생
        if (MusicManager.I.source != null)
        {
            var src = MusicManager.I.source;
            Setup2DLoop(src);
            src.clip = clip;
            src.time = 0f;
            src.mute = false;
            src.volume = 1f;
            src.Play();
            return true;
        }

        return false;
    }

    // ====== 재생 (폴백) ======
    void PlayViaFallback(AudioClip clip)
    {
        if (_fallbackSrc == null)
        {
            var go = new GameObject("TempBgmPlayer_DDOL");
            DontDestroyOnLoad(go);
            _fallbackSrc = go.AddComponent<AudioSource>();
            Setup2DLoop(_fallbackSrc);
        }

        _fallbackSrc.clip = clip;
        _fallbackSrc.time = 0f;
        _fallbackSrc.mute = false;
        _fallbackSrc.volume = 1f;
        _fallbackSrc.Play();

        Debug.Log("[SceneMusicTrigger] Played via fallback AudioSource (MusicManager not found).");
    }

    // ====== 유틸 ======
    void EnsureListener()
    {
        if (_listener && _listener.enabled) return;

        _listener = FindObjectOfType<AudioListener>();
        if (_listener == null)
        {
            var go = new GameObject("GlobalAudioListener_DDOL");
            _listener = go.AddComponent<AudioListener>();
            DontDestroyOnLoad(go);
            Debug.LogWarning("[SceneMusicTrigger] No AudioListener found → created GlobalAudioListener_DDOL.");
        }
        else if (!_listener.enabled)
        {
            _listener.enabled = true;
            Debug.LogWarning("[SceneMusicTrigger] Found AudioListener but disabled → enabled.");
        }
    }

    void Setup2DLoop(AudioSource src)
    {
        src.loop = true;
        src.playOnAwake = false;
        src.spatialBlend = 0f;           // 2D
        src.ignoreListenerPause = true;
    }

    IEnumerator FadeOutAndStop(AudioSource src, float t)
    {
        if (src == null) yield break;
        float start = src.volume;
        float time = 0f;
        while (time < t)
        {
            time += Time.unscaledDeltaTime;
            src.volume = Mathf.Lerp(start, 0f, time / t);
            yield return null;
        }
        src.Stop();
        src.volume = start; // 다음 재생 대비 복구
    }

    void Probe(string tag)
    {
        AudioSource src = null;
        if (MusicManager.I != null && MusicManager.I.source != null) src = MusicManager.I.source;
        else if (_fallbackSrc != null) src = _fallbackSrc;

        if (src != null)
        {
            Debug.Log($"[SceneMusicTrigger PROBE {tag}] " +
                      $"isPlaying={src.isPlaying}, vol={src.volume}, mute={src.mute}, " +
                      $"clip={(src.clip ? src.clip.name : "None")}, " +
                      $"output={(src.outputAudioMixerGroup ? src.outputAudioMixerGroup.name : "None")}");
        }
    }
}
