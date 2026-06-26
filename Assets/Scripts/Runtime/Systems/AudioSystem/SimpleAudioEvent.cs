using UnityEngine;
[CreateAssetMenu(fileName ="Simple_Audio_Event" ,menuName ="Events/Simple Audio Event")]
class SimpleAudioEvent : BaseAudioEvent
{
    [Header("Audio Clips random")]
    [SerializeField]private AudioClip[] _clips;
    [Header("Setting")]
    [Range(0f, 1f)][SerializeField]private float _volume = 1f;
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
        source.clip = _clips[id];
        source.volume = _volume;
        source.pitch = Random.Range(_pitchMin,_pitchMax);
        source.spatialBlend = _spatialBlend;
        source.loop = _loop; 
        source.Play();

        if(_loop) return;
        manager.StopSfxUntilFinish(source);
    }

}