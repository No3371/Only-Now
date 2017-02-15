using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Rotorz.ReorderableList;
using System.Linq;

	namespace DataEditor.InfoLogData{
	public class InfoLogEditor : EditorWindow{

#region Editor Initialization

		private static InfoLogEditor _editor;
		public static InfoLogEditor editor { get { AssureEditor(); return _editor; }}

		public static void AssureEditor()
		{
			if (_editor == null) OpenEditor(); 
		}

		[MenuItem("Window/Data Editors/Info Log Data")]
		public static InfoLogEditor OpenEditor(){
			_editor = GetWindow<InfoLogEditor>();
			_editor.minSize = new Vector2(1280, 600);
			_editor.maxSize = new Vector2(1440, 720);

			_editor.titleContent = new GUIContent ("Info Data Editor");
			return _editor;
		}

		void Init(){
			if (databaseCache == null) LoadDatabase();
			if (groupNamesCache == null) CacheGroupTags();

			if (_listControl == null){
				_listControl = new ReorderableListControl();
				_listControl.ItemInserted += OnEntryItemInserted;
        		_listControl.ItemRemoving += OnEntryItemRemoving;
			}

			if (DrawSideBar_Group == null){
				if (databaseCache.entryGroupList.Count == 0) DrawSideBar_Group = DrawSideBar_Groups_Empty;
				else DrawSideBar_Group = DrawSideBar_Groups_Normal;
			}
			if (DrawSideBar_Entry == null)
			{
				DrawSideBar_Entry = DrawSideBar_Entries_None;
			}
			if (DrawEditor == null) DrawEditor = DrawEditor_NotEditing;

			LoadTexture();

		}

		Texture2D BoxBG;

		void LoadTexture(){
			if (BoxBG == null)
			{
				BoxBG = Resources.Load("EditorFakeTransparentPattern", typeof(Texture2D)) as Texture2D;
				GUI.skin.GetStyle("box").normal.background = BoxBG;
			}
		}

#endregion

#region Declarations

		int sidebarWidth = 160, windowInset = 4;

		Rect rect_Sidebar_Group {
			get { return new Rect( 0, 0, sidebarWidth, position.height); }
		}
		Rect rect_Sidebar_Entry {
			get { return new Rect( sidebarWidth, 0, sidebarWidth*2, position.height); }
		}
		Rect rect_Editor {
			get { return new Rect( sidebarWidth * 3, 0, position.width - sidebarWidth * 3, position.height); }
		}



#endregion

#region GUI
		delegate void Drawer();
		Drawer DrawSideBar_Group, DrawSideBar_Entry, DrawEditor;

		void OnGUI(){
			Init();
			GUILayout.BeginVertical();
				GUILayout.Space(4);			
				GUILayout.BeginHorizontal();
					GUILayout.Space(4);
					DrawSideBar_Group();
					GUILayout.Space(4);
					DrawSideBar_Entry();		
					DrawEditor();

				GUILayout.EndHorizontal();	
				GUILayout.Space(4);			
			GUILayout.EndVertical();

			if (selectedGroupIndex != selectedGroupIndexCache) OnSidebarSelectionChanged();

		}

	#region Sidebar(Groups)

		int selectedGroupIndex = -1, selectedGroupIndexCache = -1;

		string[] groupNamesCache;

		Vector2 scrollPos_GroupBar;

		void DrawSideBar_Groups_Normal(){
			scrollPos_GroupBar = GUILayout.BeginScrollView(scrollPos_GroupBar, GUILayout.Width(sidebarWidth));
				GUILayout.BeginVertical();
					GUILayout.Label("Groups");
					GUILayout.BeginVertical(GUILayout.ExpandHeight(true));
						selectedGroupIndex = GUILayout.SelectionGrid(selectedGroupIndex, groupNamesCache, 1);
					GUILayout.EndVertical();
					if (GUILayout.Button("Add New Group")){
						OnAddNewGroupButtonClicked();
					}
					if (GUILayout.Button("Delete Selected Group")){
						OnDeleteSelectedButtonClicked();
					}			
				GUILayout.EndVertical();
				GUILayout.EndScrollView();

			Rect temp = GUILayoutUtility.GetLastRect();
			Handles.color = new Color(0, 0, 0, 0.4f);
			Handles.DrawLine(new Vector2(temp.xMax, temp.yMax), new Vector2(temp.xMax, temp.yMin));
		}

		void DrawSideBar_Groups_Empty(){
			GUILayout.BeginVertical(GUILayout.Width(sidebarWidth));
				TextAnchor tempTAnchor = GUI.skin.label.alignment;
				GUI.skin.label.alignment = TextAnchor.MiddleCenter;
				GUILayout.Label("Database is empty.", GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
				GUI.skin.label.alignment = tempTAnchor;
				if (GUILayout.Button("Add New Group")){
					OnAddNewGroupButtonClicked();
				}
				EditorGUI.BeginDisabledGroup(true);
					if (GUILayout.Button("Delete Selected Group")){
						
					}			
				EditorGUI.EndDisabledGroup();
			GUILayout.EndVertical();

			Rect temp = GUILayoutUtility.GetLastRect();
			Handles.color = new Color(0, 0, 0, 0.4f);
			Handles.DrawLine(new Vector2(temp.xMax, temp.yMax), new Vector2(temp.xMax, temp.yMin));
		}

		void CacheGroupTags(){
			groupNamesCache = databaseCache.entryGroupList.Select(e => e.groupTag).ToArray();
		}

	#endregion

	#region Sidebar(Entries)

		private ReorderableListControl _listControl;
		private IReorderableListAdaptor _listAdaptor;

		string[] entryNamesCache;

		Vector2 scrollPos_EntryBar;

		void DrawSideBar_Entries_Normal(){
			scrollPos_EntryBar = GUILayout.BeginScrollView(scrollPos_EntryBar, GUILayout.Width(sidebarWidth*2));
				GUILayout.BeginVertical();
					GUILayout.Label("Entries");
					GUILayout.BeginVertical(GUILayout.ExpandHeight(true));
						_listControl.Draw(_listAdaptor);
					GUILayout.EndVertical();
				GUILayout.EndVertical();
			GUILayout.EndScrollView();

			Rect temp = GUILayoutUtility.GetLastRect();
			Handles.color = new Color(0, 0, 0, 0.4f);
			Handles.DrawLine(new Vector2(temp.xMax, temp.yMax), new Vector2(temp.xMax, temp.yMin));

		}

		void DrawSideBar_Entries_None(){
			GUILayout.BeginVertical(GUILayout.Width(sidebarWidth*2));
				TextAnchor tempTAnchor = GUI.skin.label.alignment;
				GUI.skin.label.alignment = TextAnchor.MiddleCenter;
				GUILayout.Label("Select a group to start editing", GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
				GUI.skin.label.alignment = tempTAnchor;	
			GUILayout.EndVertical();

			Rect temp = GUILayoutUtility.GetLastRect();
			Handles.color = new Color(0, 0, 0, 0.4f);
			Handles.DrawLine(new Vector2(temp.xMax, temp.yMax), new Vector2(temp.xMax, temp.yMin));
		}

		InfoLogEntry DrawListEntry(Rect position, InfoLogEntry entry){
			EditorGUI.SelectableLabel(position, entry.briefDescription);
			return entry;
		}

		void CacheEntryTags(){
			if (infoGroupCache != null) entryNamesCache = infoGroupCache.entries.Select(e => e.briefDescription).ToArray();
		}

		void OnEntryItemInserted(object sender, ItemInsertedEventArgs args){
			
			InfoLogEntry newEntry = InfoLogEntry.Create();
			AddSubAsset(newEntry, infoGroupCache);
			infoGroupCache.entries[args.ItemIndex] = newEntry;
			
			DrawSideBar_Entry = DrawSideBar_Entries_Normal;
			DrawEditor =DrawEditor_Normal;

			SaveDatabase();

		}
		void OnEntryItemRemoving(object sender, ItemRemovingEventArgs args){
			
			DestroyImmediate(infoGroupCache.entries[args.ItemIndex], true);
			
			DrawSideBar_Entry = DrawSideBar_Entries_Normal;
			DrawEditor =DrawEditor_Normal;

			SaveDatabase();

		}

	#endregion
	
	
	#region Editor

		Vector2 scrollPos_Editor;

		void DrawEditor_Normal(){
			EditorGUI.BeginChangeCheck();
			scrollPos_Editor = GUILayout.BeginScrollView(scrollPos_Editor);				
				GUILayout.BeginVertical();			
					infoGroupCache.groupTag = EditorGUILayout.TextField("Group Tag", infoGroupCache.groupTag);
					GUILayout.Space(48);
					for (int i = 0; i < infoGroupCache.entries.Count; i++)
					{
						switch (infoGroupCache.entries[i].entryType){
							case InfoLogEntry.EntryType.Article:
								DrawArticleInfo(infoGroupCache.entries[i]);
								break;
							case InfoLogEntry.EntryType.Gallery:
								DrawGalleryInfo(infoGroupCache.entries[i]);
								break;
							case InfoLogEntry.EntryType.Comment:
								DrawCommentInfo(infoGroupCache.entries[i]);
								break;
						}
						GUILayout.Space(24);
						GUILayout.BeginHorizontal();
							GUILayout.FlexibleSpace();
							GUILayout.Label("", GUILayout.Height(1), GUILayout.Width(420));
							CustomEditorUtility.DrawRectOutline(GUILayoutUtility.GetLastRect());
							GUILayout.FlexibleSpace();					
						GUILayout.EndHorizontal();
						GUILayout.Space(24);
					}
				GUILayout.EndVertical();
			GUILayout.EndScrollView();
			if (EditorGUI.EndChangeCheck()) {
				OnDataValueChanged();
				CacheGroupTags();
			}
		}

		void DrawEditor_NotEditing(){
			TextAnchor temp = GUI.skin.label.alignment;
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
			GUILayout.Label("Select a group to start editing.", GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
			GUI.skin.label.alignment = temp;
		}

		void DrawArticleInfo(InfoLogEntry entry){
			GUILayout.BeginVertical();
				entry.entryType = (InfoLogEntry.EntryType) EditorGUILayout.EnumPopup("Info Type", entry.entryType);
				entry.briefDescription = EditorGUILayout.TextField("Brief", entry.briefDescription);
				GUILayout.Label("Detail");
				entry.contentDescription = EditorGUILayout.TextArea(entry.contentDescription, GUILayout.MinHeight(60));
				DrawAttachedImages(entry);				
			GUILayout.EndVertical();
		}
		void DrawCommentInfo(InfoLogEntry entry){
			GUILayout.BeginVertical();
				entry.entryType = (InfoLogEntry.EntryType) EditorGUILayout.EnumPopup("Info Type", entry.entryType);
				entry.briefDescription = EditorGUILayout.TextField("Brief", entry.briefDescription);
				GUILayout.Label("Detail");
				entry.contentDescription = EditorGUILayout.TextArea(entry.contentDescription, GUILayout.MinHeight(60));
				DrawAttachedImages(entry);				
			GUILayout.EndVertical();
		}

		void DrawGalleryInfo(InfoLogEntry entry){
			entry.entryType = (InfoLogEntry.EntryType) EditorGUILayout.EnumPopup("Info Type", entry.entryType);
			entry.briefDescription = EditorGUILayout.TextField("Brief", entry.briefDescription);
			DrawAttachedImages(entry);
		}

		Sprite imagePickerBuffer;
		void DrawAttachedImages(InfoLogEntry entry){
			GUILayout.Label("Attached Images");

			GUILayout.BeginHorizontal(GUILayout.Height(160));
				GUILayout.Space(8);
				for (int i = 0; i < entry.attachedImages.Count; i++)
				{
					Rect temp;
					GUILayout.BeginVertical(GUILayout.Width(160));
						GUILayout.Space(4);
						GUILayout.Box(GUIContent.none, GUILayout.Width(160), GUILayout.Height(160));
						temp = GUILayoutUtility.GetLastRect();
						GUI.DrawTextureWithTexCoords (temp, BoxBG, new Rect (0, 0, 12, 12));
						CustomEditorUtility.DrawRectOutline(temp);
						GUI.DrawTexture(CustomEditorUtility.RectScaler(temp, 0.96f), entry.attachedImages[i].texture, ScaleMode.ScaleToFit);
						entry.attachedImages[i] = EditorGUILayout.ObjectField(entry.attachedImages[i], typeof(Sprite), false) as Sprite;
						entry.imageCaptions[i] = EditorGUILayout.TextField(entry.imageCaptions[i]);							
					GUILayout.EndVertical();
					DeleteButtonOnAttachment(temp, entry, i);
				}
				GUILayout.BeginVertical(GUILayout.Height(160));
					GUILayout.FlexibleSpace();
					GUILayout.BeginHorizontal();
						GUILayout.FlexibleSpace();
						Layout_ImagePickerButton("+", ref imagePickerBuffer, GUIUtility.GetControlID(FocusType.Passive), GUILayout.Width(32), GUILayout.Height(32));
						GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();
					GUILayout.FlexibleSpace();
				GUILayout.EndVertical();
				GUILayout.Space(8);
			GUILayout.EndHorizontal();

			CustomEditorUtility.DrawRectOutline(GUILayoutUtility.GetLastRect());

	

			if (imagePickerBuffer != null)
			{
				entry.attachedImages.Add(imagePickerBuffer);
				entry.imageCaptions.Add("");
				imagePickerBuffer = null;
			}
		
		}

		bool Layout_ImagePickerButton(string buttonText, ref Sprite target, int pickerID, params GUILayoutOption[] layoutOptions){
			GUI.SetNextControlName("BackToHere");
			if (GUILayout.Button(buttonText, layoutOptions))
				EditorGUIUtility.ShowObjectPicker<Sprite>(target, false, "", pickerID);

			if (Event.current.commandName == "ObjectSelectorUpdated")
			{
				if (EditorGUIUtility.GetObjectPickerControlID() == pickerID){ 
					target = (Sprite) EditorGUIUtility.GetObjectPickerObject();
					GUI.FocusControl("BackToHere");
					return true;
				}
			}
			return false;
		}

		void DeleteAttachment(InfoLogEntry entry, int index){
			entry.attachedImages.RemoveAt(index);
			entry.imageCaptions.RemoveAt(index);
		}

		void DeleteButtonOnAttachment(Rect targetRect, InfoLogEntry entry, int index){
			if (GUI.Button(new Rect (targetRect.xMax - 20, targetRect.yMin + 4, 16, 16), "X"))
				DeleteAttachment(entry, index);
		}


	#endregion

#endregion

#region Event Handlers

		void OnAddNewGroupButtonClicked(){			
			InfoLogGroup newGroup = InfoLogGroup.Create("New Group");
			AddSubAsset(newGroup, databaseCache);
			databaseCache.entryGroupList.Add(newGroup);
			CacheGroupTags();

			selectedGroupIndex = databaseCache.entryGroupList.Count - 1;		
			LoadInfoGroup(newGroup);

			DrawSideBar_Group = DrawSideBar_Groups_Normal;
			DrawEditor =DrawEditor_Normal;

			SaveDatabase();
		}

		void OnDeleteSelectedButtonClicked(){
			databaseCache.entryGroupList.Remove(infoGroupCache);
			DestroyImmediate(infoGroupCache, true);

			CacheGroupTags();

			if (databaseCache.entryGroupList.Count > 0)
			{
				OnSidebarSelectionChanged();
			}

			SaveDatabase();
		}

		void OnAddNewEntryButtonClicked(){
			InfoLogEntry newEntry = InfoLogEntry.Create();
			AddSubAsset(newEntry, infoGroupCache);
			infoGroupCache.entries.Add(newEntry);
			CacheEntryTags();
			
			DrawSideBar_Entry = DrawSideBar_Entries_Normal;
			DrawEditor = DrawEditor_Normal;

			SaveDatabase();
		}

		//Load the targeted group if it actually exist.
		void OnSidebarSelectionChanged(){
			if (databaseCache.entryGroupList[selectedGroupIndex] != null)
			{
				LoadInfoGroup(databaseCache.entryGroupList[selectedGroupIndex]);
				selectedGroupIndexCache = selectedGroupIndex;
				if (infoGroupCache != null) DrawEditor = DrawEditor_Normal;
			}
			else Debug.LogWarning("InfoLogEditor:: entryGroupList[selectedGroupIndex] does not exist.");
			GUI.FocusControl("0");
		}

		void OnDataValueChanged(){
			lastChangeTime = EditorApplication.timeSinceStartup;
			EditorApplication.update -= SaveTimer;
			EditorApplication.update += SaveTimer;
		}



#endregion

#region Asset Handling

		static string databaseDir = "Assets/DataEditors/InfoLogEditor/Resources/";
		static string databaseName = "InfoLogDatabase";

		static InfoLogDatabase databaseCache;
		static InfoLogGroup infoGroupCache;

		static void SaveDatabase(){
			EditorUtility.SetDirty(databaseCache);
			AssetDatabase.SaveAssets();
		}

		//Load database from resources or create a new one if fail to found one.
		static void LoadDatabase(){			
			databaseCache = (InfoLogDatabase) Resources.Load(databaseName, typeof(InfoLogDatabase));
			//
			if (!databaseCache) {
				Directory.CreateDirectory(databaseDir);
				databaseCache = InfoLogDatabase.Create();
				AssetDatabase.CreateAsset(databaseCache, databaseDir + databaseName + ".asset");
				SaveDatabase();
			}
			
		}  

		///Load entry content into Editor block, does not thing to do with (int) selected.
		void LoadInfoGroup(InfoLogGroup group){
			infoGroupCache = group;
			_listAdaptor = new GenericListAdaptor<InfoLogEntry>(infoGroupCache.entries, DrawListEntry, 16f);
			DrawSideBar_Entry = DrawSideBar_Entries_Normal;
		}

		public static void AddSubAsset(ScriptableObject subAsset, ScriptableObject mainAsset)
		{
			if (subAsset != null && mainAsset != null)
			{
				AssetDatabase.AddObjectToAsset(subAsset, mainAsset);
				subAsset.hideFlags = HideFlags.HideInHierarchy;
			}
		}

		static double lastChangeTime;
		static void SaveTimer(){
			double temp = EditorApplication.timeSinceStartup - lastChangeTime;
			if (temp > 2){
				SaveDatabase();
				EditorApplication.update -= SaveTimer;
			}
		}
#endregion

 	}
}