using UnityEngine;
using UnityEngine.UI;

namespace Assets.Feelings.Scripts.Sample
{
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
            if (madSlider != null)
                madSlider.value = feelingsMap.GetFeeling(BasicFeelingsMap.Mad);
            if (scaredSlider != null)
                scaredSlider.value = feelingsMap.GetFeeling(BasicFeelingsMap.Scared);
            if (joyfulSlider != null)
                joyfulSlider.value = feelingsMap.GetFeeling(BasicFeelingsMap.Joyful);
            if (powerfulSlider != null)
                powerfulSlider.value = feelingsMap.GetFeeling(BasicFeelingsMap.Powerful);
            if (peacefulSlider != null)
                peacefulSlider.value = feelingsMap.GetFeeling(BasicFeelingsMap.Peaceful);
            if (sadSlider != null)
                sadSlider.value = feelingsMap.GetFeeling(BasicFeelingsMap.Sad);
        }
    }
}