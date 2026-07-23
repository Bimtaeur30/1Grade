using DG.Tweening;
using TMPro;
using UnityEngine;

namespace _MemberWorkspace.JJW.Asset._02_Script.UIUtility
{
    public static class CountUpText
    {
        public static Tween Animate(TextMeshProUGUI target, int from, int to, float duration, Ease ease = Ease.Linear)
        {
            int display = from;
            return DOTween.To(() => display, x =>
            {
                display = x;
                target.text = display.ToString();
            }, to, duration).SetEase(ease);
        }

        public static Tween Animate(TextMeshProUGUI target, int from, int to, float duration, string format, Ease ease = Ease.Linear)
        {
            int display = from;
            return DOTween.To(() => display, x =>
            {
                display = x;
                target.text = string.Format(format, display);
            }, to, duration).SetEase(ease);
        }
    }

    public static class TextPunch
    {
        public static Tween Play(TextMeshProUGUI target, float punchScale = 0.3f, float duration = 0.3f, int vibrato = 8, float elasticity = 0.8f)
        {
            target.transform.DOKill(); 
            target.transform.localScale = Vector3.one;
            return target.transform.DOPunchScale(Vector3.one * punchScale, duration, vibrato, elasticity);
        }
    }
}