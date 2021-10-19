﻿using MelonLoader;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

[assembly: MelonInfo(typeof(GestureIndicator.GestureIndicator), "GestureIndicator", "1.0.5", "ImTiara", "https://github.com/ImTiara/VRCMods")]
[assembly: MelonGame("VRChat", "VRChat")]

namespace GestureIndicator
{
    public class GestureIndicator : MelonMod
    {
        private bool m_Enable;
        private Color m_LeftTextColor = Color.cyan;
        private Color m_RightTextColor = Color.cyan;
        private float m_TextOpacity;

        private TextMeshProUGUI m_LeftGestureText;
        private TextMeshProUGUI m_RightGestureText;

        public override void OnApplicationStart()
            => MelonCoroutines.Start(UiManagerInitializer());

        public void OnUiManagerInit()
        {
            MelonPreferences.CreateCategory(GetType().Name, "Gesture Indicator");
            MelonPreferences.CreateEntry(GetType().Name, "Enable", true, "Enable Gesture Indicator");
            MelonPreferences.CreateEntry(GetType().Name, "TextOpacity", 85f, "Text Opacity (%)");
            MelonPreferences.CreateEntry(GetType().Name, "LeftTextColor", "#00FFFF", "Left Text Color");
            MelonPreferences.CreateEntry(GetType().Name, "RightTextColor", "#00FFFF", "Right Text Color");

            CreateIndicators();

            OnPreferencesSaved();
        }

        public override void OnPreferencesSaved()
        {
            m_Enable = MelonPreferences.GetEntryValue<bool>(GetType().Name, "Enable");
            m_TextOpacity = MelonPreferences.GetEntryValue<float>(GetType().Name, "TextOpacity");
            m_LeftTextColor = Manager.HexToColor(MelonPreferences.GetEntryValue<string>(GetType().Name, "LeftTextColor"));
            m_RightTextColor = Manager.HexToColor(MelonPreferences.GetEntryValue<string>(GetType().Name, "RightTextColor"));

            ToggleIndicators(m_Enable);
            ApplyTextColors();
        }

        private IEnumerator CheckGesture()
        {
            while (m_Enable)
            {
                try
                {
                    if (Manager.GetLocalVRCPlayer() != null && m_LeftGestureText != null && m_RightGestureText != null)
                    {
                        if (Manager.GetGestureLeftWeight() >= 0.1f)
                        {
                            Manager.Gesture leftGesture = Manager.GetGesture(Manager.Hand.Left);
                            switch (leftGesture)
                            {
                                case Manager.Gesture.Fist:
                                    m_LeftGestureText.text = "Fist";
                                    break;
                                case Manager.Gesture.Open:
                                    m_LeftGestureText.text = "Hand Open";
                                    break;
                                case Manager.Gesture.Point:
                                    m_LeftGestureText.text = "Point";
                                    break;
                                case Manager.Gesture.Victory:
                                    m_LeftGestureText.text = "Victory";
                                    break;
                                case Manager.Gesture.RockNRoll:
                                    m_LeftGestureText.text = "RockNRoll";
                                    break;
                                case Manager.Gesture.Gun:
                                    m_LeftGestureText.text = "Hand Gun";
                                    break;
                                case Manager.Gesture.ThumbsUp:
                                    m_LeftGestureText.text = "Thumbs Up";
                                    break;
                            }
                        }
                        else m_LeftGestureText.text = "";

                        if (Manager.GetGestureRightWeight() >= 0.1f)
                        {
                            Manager.Gesture rightGesture = Manager.GetGesture(Manager.Hand.Right);
                            switch (rightGesture)
                            {
                                case Manager.Gesture.Fist:
                                    m_RightGestureText.text = "Fist";
                                    break;
                                case Manager.Gesture.Open:
                                    m_RightGestureText.text = "Hand Open";
                                    break;
                                case Manager.Gesture.Point:
                                    m_RightGestureText.text = "Point";
                                    break;
                                case Manager.Gesture.Victory:
                                    m_RightGestureText.text = "Victory";
                                    break;
                                case Manager.Gesture.RockNRoll:
                                    m_RightGestureText.text = "RockNRoll";
                                    break;
                                case Manager.Gesture.Gun:
                                    m_RightGestureText.text = "Hand Gun";
                                    break;
                                case Manager.Gesture.ThumbsUp:
                                    m_RightGestureText.text = "Thumbs Up";
                                    break;
                            }
                        }
                        else m_RightGestureText.text = "";
                    }
                }
                catch (Exception e) { MelonLogger.Error("Error checking gesture: " + e); }

                yield return new WaitForSeconds(.1f);
            }
        }

        private void CreateIndicators()
        {
            Transform hud = Manager.GetVRCUiManager().transform.Find("UnscaledUI/HudContent");
            GameObject textTemplate = Manager.GetQuickMenu().transform.Find("Container/Window/QMNotificationsArea/DebugInfoPanel/Panel/Text_FPS").gameObject;

            m_LeftGestureText = UnityEngine.Object.Instantiate(textTemplate, hud, true).GetComponent<TextMeshProUGUI>();
            UnityEngine.Object.Destroy(m_LeftGestureText.GetComponent<TextBinding>());
            m_LeftGestureText.name = "GestureIndicator(Left)";
            RectTransform rectTransformLeft = m_LeftGestureText.GetComponent<RectTransform>();
            rectTransformLeft.anchoredPosition = new Vector3(-300f, -415f, 0);
            rectTransformLeft.localScale = new Vector2(1.0f, 1.0f);
            rectTransformLeft.sizeDelta = new Vector2(200f, -946f);
            m_LeftGestureText.text = "";
            m_LeftGestureText.alignment = TextAlignmentOptions.MidlineLeft;
            m_LeftGestureText.fontStyle = FontStyles.Normal;

            m_RightGestureText = UnityEngine.Object.Instantiate(textTemplate, hud, true).GetComponent<TextMeshProUGUI>();
            UnityEngine.Object.Destroy(m_RightGestureText.GetComponent<TextBinding>());
            m_RightGestureText.name = "GestureIndicator(Right)";
            RectTransform rectTransformRight = m_RightGestureText.GetComponent<RectTransform>();
            rectTransformRight.anchoredPosition = new Vector3(150f, -415f, 0);
            rectTransformRight.localScale = new Vector2(1.0f, 1.0f);
            rectTransformRight.sizeDelta = new Vector2(200f, -946f);
            m_RightGestureText.text = "";
            m_RightGestureText.alignment = TextAlignmentOptions.MidlineRight;
            m_RightGestureText.fontStyle = FontStyles.Normal;

            ApplyTextColors();
        }

        private void ApplyTextColors()
        {
            if (m_LeftGestureText != null && m_RightGestureText != null)
            {
                float op = m_TextOpacity / 100.0f;

                Color colorL = m_LeftTextColor;
                colorL.a = op;
                m_LeftGestureText.color = colorL;

                Color colorR = m_RightTextColor;
                colorR.a = op;
                m_RightGestureText.color = colorR;
            }
        }

        private void ToggleIndicators(bool enable)
        {
            if (enable) MelonCoroutines.Start(CheckGesture());

            m_LeftGestureText.gameObject.SetActive(enable);
            m_RightGestureText.gameObject.SetActive(enable);
        }

        public IEnumerator UiManagerInitializer()
        {
            while (VRCUiManager.prop_VRCUiManager_0 == null) yield return null;
            OnUiManagerInit();
        }
    }
}