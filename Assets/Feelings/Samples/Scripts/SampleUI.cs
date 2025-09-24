using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SampleUI : MonoBehaviour
{
    public BasicFeelingsMap feelingsMap = new BasicFeelingsMap();

    public Slider madSlider;
    public Slider scaredSlider;
    public Slider joyfulSlider;
    public Slider powerfulSlider;
    public Slider peacefulSlider;
    public Slider sadSlider;

    public void ApplyFeeling(string feeling)
    {
        feelingsMap.ApplyFeeling(feeling, 1.0f);

        RefreshSliders();
    }

    private void RefreshSliders()
    {
        madSlider.value = feelingsMap.GetFeeling(BasicFeelingsMap.Mad);
        scaredSlider.value = feelingsMap.GetFeeling(BasicFeelingsMap.Scared);
        joyfulSlider.value = feelingsMap.GetFeeling(BasicFeelingsMap.Joyful);
        powerfulSlider.value = feelingsMap.GetFeeling(BasicFeelingsMap.Powerful);
        peacefulSlider.value = feelingsMap.GetFeeling(BasicFeelingsMap.Peaceful);
        sadSlider.value = feelingsMap.GetFeeling(BasicFeelingsMap.Sad);
    }
}