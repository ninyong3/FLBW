using UnityEngine;
// Debug 모호성 방지
using Debug = UnityEngine.Debug;

public class SfxRegistry_Int : MonoBehaviour
{
    public static SfxRegistry_Int I { get; private set; }

    [Header("SFX 재생에 사용할 AudioSource (없으면 자동 생성)")]
    public AudioSource source;

    [Header("CSV SFX 인덱스 순서와 동일")]
    public AudioClip[] clips;

    [Range(0f, 1f)] public float defaultVolume = 1f;
    public bool debugLog = true;

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        if (source == null)
        {
            source = gameObject.AddComponent<AudioSource>();
        }

        // 2D 단발 기본 세팅
        source.playOnAwake = false;
        source.loop = false;
        source.spatialBlend = 0f;          // 2D
        source.ignoreListenerPause = true;
        source.volume = defaultVolume;

        if (debugLog) Debug.Log("[SFX] Awake() ready");
    }

    /// <summary>
    /// 줄 시작에서 호출: 이전 줄 SFX를 끊고 새 인덱스를 재생
    /// (PlayOneShot이 아닌 clip+Play를 사용해 Stop()이 즉시 먹도록)
    /// </summary>
    public void PlayByIndex(int index, float? volume = null)
    {
        if (index < 0 || clips == null || index >= clips.Length)
        {
            if (debugLog) Debug.LogWarning($"[SFX] invalid index={index}");
            return;
        }

        var clip = clips[index];
        if (clip == null)
        {
            if (debugLog) Debug.LogWarning($"[SFX] null clip at index={index}");
            return;
        }

        // 이전 줄에서 재생 중이던 효과음 강제 정지
        if (source.isPlaying) source.Stop();

        // 한 줄용(비루프) 재생 → Stop()이 바로 먹힘
        source.loop = false;
        source.clip = clip;
        source.time = 0f;
        source.volume = Mathf.Clamp01(volume ?? defaultVolume);
        source.mute = false;
        source.Play();

        if (debugLog) Debug.Log($"[SFX] ▶ idx={index}, clip={clip.name}");
    }

    /// <summary>
    /// 현재 AudioSource에서 재생 중인 SFX를 즉시 정지
    /// </summary>
    public void StopAll()
    {
        if (debugLog) Debug.Log("[SFX] StopAll()");
        if (source != null) source.Stop();
    }
}
