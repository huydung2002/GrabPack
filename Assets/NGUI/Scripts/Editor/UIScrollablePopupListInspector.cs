//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

#if !UNITY_3_5 && !UNITY_FLASH
#define DYNAMIC_FONT
#endif

using NGUI.Scripts.Interaction;
using UnityEditor;
using UnityEngine;

namespace NGUI.Scripts.Editor
{
	/// <summary>
	/// Inspector class used to edit UIPopupLists.
	/// </summary>

	[CustomEditor(typeof(UIScrollablePopupList))]
	public class UIScrollablePopupListInspector : UIWidgetContainerEditor
	{
		enum FontType
		{
			NGUI,
			Unity,
		}

		UIScrollablePopupList mList;
		FontType mType;

		void OnEnable ()
		{
			SerializedProperty bit = serializedObject.FindProperty("bitmapFont");
			mType = (bit.objectReferenceValue != null) ? FontType.NGUI : FontType.Unity;
			mList = target as UIScrollablePopupList;

			if (mList.ambigiousFont == null)
			{
				mList.ambigiousFont = NGUISettings.ambigiousFont;
				mList.fontSize = NGUISettings.fontSize;
				mList.fontStyle = NGUISettings.fontStyle;
				EditorUtility.SetDirty(mList);
			}

			if (mList.atlas == null)
			{
				mList.atlas = NGUISettings.atlas as UIAtlas;
				mList.backgroundSprite = NGUISettings.selectedSprite;
				mList.highlightSprite = NGUISettings.selectedSprite;
				EditorUtility.SetDirty(mList);
			}

			// Lumos: add the scrollbar's atlas, set up the colours
			if (mList.scrollbarAtlas == null) {
				mList.scrollbarAtlas = NGUISettings.atlas as UIAtlas;
				mList.scrollbarSpriteName = NGUISettings.selectedSprite;
				mList.scrollbarForegroundName = NGUISettings.selectedSprite;
				mList.scrollbarBgDefColour = mList.gameObject.GetComponent<UISprite>().color;
				mList.scrollbarBgHovColour = mList.highlightColor;
				mList.scrollbarBgPrsColour = mList.backgroundColor;
				mList.scrollbarFgDefColour = mList.backgroundColor;
				mList.scrollbarFgHovColour = mList.highlightColor;
				mList.scrollbarFgPrsColour = mList.backgroundColor;

				EditorUtility.SetDirty(mList);
			}
		}

		void RegisterUndo ()
		{
			NGUIEditorTools.RegisterUndo("Scrollable Popup List Change", mList); // lawl
		}

		void OnSelectAtlas (Object obj)
		{
			RegisterUndo();
			mList.atlas = obj as UIAtlas;
			NGUISettings.atlas = mList.atlas as INGUIAtlas;
		}

		// Lumos:
		void OnSelectScrollbarAtlas (Object obj)
		{
			RegisterUndo();
			mList.scrollbarAtlas = obj as UIAtlas;
			NGUISettings.atlas = mList.scrollbarAtlas as INGUIAtlas;
			Repaint();
		}

		void OnBackground (string spriteName)
		{
			RegisterUndo();
			mList.backgroundSprite = spriteName;
			Repaint();
		}

		void OnHighlight (string spriteName)
		{
			RegisterUndo();
			mList.highlightSprite = spriteName;
			Repaint();
		}

		// Lumos:
		void OnScrollbarBackground (string spriteName) {
			RegisterUndo();
			mList.scrollbarSpriteName = spriteName;
			Repaint();
		}
		void OnScrollbarForeground (string spriteName) {
			RegisterUndo();
			mList.scrollbarForegroundName = spriteName;
			Repaint();
		}

		void OnBitmapFont (Object obj)
		{
			serializedObject.Update();
			SerializedProperty sp = serializedObject.FindProperty("bitmapFont");
			sp.objectReferenceValue = obj;
			serializedObject.ApplyModifiedProperties();
			NGUISettings.ambigiousFont = obj;
		}

		void OnDynamicFont (Object obj)
		{
			serializedObject.Update();
			SerializedProperty sp = serializedObject.FindProperty("trueTypeFont");
			sp.objectReferenceValue = obj;
			serializedObject.ApplyModifiedProperties();
			NGUISettings.ambigiousFont = obj;
		}

