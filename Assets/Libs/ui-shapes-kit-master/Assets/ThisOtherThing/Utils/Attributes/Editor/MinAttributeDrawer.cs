using UnityEngine;
using UnityEditor;

using ThisOtherThing.Utils;

namespace ThisOtherThing
{
	[CustomPropertyDrawer(typeof(UnityEngine.MinAttribute))] 
	public class MinDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			UnityEngine.MinAttribute attribute = (UnityEngine.MinAttribute)base.attribute;

			switch (property.propertyType)
			{
				case SerializedPropertyType.Integer:
					int valueI = EditorGUI.IntField(position, label, property.intValue);
					property.intValue = Mathf.Max(valueI, (int)attribute.min);
					break;
				case SerializedPropertyType.Float:
					float valueF = EditorGUI.FloatField(position, label, property.floatValue);
					property.floatValue = Mathf.Max(valueF, attribute.min);
					break;
			}
		}
	}
}
