using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEssentials;

namespace UnityEssentialsEditor
{
	public class ListItem<T> : IEnumerable<ListItem<T>>
	{
		public string key;
		protected List<ListItem<T>> childItems = new List<ListItem<T>>();
		public bool foldout = true;

		public bool selectable = true;
		public T value;

		public ListItem<T> Parent { get; private set; }

		public int RecursiveChildCount
		{
			get
			{
				int i = 1;
				foreach(var c in childItems) i += c.RecursiveChildCount;
				return i;
			}
		}

		public int RecursiveVisibleItemCount
		{
			get
			{
				int i = 1;
				if(foldout)
				{
					foreach(var c in childItems) i += c.RecursiveVisibleItemCount;
				}
				return i;
			}
		}

		public int DirectChildCount => childItems.Count;

		public ListItem(string name, T value)
		{
			this.key = name;
			this.value = value;
		}

		public ListItem()
		{

		}

		protected virtual string GetTooltip()
		{
			return null;
		}

		public string GetFullPath(string separator)
		{
			if(Parent != null && !string.IsNullOrEmpty(Parent.key))
			{
				return Parent.GetFullPath(separator) + separator + key;
			}
			else
			{
				return key;
			}
		}

		public void AddChild(ListItem<T> child)
		{
			childItems.Add(child);
			child.Parent = this;
		}

		public void ForEachChild(bool recursive, System.Action<ListItem<T>> action)
		{
			foreach(var c in childItems)
			{
				action.Invoke(c);
				if(recursive && c.childItems?.Count > 0) c.ForEachChild(true, action);
			}
		}

		public IEnumerator<ListItem<T>> GetEnumerator()
		{
			return childItems.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return childItems.GetEnumerator();
		}
	}


	public abstract class SelectorWindow<T> : EditorWindow
	{

		protected SerializedObject targetPropertyObject;
		protected string targetPropertyPath;
		protected object lastPropertyValue;

		protected static GUIStyle searchField;
		protected static GUIStyle searchFieldEnd;
		protected static GUIStyle listItemEven;
		protected static GUIStyle listItemOdd;
		protected static GUIStyle listItemSelected;

		protected static GUIStyle textLabel;
		protected static GUIContent hierarchyViewContent;

		protected bool initialized;
		protected string searchInput = "";
		protected ListItem<T> listItems = new ListItem<T>();
		protected Vector2 scroll;
		protected bool hierarchicalView = true;

		private void OnEnable()
		{
			searchField = "ToolbarSeachTextField";
			searchFieldEnd = "ToolbarSeachCancelButton";
			listItemEven = "ObjectPickerResultsEven";
			listItemOdd = "ObjectPickerResultsOdd";
			listItemSelected = "OL SelectedRow";

			textLabel = new GUIStyle(EditorStyles.label) { alignment = TextAnchor.UpperLeft };
			hierarchyViewContent = EditorGUIUtility.IconContent("d_UnityEditor.HierarchyWindow");
			hierarchyViewContent.tooltip = "Toggle Hierarchical View";

			Init();
		}

		protected abstract void Init();

		private void OnGUI()
		{
			if(targetPropertyObject == null)
			{
				GUIUtility.ExitGUI();
				Close();
			}
			DrawSearchBar();
			DrawToolbar();
			DrawList();
		}

		private void DrawSearchBar()
		{
			if(Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
			{
				if(GUI.GetNameOfFocusedControl() == "search" && searchInput.Length > 0)
				{
					searchInput = "";
					UpdateSearch();
				}
			}
			var searchRect = GUILayoutUtility.GetRect(GUIContent.none, searchField, GUILayout.ExpandWidth(true));
			GUI.SetNextControlName("search");
			var newSearchString = GUI.TextField(searchRect, searchInput, searchField);
			if(!initialized)
			{
				GUI.FocusControl("search");
				initialized = true;
			}
			searchRect.xMin = searchRect.xMax - 14;
			if(!string.IsNullOrEmpty(newSearchString))
			{
				if(GUI.Button(searchRect, "", searchFieldEnd))
				{
					newSearchString = "";
				}
			}
			if(searchInput != newSearchString)
			{
				searchInput = newSearchString;
				UpdateSearch();
			}
		}

		protected virtual void DrawToolbar()
		{
			var r = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.toolbar, GUILayout.ExpandWidth(true));
			var fullRect = r;
			if(Event.current.type == EventType.Repaint) EditorStyles.toolbar.Draw(r, false, false, false, false);
			r.SplitHorizontalRight(10, out r, out _);
			var toolbar = r.SplitHorizontalMultiRight(3, 30, out r);
			if(GUI.Button(toolbar[0], new GUIContent("+", "Expand All"), EditorStyles.toolbarButton)) listItems.ForEachChild(true, c => c.foldout = true);
			if(GUI.Button(toolbar[1], new GUIContent("-", "Collapse All"), EditorStyles.toolbarButton)) listItems.ForEachChild(true, c => c.foldout = false);
			hierarchicalView = GUI.Toggle(toolbar[2], hierarchicalView, hierarchyViewContent, EditorStyles.toolbarButton);
			OnDrawToolbarExtras(r);
		}

