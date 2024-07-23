using UnityEngine;
using TMPro;
public class SliderValueUpdater : MonoBehaviour
{
    public void SetDropSliderValue(float val)
    {
        gameObject.GetComponent<TextMeshProUGUI>().text = val.ToString();
    }
}
