using System;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    SE,
    BGM
}

/// <summary>1つの音の登録情報</summary>
[Serializable]
public class SoundEntry
{
    [Tooltip("呼び出し用の名前（例: Jump, Drift, Stage1）")]
    public string key;
    public AudioClip clip;
    public SoundType type = SoundType.SE;

    [Range(0f, 1f)] public float volume = 1f;
    [Tooltip("SEのみ有効")]
    [Range(0.5f, 2f)] public float pitch = 1f;
    [Tooltip("BGMはこの設定でループ。SEはtrueだとStopするまで鳴り続ける")]
    public bool loop = true;
}

/// <summary>
/// AudioClipをインスペクターに登録しておき、名前（key）かインデックスを指定して
/// AudioManagerに再生を依頼するクラス。
/// 車・敵・UIなど、音を持たせたいオブジェクトにアタッチして使う。
/// ループSEはここでHandleを保持し、Stopで止められる。
/// </summary>
public class SoundBank : MonoBehaviour
{
    [SerializeField] private List<SoundEntry> sounds = new List<SoundEntry>();

    private readonly Dictionary<string, SoundEntry> table = new Dictionary<string, SoundEntry>();

    // このSoundBankが鳴らしているループSEのHandle
    private readonly Dictionary<SoundEntry, SEHandle> loopHandles = new Dictionary<SoundEntry, SEHandle>();

    private void Awake()
    {
        BuildTable();
    }

    // 無効化・破棄されたときに、鳴らしっぱなしのループSEを残さない
    private void OnDisable()
    {
        StopAllLoops();
    }

    private void BuildTable()
    {
        table.Clear();
        foreach (var entry in sounds)
        {
            if (string.IsNullOrEmpty(entry.key))
            {
                Debug.LogWarning($"{name}: keyが空のSoundEntryがあります", this);
                continue;
            }
            if (!table.TryAdd(entry.key, entry))
            {
                Debug.LogWarning($"{name}: key「{entry.key}」が重複しています", this);
            }
        }
    }

    // ------------------------------------------------------------
    // 再生API
    // ------------------------------------------------------------

    /// <summary>登録した名前で再生する（コードから呼ぶ用）</summary>
    public void Play(string key)
    {
        if (!table.TryGetValue(key, out SoundEntry entry))
        {
            //Debug.LogWarning($"{name}: key「{key}」は登録されていません", this);
            return;
        }
        Send(entry);
    }

    /// <summary>リストの番号で再生する（UnityEventから呼ぶ用）</summary>
    public void PlayByIndex(int index)
    {
        if (index < 0 || index >= sounds.Count)
        {
            Debug.LogWarning($"{name}: index {index} は範囲外です", this);
            return;
        }
        Send(sounds[index]);
    }

    // ------------------------------------------------------------
    // 停止API
    // ------------------------------------------------------------

    /// <summary>ループSEを名前で止める（コードから呼ぶ用）</summary>
    public void Stop(string key)
    {
        if (!table.TryGetValue(key, out SoundEntry entry))
        {
            //Debug.LogWarning($"{name}: key「{key}」は登録されていません", this);
            return;
        }
        StopLoop(entry);
    }

    /// <summary>ループSEを番号で止める（UnityEventから呼ぶ用）</summary>
    public void StopByIndex(int index)
    {
        if (index < 0 || index >= sounds.Count) return;
        StopLoop(sounds[index]);
    }

    public void StopAllLoops()
    {
        if (AudioManager.Instance != null)
        {
            foreach (var handle in loopHandles.Values)
            {
                AudioManager.Instance.StopSE(handle);
            }
        }
        loopHandles.Clear();
    }

    public void StopBGM()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.StopBGM();
    }

    // ------------------------------------------------------------
    // AudioManagerへ情報を送る
    // ------------------------------------------------------------

    private void Send(SoundEntry entry)
    {
        if (entry.clip == null) return;

        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("シーンにAudioManagerがありません", this);
            return;
        }

        switch (entry.type)
        {
            case SoundType.SE:
                if (entry.loop) PlayLoopSE(entry);
                else AudioManager.Instance.PlaySE(entry.clip, entry.volume, entry.pitch);
                break;
            case SoundType.BGM:
                AudioManager.Instance.PlayBGM(entry.clip, entry.volume, entry.loop);
                break;
        }
    }

    private void PlayLoopSE(SoundEntry entry)
    {
        // すでに鳴っているなら重ねて鳴らさない
        if (loopHandles.TryGetValue(entry, out SEHandle current)
            && AudioManager.Instance.IsPlayingSE(current))
        {
            return;
        }

        loopHandles[entry] = AudioManager.Instance.PlaySE(entry.clip, entry.volume, entry.pitch, true);
    }

    private void StopLoop(SoundEntry entry)
    {
        if (!loopHandles.TryGetValue(entry, out SEHandle handle)) return;

        if (AudioManager.Instance != null) AudioManager.Instance.StopSE(handle);
        loopHandles.Remove(entry);
    }
}
