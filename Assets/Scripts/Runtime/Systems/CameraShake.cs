using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField]private Transform _cameraTransform;
    private Vector3 _originPos;
    private Coroutine _shakeCoroutine;
    public bool IsShaking { get;private set;}

    void Awake()
    {
        if(_cameraTransform == null) _cameraTransform = Camera.main.transform;
        _originPos = _cameraTransform.localPosition;

    }

    public void Shake(float duration, float strength)
    {
        if(_shakeCoroutine != null)
            StopCoroutine(_shakeCoroutine);
        
        _shakeCoroutine = StartCoroutine(ShakeRoutine(duration, strength));
    }

    IEnumerator ShakeRoutine(float duration , float strength)
    {
        float elapsed = 0;
        IsShaking = true;
        while(elapsed < duration)
        {
            _cameraTransform.localPosition = _originPos + (Vector3)Random.insideUnitCircle * strength;
            elapsed += Time.deltaTime;
            yield return null;
        }
        IsShaking = false;
        _cameraTransform.localPosition = _originPos;
    }
}
