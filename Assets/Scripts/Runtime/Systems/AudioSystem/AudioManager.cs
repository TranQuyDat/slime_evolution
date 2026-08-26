using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class AudioManager : MonoBehaviour
{
    private const string MuteKey = "audio_muted";

    public static AudioManager Instance;
    [SerializeField]private AudioSource _sfxSourcePrefab;
    [SerializeField]private AudioSource _bgmSource;

    private Queue<AudioSource> _sfxSources;
    private bool _isMuted;
    public bool IsMuted => _isMuted;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        _sfxSources = new Queue<AudioSource>();
        SetMuted(PlayerPrefs.GetInt(MuteKey, 0) == 1, false);
    }

    public void ToggleMute() => SetMuted(!_isMuted);

    public void SetMuted(bool muted, bool save = true)
    {
        _isMuted = muted;

        if (_bgmSource != null)
            _bgmSource.mute = muted;

        foreach (AudioSource source in GetComponentsInChildren<AudioSource>(true))
            source.mute = muted;

        if (!save) return;
        PlayerPrefs.SetInt(MuteKey, muted ? 1 : 0);
        PlayerPrefs.Save();
    }
  
    public AudioSource GetSource()
    { 
        AudioSource source ;
        if (_sfxSources.Count <= 0)
        {
            source  = Instantiate(_sfxSourcePrefab,transform);
            source.mute = _isMuted;
            return source; 
        }
        source  = _sfxSources.Dequeue();
        source.gameObject.SetActive(true);
        source.mute = _isMuted;
        return source;
    }

    public void StopSfxUntilFinish(AudioSource source)
    {
        StartCoroutine(StopSfx(source));
    }
    private IEnumerator StopSfx(AudioSource source)
    {
        while (source != null && source.isPlaying)
        {
            yield return null; 
        }
        source.gameObject.SetActive(false);
        _sfxSources.Enqueue(source);
    }

}
