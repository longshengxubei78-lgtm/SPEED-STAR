using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 再生中のSEを後から止めるための控え。
/// PlaySEの戻り値として受け取り、StopSEに渡すと止められる。
/// 別のSEに上書きされた後に使っても、無関係な音は止まらない。
/// </summary>
public readonly struct SEHandle
{
    public readonly int SlotIndex;
    public readonly int PlayId;

    public SEHandle(int slotIndex, int playId)
    {
        SlotIndex = slotIndex;
        PlayId = playId;
    }
}

/// <summary>
/// AudioSourceを一括管理するシングルトン。
/// AudioClipを渡すだけで再生できる。
/// ・BGM : 専用のAudioSource1つ（ループ再生・切り替え対応）
/// ・SE  : AudioSourceのプールから空いているものを使って再生（同時再生・ループ対応）
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private class SESlot
    {
        public AudioSource Source;
        public int PlayId; // 再生のたびに増やして、古いHandleを無効にする
    }

    [Header("SE設定")]
    [Tooltip("同時に鳴らせるSEの最大数")]
    [SerializeField, Min(1)] private int sePoolSize = 10;

    [Header("音量設定")]
    [SerializeField, Range(0f, 1f)] private float masterVolume = 1f;
    [SerializeField, Range(0f, 1f)] private float bgmVolume = 1f;
    [SerializeField, Range(0f, 1f)] private float seVolume = 1f;

    private AudioSource bgmSource;
    private readonly List<SESlot> seSlots = new List<SESlot>();

    private float currentBgmBaseVolume = 1f; // 再生時に指定されたBGM個別の音量
    private int stealIndex;                  // 全SEが使用中のときに上書きを探し始める位置

    private void Awake()
    {
        // シングルトン（重複していたら自分を破棄）
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        CreateSources();
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

#if UNITY_EDITOR
    // インスペクターで音量を触ったときも、再生中のBGMに反映させる
    private void OnValidate()
    {
        if (bgmSource != null) ApplyBgmVolume();
    }
#endif

    // ------------------------------------------------------------
    // 初期化
    // ------------------------------------------------------------

    private void CreateSources()
    {
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.playOnAwake = false;
        bgmSource.loop = true;

        for (int i = 0; i < sePoolSize; i++)
        {
            var source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = false;
            seSlots.Add(new SESlot { Source = source });
        }
    }

    // ------------------------------------------------------------
    // SE再生API
    // ------------------------------------------------------------

    /// <summary>
    /// SEを再生する。戻り値のHandleを持っておけば、StopSEで後から止められる。
    /// （止める必要がない単発SEなら戻り値は無視してよい）
    /// </summary>
    public SEHandle PlaySE(AudioClip clip, float volume = 1f, float pitch = 1f, bool loop = false)
    {
        if (clip == null) return default;

        int index = GetSESlotIndex();
        SESlot slot = seSlots[index];
        slot.PlayId++;

        AudioSource source = slot.Source;
        source.Stop();
        source.clip = clip;
        source.volume = volume * seVolume * masterVolume;
        source.pitch = pitch;
        source.loop = loop;
        source.Play();

        return new SEHandle(index, slot.PlayId);
    }

    /// <summary>Handleが指すSEがまだ鳴っているか</summary>
    public bool IsPlayingSE(SEHandle handle)
    {
        return IsCurrent(handle) && seSlots[handle.SlotIndex].Source.isPlaying;
    }

    /// <summary>Handleが指すSEを止める（すでに終わっている・上書きされている場合は何もしない）</summary>
    public void StopSE(SEHandle handle)
    {
        if (!IsCurrent(handle)) return;

        AudioSource source = seSlots[handle.SlotIndex].Source;
        source.Stop();
        source.loop = false;
        source.clip = null;
    }

    public void StopAllSE()
    {
        foreach (var slot in seSlots)
        {
            slot.Source.Stop();
            slot.Source.loop = false;
            slot.Source.clip = null;
        }
    }

    // ------------------------------------------------------------
    // BGM再生API
    // ------------------------------------------------------------

    /// <summary>BGMを再生する（同じ曲が再生中なら何もしない）</summary>
    public void PlayBGM(AudioClip clip, float volume = 1f, bool loop = true)
    {
        if (clip == null) return;
        if (bgmSource.clip == clip && bgmSource.isPlaying) return;

        bgmSource.Stop();
        bgmSource.clip = clip;
        bgmSource.loop = loop;
        currentBgmBaseVolume = volume;
        ApplyBgmVolume();
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
        bgmSource.clip = null;
    }

    public void PauseBGM() => bgmSource.Pause();
    public void ResumeBGM() => bgmSource.UnPause();

    // ------------------------------------------------------------
    // 音量API
    // ------------------------------------------------------------

    public void SetMasterVolume(float value)
    {
        masterVolume = Mathf.Clamp01(value);
        ApplyBgmVolume();
    }

    public void SetBGMVolume(float value)
    {
        bgmVolume = Mathf.Clamp01(value);
        ApplyBgmVolume();
    }

    public void SetSEVolume(float value)
    {
        seVolume = Mathf.Clamp01(value); // 次に鳴らすSEから反映
    }

    // ------------------------------------------------------------
    // 内部処理
    // ------------------------------------------------------------

    private void ApplyBgmVolume()
    {
        bgmSource.volume = currentBgmBaseVolume * bgmVolume * masterVolume;
    }

    private bool IsCurrent(SEHandle handle)
    {
        return handle.PlayId != 0
            && handle.SlotIndex >= 0
            && handle.SlotIndex < seSlots.Count
            && seSlots[handle.SlotIndex].PlayId == handle.PlayId;
    }

    private int GetSESlotIndex()
    {
        // 1. 空いているものを使う
        for (int i = 0; i < seSlots.Count; i++)
        {
            if (!seSlots[i].Source.isPlaying) return i;
        }

        // 2. 全部使用中なら、ループしていないSEを優先して上書きする
        for (int n = 0; n < seSlots.Count; n++)
        {
            int i = (stealIndex + n) % seSlots.Count;
            if (!seSlots[i].Source.loop)
            {
                stealIndex = (i + 1) % seSlots.Count;
                return i;
            }
        }

        // 3. 全部ループ中なら、順番に上書きする
        int stolen = stealIndex;
        stealIndex = (stealIndex + 1) % seSlots.Count;
        return stolen;
    }
}
