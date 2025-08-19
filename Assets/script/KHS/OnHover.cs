using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OnHover : MonoBehaviour
{
    [Tooltip("외곽선 역할을 하는 자식 GameObject")]
    public GameObject outlineObj;

    // === [ADD] Hover SFX 설정값 ===
    [Header("Hover SFX")]                           // [ADD]
    public AudioClip hoverClip;                     // [ADD] 틱틱 효과음 클립
    [Range(0f, 1f)] public float hoverVolume = 0.6f;// [ADD] 재생 볼륨
    [Tooltip("빠르게 들락날락할 때 중복 재생 방지")] // [ADD]
    public float hoverCooldown = 0.05f;             // [ADD] 쿨다운(초)

    // === [ADD] 내부 상태 ===
    AudioSource _audio;                             // [ADD] 재생용 AudioSource
    float _lastHoverTime = -999f;                   // [ADD] 마지막 재생 시각(쿨다운용)

    void Awake()
    {
        // (원래 있던) Outline 자동 할당 로직 유지
        if (outlineObj == null)
        {
            var t = transform.Find("Outline");
            if (t != null) outlineObj = t.gameObject;
        }

        // === [ADD] Hover SFX용 AudioSource 준비 ===
        if (hoverClip != null)                      // [ADD]
        {                                           // [ADD]
            _audio = GetComponent<AudioSource>();   // [ADD]
            if (_audio == null)                     // [ADD]
                _audio = gameObject.AddComponent<AudioSource>(); // [ADD]
            _audio.playOnAwake  = false;            // [ADD]
            _audio.loop         = false;            // [ADD]
            _audio.spatialBlend = 0f;               // [ADD] 2D 사운드
        }                                           // [ADD]
    }

    void OnDisable()
    {
        // (원래 있던) Outline 끄기
        if (outlineObj != null) outlineObj.SetActive(false);
    }

    // EventTrigger → PointerEnter 에 연결
    public void OnPointerEnter(BaseEventData data)
    {
        // (원래 있던) Outline 켜기
        if (outlineObj != null) outlineObj.SetActive(true);

        // === [ADD] 틱틱 SFX 재생(쿨다운 적용) ===
        if (hoverClip != null) // 클립이 지정된 경우에만 재생
        {
            if (Time.unscaledTime - _lastHoverTime >= hoverCooldown)
            {
                if (_audio == null) // 혹시 Awake에서 못 만들었으면 보정
                {
                    _audio = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
                    _audio.playOnAwake = false; _audio.loop = false; _audio.spatialBlend = 0f;
                }
                _audio.PlayOneShot(hoverClip, hoverVolume);
                _lastHoverTime = Time.unscaledTime;
            }
        }
    }

    // EventTrigger → PointerExit 에 연결
    public void OnPointerExit(BaseEventData data)
    {
        // (원래 있던) Outline 끄기
        if (outlineObj != null) outlineObj.SetActive(false);
    }
}
