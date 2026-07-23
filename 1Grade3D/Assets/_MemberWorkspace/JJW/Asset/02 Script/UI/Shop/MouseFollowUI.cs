using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _MemberWorkspace.JJW.Asset._02_Script.UI.Shop
{
    public class MouseFollowUI : MonoBehaviour
    {
        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            _rectTransform.position = Mouse.current.position.ReadValue();
        }
    }
}