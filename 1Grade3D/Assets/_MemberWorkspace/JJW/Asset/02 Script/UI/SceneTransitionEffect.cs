using System;
using _MemberWorkspace.JJW.Asset._02_Script.Managers;
using UnityEngine;

namespace _MemberWorkspace.JJW.Asset._02_Script.UI
{
    
    public class SceneTransitionEffect : MonoBehaviour
    {
        [SerializeField] private int canvasSortingOrder = 1000;

        private SpiralTransition _spiralTransition;

        public bool IsPlaying => _spiralTransition.IsPlaying;
        private static SceneTransitionEffect _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(transform.root.gameObject);
                return;
            }
            _instance = this;

            _spiralTransition = GetComponent<SpiralTransition>();

            Canvas rootCanvas = GetComponentInParent<Canvas>();
            if (rootCanvas != null)
            {
                rootCanvas.sortingOrder = canvasSortingOrder;
            }

            DontDestroyOnLoad(transform.root.gameObject);
        }

        private void OnEnable()
        {
            SceneFlowManager.Instance.OnSceneChangedStarted += HandleTransitionStarted;
            SceneFlowManager.Instance.OnSceneChanged += HandleTransitionCompleted;
        }

        private void OnDisable()
        {
            if (SceneFlowManager.Instance != null)
            {
                SceneFlowManager.Instance.OnSceneChangedStarted -= HandleTransitionStarted;
                SceneFlowManager.Instance.OnSceneChanged -= HandleTransitionCompleted;
            }
        }

        private void HandleTransitionStarted(SceneType type)
        {
            _spiralTransition.PlayClose(); 
        }

        private void HandleTransitionCompleted(SceneType type)
        {
            _spiralTransition.PlayOpen(); 
        }
    }
}