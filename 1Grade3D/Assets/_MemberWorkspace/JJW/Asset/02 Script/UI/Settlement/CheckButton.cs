using _MemberWorkspace.JJW.Asset._02_Script.Managers;
using DG.Tweening;
using UnityEngine;

namespace _MemberWorkspace.JJW.Asset._02_Script.UI.Settlement
{
    public class CheckButton : MonoBehaviour
    {
        [SerializeField] private float duration;
        [SerializeField] private CanvasGroup settlementUIPanel;
        public void OnClick()
        {
            
            settlementUIPanel.DOFade(0,duration).OnComplete(() =>
            {
                GameFlowManager.Instance.OpenShop();
                gameObject.SetActive(false);
            });
            
        }
    }
}