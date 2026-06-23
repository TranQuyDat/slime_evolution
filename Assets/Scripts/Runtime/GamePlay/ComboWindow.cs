using TMPro;
using UnityEngine;

class ComboWindow : MonoBehaviour
{
    [SerializeField]private TextMeshProUGUI _txtCombo;

    public void show () => gameObject.SetActive(true);
    public void Hide () => gameObject.SetActive(false);

    public void SetCombo(int cb)
    {
        _txtCombo.text = "X" + cb;
    }
}