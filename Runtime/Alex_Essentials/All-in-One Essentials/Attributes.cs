#region
using UnityEditor;
using UnityEngine;
#endregion

namespace Lumina.Essentials
{
#if UNITY_EDITOR
    public abstract class Attributes
    {
        /// <example> [SerializeField, ReadOnly] bool readOnlyBool; </example>
        /// <remarks> Allows you to add '[ReadOnly]' before a variable so that it is shown but not editable in the inspector. </remarks>
        public class ReadOnlyAttribute : PropertyAttribute
        {
        }

        /// <summary>
        ///     Allows you to add '[ReadOnly]' before a variable so that it is shown but not editable in the inspector.
        ///     Small but useful script, to make your inspectors look pretty and useful.
        ///     <example> [SerializeField, ReadOnly] int myInt; </example>
        /// </summary>
        [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
        public class ReadOnlyPropertyDrawer : PropertyDrawer
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                GUI.enabled = false;
                EditorGUI.PropertyField(position, property, label);
                GUI.enabled = true;
            }
        }
    }
#endif
}
