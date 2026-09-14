using DG.Tweening;
using TMPro;
using UnityEngine;

public class FloatingScore: MonoBehaviour ,IPoolable
{
    [SerializeField]private TextMeshProUGUI _txtFloatingScore;
    [Header("Gradient Color")]
    [SerializeField] private TMP_ColorGradient _normalGradient;
    [SerializeField] private TMP_ColorGradient _greatGradient;
    [SerializeField] private TMP_ColorGradient _legendaryGradient;

    public string PoolKey => "Floating Score";

    public void run(int score , Vector2 pos)
    {
        _txtFloatingScore.text ="+"+score;
        transform.position = pos;
        
        _txtFloatingScore.alpha = 1f;
        SwapStyle(score);
        gameObject.SetActive(true);
        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DOMoveY(pos.y + 1f, 0.8f).SetEase(Ease.OutQuad));

        seq.Join(transform.DOScale(1.2f, 0.15f)
            .SetLoops(2, LoopType.Yoyo));

        seq.Join(_txtFloatingScore.DOFade(0, 0.8f));

        seq.OnComplete(() =>
        {
            ObjectPoolSystem.Instance.Cancel<FloatingScore>(this,PoolKey);
        });

    }

    private void SwapStyle(int score)
    {
        if (score < 50)
        {
            _txtFloatingScore.colorGradientPreset = _normalGradient;
        }
        else if (score < 200)
        {
            _txtFloatingScore.colorGradientPreset = _greatGradient;
        }
        else
        {
            _txtFloatingScore.colorGradientPreset = _legendaryGradient;
        }
    }
}
