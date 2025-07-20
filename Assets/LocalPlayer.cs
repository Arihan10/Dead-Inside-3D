using UnityEngine;
using UnityEngine.UI;

public class LocalPlayer : MonoBehaviour
{
    [SerializeField] private float hp;
    
    [SerializeField] private Image hpMain;
    [SerializeField] private Image hpLerp;

    void Update()
    {
        hpMain.fillAmount = Mathf.Lerp(hpMain.fillAmount, hp / 100.0f, 0.3f);
        hpLerp.fillAmount = Mathf.Lerp(hpMain.fillAmount, hp / 100.0f, Time.deltaTime * 1.2f);
    }
}
