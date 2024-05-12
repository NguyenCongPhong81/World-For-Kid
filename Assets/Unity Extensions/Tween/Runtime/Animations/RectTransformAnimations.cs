using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
using UnityExtensions.Editor;
#endif

namespace UnityExtensions.Tween
{
            [Serializable, TweenAnimation("Transform/RectTransform", "Transform")]
    public class TweenRectTransform : TweenFromTo<RectTransform>
    {
        public bool togglePosition = default;
        public bool toggleRotation = default;
        public bool toggleLocalScale = default;
        public RectTransform target = default;

 

        public Vector3 currentPosition
        {
            get => target ? target.localPosition : default;
            set { if (target) target.localPosition = value; }
        }

        public Quaternion currentRotation
        {
            get => target ? target.rotation : Quaternion.identity;
            set { if (target) target.rotation = value; }
        }

        public Vector3 currentLocalScale
        {
            get => target ? target.localScale : Vector3.one;
            set { if (target) target.localScale = value; }
        }

        public override void Interpolate(float factor)
        {
            if (From && to && target)
            {
                if (togglePosition)
                {
                    currentPosition = Vector3.LerpUnclamped(From.localPosition, to.localPosition, factor);
                }
                if (toggleRotation)
                {
                    currentRotation = Quaternion.SlerpUnclamped(From.rotation, to.rotation, factor);
                }
                if (toggleLocalScale)
                {
                    currentLocalScale = Vector3.LerpUnclamped(From.localScale, to.localScale, factor);
                }
            }
        }

#if UNITY_EDITOR

        RectTransform _originalTarget;
        Vector3 _tempPosition;
        Quaternion _tempRotation;
        Vector3 _tempLocalScale;

        public override void Reset(TweenPlayer player)
        {
            base.Reset(player);
            togglePosition = default;
            toggleRotation = default;
            toggleLocalScale = default;
           // target = target.rectTransform();
        }

        public override void RecordState()
        {
            _originalTarget = target;
            _tempPosition = currentPosition;
            _tempRotation = currentRotation;
            _tempLocalScale = currentLocalScale;
        }

        public override void RestoreState()
        {
            var currentTarget = target;
            target = _originalTarget;
            if (togglePosition)
            {
                currentPosition = _tempPosition;
            }

            if (toggleRotation)
            {

                currentRotation = _tempRotation;
            }

            if (toggleLocalScale)
            {
                currentLocalScale = _tempLocalScale;
            }

            target = currentTarget;
        }

        protected override void OnPropertiesGUI(TweenPlayer player, SerializedProperty property)
        {
            using (DisabledScope.New(player.playing))
            {
                EditorGUILayout.PropertyField(property.FindPropertyRelative(nameof(target)));
            }

            var rect = EditorGUILayout.GetControlRect();
            float labelWidth = EditorGUIUtility.labelWidth;

            var fromRect = new Rect(rect.x + labelWidth, rect.y, (rect.width - labelWidth - 8) / 2, rect.height);
            var toRect = new Rect(rect.xMax - fromRect.width, fromRect.y, fromRect.width, fromRect.height);

            var togglePositionProp = property.FindPropertyRelative(nameof(togglePosition));
            var toggleRotationProp = property.FindPropertyRelative(nameof(toggleRotation));
            var toggleLocalScaleProp = property.FindPropertyRelative(nameof(toggleLocalScale));
            
            rect.width = rect.height + EditorStyles.label.CalcSize(EditorGUIUtilities.TempContent("P")).x;
            togglePositionProp.boolValue = EditorGUI.ToggleLeft(rect, "P", togglePositionProp.boolValue);

            rect.x = rect.xMax + rect.height * 0.5f;
            rect.width = rect.height + EditorStyles.label.CalcSize(EditorGUIUtilities.TempContent("R")).x;
            toggleRotationProp.boolValue = EditorGUI.ToggleLeft(rect, "R", toggleRotationProp.boolValue);

            rect.x = rect.xMax + rect.height * 0.5f;
            rect.width = rect.height + EditorStyles.label.CalcSize(EditorGUIUtilities.TempContent("S")).x;
            toggleLocalScaleProp.boolValue = EditorGUI.ToggleLeft(rect, "S", toggleLocalScaleProp.boolValue);

            using (DisabledScope.New(!togglePosition && !toggleRotation && !toggleLocalScale))
            {
                using (LabelWidthScope.New(12))
                {
                    var (runtimeFromProp,fromProp, toProp) = GetFromToProperties(property);
                    EditorGUILayout.PropertyField(runtimeFromProp);
                    EditorGUI.BeginDisabledGroup(runtimeFromProp.boolValue);
                    EditorGUI.ObjectField(fromRect, fromProp, EditorGUIUtilities.TempContent("F"));
                    EditorGUI.EndDisabledGroup();
                    EditorGUI.ObjectField(toRect, toProp, EditorGUIUtilities.TempContent("T"));
                }
            }
        }