		public override void OnInspectorGUI ()
		{
			serializedObject.Update();
			NGUIEditorTools.SetLabelWidth(80f);

			GUILayout.BeginHorizontal();
			GUILayout.Space(6f);
			GUILayout.Label("Options");
			GUILayout.EndHorizontal();

			string text = "";
			foreach (string s in mList.items) text += s + "\n";

			GUILayout.Space(-14f);
			GUILayout.BeginHorizontal();
			GUILayout.Space(84f);
			string modified = EditorGUILayout.TextArea(text, GUILayout.Height(100f));
			GUILayout.EndHorizontal();

			if (modified != text)
			{
				RegisterUndo();
				string[] split = modified.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
				mList.items.Clear();
				foreach (string s in split) mList.items.Add(s);

				if (string.IsNullOrEmpty(mList.value) || !mList.items.Contains(mList.value))
				{
					mList.value = mList.items.Count > 0 ? mList.items[0] : "";
				}
			}

			GUI.changed = false;
			string sel = NGUIEditorTools.DrawList("Default", mList.items.ToArray(), mList.value);
			if (GUI.changed) serializedObject.FindProperty("mSelectedItem").stringValue = sel;

			NGUIEditorTools.DrawProperty("Position", serializedObject, "position");
			NGUIEditorTools.DrawProperty("Alignment", serializedObject, "alignment");
			NGUIEditorTools.DrawProperty("Localized", serializedObject, "isLocalized");
			NGUIEditorTools.DrawProperty("Max Height", serializedObject, "maxHeight");

			DrawAtlas();
			DrawFont();

			// Lumos:
			DrawScrollbarAtlas();

			NGUIEditorTools.DrawEvents("On Value Change", mList, mList.onChange);

			serializedObject.ApplyModifiedProperties();
		}

		void OnNGUIFont (Object obj)
		{
			if (obj != null && obj is GameObject) obj = (obj as GameObject).GetComponent<UIFont>();
			serializedObject.Update();
			var sp = serializedObject.FindProperty("bitmapFont");
			sp.objectReferenceValue = obj;
			sp = serializedObject.FindProperty("trueTypeFont");
			sp.objectReferenceValue = null;
			serializedObject.ApplyModifiedProperties();
			NGUISettings.ambigiousFont = obj;
		}
	
		void DrawAtlas()
		{
			if (NGUIEditorTools.DrawHeader("Atlas"))
			{
				NGUIEditorTools.BeginContents();

				GUILayout.BeginHorizontal();
				{
					if (NGUIEditorTools.DrawPrefixButton("Atlas"))
						ComponentSelector.Show<UIAtlas>(OnSelectAtlas);
					NGUIEditorTools.DrawProperty("", serializedObject, "atlas");
				}
				GUILayout.EndHorizontal();

				NGUIEditorTools.DrawPaddedSpriteField("Background", mList.atlas as INGUIAtlas, mList.backgroundSprite, OnBackground);
				NGUIEditorTools.DrawPaddedSpriteField("Highlight", mList.atlas as INGUIAtlas, mList.highlightSprite, OnHighlight);

				EditorGUILayout.Space();

				NGUIEditorTools.DrawProperty("Background", serializedObject, "backgroundColor");
				NGUIEditorTools.DrawProperty("Highlight", serializedObject, "highlightColor");
				NGUIEditorTools.DrawProperty("Animated", serializedObject, "isAnimated");
				NGUIEditorTools.EndContents();
			}
		}

		// Lumos:
		void DrawScrollbarAtlas() {
			if (NGUIEditorTools.DrawHeader("Scrollbar"))
			{
				NGUIEditorTools.BeginContents();
			
				GUILayout.BeginHorizontal();
				{
					if (NGUIEditorTools.DrawPrefixButton("Atlas"))
						ComponentSelector.Show<UIAtlas>(OnSelectScrollbarAtlas);
					NGUIEditorTools.DrawProperty("", serializedObject, "scrollbarAtlas");
				}
				GUILayout.EndHorizontal();
			
				NGUIEditorTools.DrawPaddedSpriteField("Background", mList.scrollbarAtlas as INGUIAtlas, mList.scrollbarSpriteName, OnScrollbarBackground);
				NGUIEditorTools.DrawPaddedSpriteField("Foreground", mList.scrollbarAtlas as INGUIAtlas, mList.scrollbarForegroundName, OnScrollbarForeground);
			
				EditorGUILayout.Space();
			
				NGUIEditorTools.DrawProperty("Bg Default", serializedObject, "scrollbarBgDefColour");
				NGUIEditorTools.DrawProperty("Bg Hover",   serializedObject, "scrollbarBgHovColour");
				NGUIEditorTools.DrawProperty("Bg Pressed", serializedObject, "scrollbarBgPrsColour");
				EditorGUILayout.Space();
				NGUIEditorTools.DrawProperty("Fg Default", serializedObject, "scrollbarFgDefColour");
				NGUIEditorTools.DrawProperty("Fg Hover",   serializedObject, "scrollbarFgHovColour");
				NGUIEditorTools.DrawProperty("Fg Pressed", serializedObject, "scrollbarFgPrsColour");
				NGUIEditorTools.EndContents();
			}
		}