		protected virtual void OnDrawToolbarExtras(Rect r)
		{

		}

		private void DrawList()
		{
			scroll = EditorGUILayout.BeginScrollView(scroll, false, true, GUIStyle.none, GUI.skin.verticalScrollbar, GUIStyle.none, GUILayout.Height(position.height - 40));
			int index = 0;
			int indent = 0;
			OnBeginDrawList();
			ListItem<T> selected = null;
			var searchKeywords = searchInput.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
			listItems.ForEachChild(false, (root) =>
			{
				var s = DrawItem(root, position.width - 15, hierarchicalView, searchKeywords, ref index, ref indent);
				if(selected == null && s != null) selected = s;
			});
			if(selected != null)
			{
				Apply(selected);
			}
			int visibleContentHeight = (hierarchicalView ? listItems.RecursiveVisibleItemCount : listItems.RecursiveChildCount) * 18;
			GUILayoutUtility.GetRect(position.width, visibleContentHeight);
			EditorGUILayout.EndScrollView();
		}

		protected virtual void OnBeginDrawList()
		{

		}

		protected virtual ListItem<T> DrawItem(ListItem<T> item, float width, bool hierarchy, string[] searchKeywords, ref int listIndex, ref int indentLevel)
		{
			ListItem<T> selection = null;
			bool isSelected = item.selectable && MatchesPropertyValue(lastPropertyValue, item);
			if(name != null)
			{
				bool hasKey = item.selectable && MatchSearch(item, searchKeywords);

				bool showHierarchy = hierarchy;
				if(searchKeywords.Length > 0)
				{
					bool childVisible = false;
					item.ForEachChild(true, c =>
					{
						childVisible |= MatchSearch(c, searchKeywords);
					});
					showHierarchy &= childVisible;
				}

				if(hasKey || showHierarchy)
				{

					Rect r = new Rect(0, listIndex * 18, width, 18);

					if(Event.current.type == EventType.Repaint)
					{
						GUIStyle s = isSelected ? listItemSelected : listIndex % 2 == 0 ? listItemEven : listItemOdd;
						s.Draw(r, false, false, false, false);
						if(r.Contains(Event.current.mousePosition))
						{
							if(item.selectable)
							{
								EditorGUI.DrawRect(r, new Color(1, 1, 1, 0.07f));
							}
							Repaint();
						}
					}
					r.xMin += 15 * indentLevel;
					r.SplitHorizontal(15, out var rFoldout, out var rKey);
					if(item.DirectChildCount > 0 && hierarchy)
					{
						item.foldout = EditorGUI.Foldout(rFoldout, item.foldout, GUIContent.none);
					}
					r.xMin += 15;
					if(item.selectable && GUI.Button(r, GUIContent.none, GUIStyle.none))
					{
						selection = item;
					}
					DrawItemContent(item, rKey, hasKey, hierarchy);
					listIndex++;
				}
			}
			if(item.foldout || !hierarchy)
			{
				bool indent = hierarchy && name != null;
				if(indent) indentLevel++;
				foreach(var child in item)
				{
					var s = DrawItem(child, width, hierarchy, searchKeywords, ref listIndex, ref indentLevel);
					if(s != null) selection = s;
				}
				if(indent) indentLevel--;
			}
			return selection;
		}

		protected abstract bool MatchesPropertyValue(object propertyValue, ListItem<T> item);

		protected virtual void DrawItemContent(ListItem<T> item, Rect position, bool hasKey, bool hierarchy)
		{
			GUI.Label(position, item.key);
		}

		protected virtual string GetTooltip(ListItem<string> item)
		{
			return null;
		}

		void UpdateSearch()
		{

		}

		void Apply(ListItem<T> item)
		{
			ApplyValue(targetPropertyObject.FindProperty(targetPropertyPath), item);
			targetPropertyObject.ApplyModifiedProperties();
			Close();
		}

		protected abstract void ApplyValue(SerializedProperty property, ListItem<T> item);

		protected virtual void OnLostFocus()
		{
			Close();
		}

		protected void Setup(SerializedProperty targetProperty)
		{
			targetPropertyObject = targetProperty.serializedObject;
			targetPropertyPath = targetProperty.propertyPath;
			try
			{
				lastPropertyValue = PropertyDrawerUtility.GetTargetObjectOfProperty(targetProperty);
			}
			catch(Exception e)
			{
				e.LogException("Failed to get last property value");
			}
		}

		protected virtual bool MatchSearch(ListItem<T> item, string[] searchKeywords)
		{
			bool b = true;
			foreach(var s in searchKeywords)
			{
				b &= item.key.Contains(s);
			}
			return b;
		}
	}
}