        protected override void CreateOptionsMenu(GenericMenu menu, TweenPlayer player, int index)
        {
            base.CreateOptionsMenu(menu, player, index);

            if (!From || !target) menu.AddDisabledItem(new GUIContent("From = Current"));
            else menu.AddItem(new GUIContent("From = Current"), false, () =>
            {
                Undo.RecordObject(From, "From = Current");
                From.position = currentPosition;
                From.rotation = currentRotation;
                From.localScale = currentLocalScale;
            });

            if (!to || !target) menu.AddDisabledItem(new GUIContent("To = Current"));
            else menu.AddItem(new GUIContent("To = Current"), false, () =>
            {
                Undo.RecordObject(to, "To = Current");
                to.position = currentPosition;
                to.rotation = currentRotation;
                to.localScale = currentLocalScale;
            });

            if (!From || !target) menu.AddDisabledItem(new GUIContent("Current = From"));
            else menu.AddItem(new GUIContent("Current = From"), false, () =>
            {
                Undo.RecordObject(target, "Current = From");
                currentPosition = From.position;
                currentRotation = From.rotation;
                currentLocalScale = From.localScale;
            });

            if (!to || !target) menu.AddDisabledItem(new GUIContent("Current = To"));
            else menu.AddItem(new GUIContent("Current = To"), false, () =>
            {
                Undo.RecordObject(target, "Current = To");
                currentPosition = to.position;
                currentRotation = to.rotation;
                currentLocalScale = to.localScale;
            });
        }

#endif // UNITY_EDITOR

    } // class TweenTransform
    [Serializable, TweenAnimation("Rect Transform/Size Delta", "Rect Transform Size Delta")]
    public class TweenRectTransformSizeDelta : TweenVector2<RectTransform>
    {
        public override Vector2 current
        {
            get => target ? target.sizeDelta : default;
            set { if (target) target.sizeDelta = value; }
        }
    }

    [Serializable, TweenAnimation("Rect Transform/Anchored Position", "Rect Transform Anchored Position")]
    public class TweenRectTransformAnchoredPosition : TweenVector2<RectTransform>
    {
        public override Vector2 current
        {
            get => target ? target.anchoredPosition : default;
            set { if (target) target.anchoredPosition = value; }
        }
    }

    [Serializable, TweenAnimation("Rect Transform/Offset Max", "Rect Transform Offset Max")]
    public class TweenRectTransformOffsetMax : TweenVector2<RectTransform>
    {
        public override Vector2 current
        {
            get => target ? target.offsetMax : default;
            set { if (target) target.offsetMax = value; }
        }
    }

    [Serializable, TweenAnimation("Rect Transform/Offset Min", "Rect Transform Offset Min")]
    public class TweenRectTransformOffsetMin : TweenVector2<RectTransform>
    {
        public override Vector2 current
        {
            get => target ? target.offsetMin : default;
            set { if (target) target.offsetMin = value; }
        }
    }

    [Serializable, TweenAnimation("Rect Transform/Anchor Max", "Rect Transform Anchor Max")]
    public class TweenRectTransformAnchorMax : TweenVector2<RectTransform>
    {
        public override Vector2 current
        {
            get => target ? target.anchorMax : default;
            set { if (target) target.anchorMax = value; }
        }
    }

    [Serializable, TweenAnimation("Rect Transform/Anchor Min", "Rect Transform Anchor Min")]
    public class TweenRectTransformAnchorMin : TweenVector2<RectTransform>
    {
        public override Vector2 current
        {
            get => target ? target.anchorMin : default;
            set { if (target) target.anchorMin = value; }
        }
    }

} // namespace UnityExtensions.Tween