		void DrawFont ()
		{
			if (NGUIEditorTools.DrawHeader("Font"))
			{
				NGUIEditorTools.BeginContents();

				SerializedProperty ttf = null;

				GUILayout.BeginHorizontal();
				{
					if (NGUIEditorTools.DrawPrefixButton("Font"))
					{
						if (mType == FontType.NGUI)
						{
							var bmf = mList.bitmapFont;
							if (bmf != null && bmf is UIFont) ComponentSelector.Show<UIFont>(OnNGUIFont);
							else ComponentSelector.Show<NGUIFont>(OnNGUIFont);
						}
						else ComponentSelector.Show<Font>(OnDynamicFont, new string[] { ".ttf", ".otf"});
					}

#if DYNAMIC_FONT
					GUI.changed = false;
					mType = (FontType)EditorGUILayout.EnumPopup(mType, GUILayout.Width(62f));

					if (GUI.changed)
					{
						GUI.changed = false;

						if (mType == FontType.NGUI) serializedObject.FindProperty("trueTypeFont").objectReferenceValue = null;
						else serializedObject.FindProperty("bitmapFont").objectReferenceValue = null;
					}
#else
				mType = FontType.Bitmap;
#endif

					if (mType == FontType.NGUI)
					{
						var fnt = NGUIEditorTools.DrawProperty("", serializedObject, "bitmapFont", GUILayout.MinWidth(40f));

						// Legacy font support
						if (fnt.objectReferenceValue != null && fnt.objectReferenceValue is GameObject)
							fnt.objectReferenceValue = (fnt.objectReferenceValue as GameObject).GetComponent<UIFont>();

						if (GUI.changed)
						{
							serializedObject.FindProperty("trueTypeFont").objectReferenceValue = null;
							NGUISettings.ambigiousFont = fnt.objectReferenceValue;
						}
					}
					else
					{
						ttf = NGUIEditorTools.DrawProperty("", serializedObject, "trueTypeFont", GUILayout.MinWidth(40f));

						if (GUI.changed)
						{
							serializedObject.FindProperty("bitmapFont").objectReferenceValue = null;
							NGUISettings.ambigiousFont = ttf.objectReferenceValue;
						}
					}
				}
				GUILayout.EndHorizontal();

				if (ttf != null && ttf.objectReferenceValue != null)
				{
					GUILayout.BeginHorizontal();
					{
						EditorGUI.BeginDisabledGroup(ttf.hasMultipleDifferentValues);
						NGUIEditorTools.DrawProperty("Font Size", serializedObject, "fontSize", GUILayout.Width(142f));
						NGUIEditorTools.DrawProperty("", serializedObject, "fontStyle", GUILayout.MinWidth(40f));
						GUILayout.Space(18f);
						EditorGUI.EndDisabledGroup();
					}
					GUILayout.EndHorizontal();
				}
				else NGUIEditorTools.DrawProperty("Font Size", serializedObject, "fontSize", GUILayout.Width(142f));

				NGUIEditorTools.DrawProperty("Text Color", serializedObject, "textColor");

				GUILayout.BeginHorizontal();
				NGUIEditorTools.SetLabelWidth(66f);
				EditorGUILayout.PrefixLabel("Padding");
				NGUIEditorTools.SetLabelWidth(14f);
				NGUIEditorTools.DrawProperty("X", serializedObject, "padding.x", GUILayout.MinWidth(30f));
				NGUIEditorTools.DrawProperty("Y", serializedObject, "padding.y", GUILayout.MinWidth(30f));
				GUILayout.Space(18f);
				NGUIEditorTools.SetLabelWidth(80f);
				GUILayout.EndHorizontal();

				NGUIEditorTools.EndContents();
			}
		}
	}
}
