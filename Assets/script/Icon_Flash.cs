using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Icon_Flash : MonoBehaviour
{
    [Tooltip("반짝일 때 적용할 색 (null이면 흰색)")]
    public Color flashColor = Color.white;

    [Tooltip("반짝이는 동안 아이콘을 약간 확대할지 여부")]
    public bool punchScale = true;

    [Tooltip("확대 비율 (1=기본)")]
    public float scaleMultiplier = 1.08f;

    [Tooltip("확대/축소 한 사이클 시간")]
    public float scaleDuration = 0.08f;

    private List<SpriteRenderer> _renderers = new();
    private List<Color> _originalColors = new();
    private Vector3 _originalScale;
    private Coroutine _co;
    private bool _busy;

    void Awake()
    {
        GetComponentsInChildren(true, _renderers);
        if (_renderers.Count == 0)
        {
            var r = GetComponent<SpriteRenderer>();
            if (r) _renderers.Add(r);
        }

        foreach (var r in _renderers)
            _originalColors.Add(r.color);

        _originalScale = transform.localScale;
    }

    /// <summary>
    /// 아이콘을 깜빡이게 한다.
    /// </summary>
    public void Flash(int times = 3, float on = 0.08f, float off = 0.08f, Color? overrideColor = null)
    {
        if (_busy && _co != null) { StopCoroutine(_co); Restore(); }
        _co = StartCoroutine(CoFlash(times, on, off, overrideColor));
    }

    private IEnumerator CoFlash(int times, float on, float off, Color? overrideColor)
    {
        _busy = true;
        var fc = overrideColor.HasValue ? overrideColor.Value : flashColor;

        for (int i = 0; i < times; i++)
        {
            // ON
            for (int k = 0; k < _renderers.Count; k++)
                _renderers[k].color = fc;

            if (punchScale)
                yield return ScaleTo(_originalScale * scaleMultiplier, scaleDuration);
            else
                yield return new WaitForSeconds(on);

            // OFF (원래 색으로)
            for (int k = 0; k < _renderers.Count; k++)
                _renderers[k].color = _originalColors[k];

            if (punchScale)
                yield return ScaleTo(_originalScale, scaleDuration);
            else
                yield return new WaitForSeconds(off);
        }

        Restore();
        _busy = false;
    }

    private IEnumerator ScaleTo(Vector3 target, float dur)
    {
        Vector3 start = transform.localScale;
        float t = 0f;
        while (t < dur)
        {
            t += Time.deltaTime;
            float a = Mathf.Clamp01(t / dur);
            transform.localScale = Vector3.Lerp(start, target, a);
            yield return null;
        }
    }

    private void Restore()
    {
        for (int k = 0; k < _renderers.Count; k++)
            _renderers[k].color = _originalColors[k];
        transform.localScale = _originalScale;
    }

    void OnDisable() => Restore();
}
