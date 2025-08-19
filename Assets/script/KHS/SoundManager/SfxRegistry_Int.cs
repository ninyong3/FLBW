    using System.Collections.Generic;
    using UnityEngine;
    using System.Collections;
    public class SfxRegistry_Int : MonoBehaviour
    {
        public static SfxRegistry_Int I { get; private set; }

        [Header("2D AudioSource (SpatialBlend=0)")]
        public AudioSource sfxSource;

        [Header("CSV Sound Effect 인덱스와 동일한 순서로 배치")]
        public List<AudioClip> clips = new(); // 0,1,2,...

        void Awake()
        {
            if (I != null) { Destroy(gameObject); return; }
            I = this;
            DontDestroyOnLoad(gameObject);

            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false;
                sfxSource.loop = false;
                sfxSource.spatialBlend = 0f;
                sfxSource.volume = 1f;
            }
        }

        public bool debugLog = true;
        public void StopAll() {
        if (sfxSource != null) sfxSource.Stop();   // PlayOneShot 포함, 해당 소스에서 나가는 소리 전부 정지
    }
    public void PlayByIndex(int index, float volume = 1f, float pitch = 1f)
    {
        if (index < 0) return;
        if (index >= clips.Count || clips[index] == null) {
            if (debugLog) Debug.LogWarning($"[SFX] invalid index {index}");
            return;
        }

        var clip = clips[index];
        if (debugLog) Debug.Log($"[SFX] Play idx={index}, clip={clip.name}, vol={volume}, pitch={pitch}, t={Time.time:F2}");

        sfxSource.pitch = pitch;
        sfxSource.PlayOneShot(clip, Mathf.Clamp01(volume));

        // (선택) 재생 상태 확인 코루틴
        StartCoroutine(CheckIsPlaying());
    }

    IEnumerator CheckIsPlaying() {
        // PlayOneShot은 한 프레임 뒤 isPlaying이 true가 됨
        yield return null;
        Debug.Log($"[SFX] sfxSource.isPlaying = {sfxSource.isPlaying}");
    }

    }
