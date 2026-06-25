using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class AudioManager : MonoBehaviour
{

    public static AudioManager Instance;
    [SerializeField]private AudioSource _sfxSourcePrefab;
    [SerializeField]private AudioSource _bgmSource;

    private Queue<AudioSource> _sfxSources;
    private bool _isMuted;
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        _sfxSources = new Queue<AudioSource>();
    }
  
    public AudioSource getSource()
    { 
        AudioSource source ;
        if (_sfxSources.Count <= 0)
        {
            source  = Instantiate(_sfxSourcePrefab,transform);
            return source; 
        }
        source  = _sfxSources.Dequeue();
        source.gameObject.SetActive(true);
        return source;
    }

    public void StopSfxUntillFinish(AudioSource source)
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