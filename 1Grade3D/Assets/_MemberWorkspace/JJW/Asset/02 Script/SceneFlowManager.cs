using UnityEngine;

namespace _MemberWorkspace.JJW.Asset._02_Script
{
    public class SceneFlowManager : MonoBehaviour
    {
        public static SceneFlowManager Instance { get; private set; }

        public enum SceneType { MainMenu, InGame, StormCutscene }
        public SceneType sceneType;

    }
}
