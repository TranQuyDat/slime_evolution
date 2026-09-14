using UnityEngine;
[CreateAssetMenu(fileName ="Simple_Audio_Event" ,menuName ="Events/Simple Audio Event")]
class SimpleAudioEvent : BaseAudioEvent
{
    [Header("Audio Clips random")]
    [SerializeField]private AudioClip[] _clips;
    [Header("Setting")]
    [Range(0f, 3f)][SerializeField]private float _volume = 1f;
    [Range(0f, 2f)][SerializeField]private float _pitchMin = 0.85f;
    [Range(0f, 2f)][SerializeField]private float _pitchMax = 1f;
    [Range(0f, 1f)][SerializeField]private float _spatialBlend = 1f;
    [SerializeField]private bool _loop = false;

    public override void Play(AudioContext ctx = default)
    {
        if(_clips == null ||_clips.Length<=0) return;
        var manager = AudioManager.Instance;
        AudioSource source = manager.GetSource();

        int id = Random.Range(0,_clips.Length);
        AudioClip clip = _clips[id];
        source.clip = clip;
        source.pitch = Random.Range(_pitchMin,_pitchMax);
        source.spatialBlend = _spatialBlend;
        source.loop = _loop; 

        if (_loop)
        {
            source.volume = Mathf.Min(_volume, 1f);
            source.Play();
            return;
        }

        // PlayOneShot supports a volume multiplier, allowing values above the
        // AudioSource.volume limit of 1 for short SFX.
        source.volume = 1f;
        source.PlayOneShot(clip, _volume);
        manager.StopSfxUntilFinish(source);
    }

}
