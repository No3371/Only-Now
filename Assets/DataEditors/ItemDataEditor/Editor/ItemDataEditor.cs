using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using ItemDataEditor.Data;

	namespace ItemDataEditor.Editor{
	public class ItemDataEditor : EditorWindow, IHasCustomMenu {

		//When to save database:
		//	- When the editor is closed
		//	- When user select another entry
		//	- 2 seconds after user change a value of a entry

		//When to filter:
		//	- When filter field is changed, and the new value is not same or ""

		private static ItemDataEditor _editor;
		public static ItemDataEditor editor { get { AssureEditor(); return _editor; } }
		public static void AssureEditor() { if (_editor == null) OpenNodeEditor(); }

		public static string databaseDir = "Assets/DataEditors/ItemDataEditor/Resources/";
		public static string databaseName = "ItemDatabase";

		public static ItemDatabase itemDatabaseCache;
		static ItemDataEntry entryCache;

		static GUISkin itemEditorSkin;

		static bool itemDataEditorDebugging = false;

		static void SaveDatabase(){
			EditorUtility.SetDirty(itemDatabaseCache);
			AssetDatabase.SaveAssets();
			if (itemDataEditorDebugging) Debug.Log("ItemDataEditor:: SaveDatabase():: Database saved.");
		}


		static void LoadDatabase(){
			
			if (itemDataEditorDebugging) Debug.Log("ItemDataEditor:: LoadDatabase():: Start.");
			
			itemDatabaseCache = (ItemDatabase) Resources.Load(databaseName, typeof(ItemDatabase));
			if (!itemDatabaseCache) {
				Directory.CreateDirectory(databaseDir);
				itemDatabaseCache = ItemDatabase.Create();
				AssetDatabase.CreateAsset(itemDatabaseCache, databaseDir + databaseName + ".asset");
				SaveDatabase();
				if (itemDataEditorDebugging) Debug.Log("ItemDataEditor:: LoadDatabase():: Created a new database asset because no database asset found at designated path.");
			}
			
			if (itemDataEditorDebugging) Debug.Log("ItemDataEditor:: LoadDatabase():: Done.");
		}  

		///Load entry content into Editor block, does not thing to do with (int) selected.
		static void LoadEntry(ItemDataEntry entry){
			if (itemDataEditorDebugging) Debug.Log("ItemDataEditor:: LoadEntry():: Loading " + entry.itemID);
			entryCache = entry;
		}

		public static void AddSubAsset(ScriptableObject subAsset, ScriptableObject mainAsset)
		{
			if (subAsset != null && mainAsset != null)
			{
				AssetDatabase.AddObjectToAsset(subAsset, mainAsset);
				subAsset.hideFlags = HideFlags.HideInHierarchy;
			}
		}

		static void DoBackup(){
			itemDatabaseCache.lastBackupTime = System.DateTime.Now.Ticks;
			if (File.Exists(databaseDir + databaseName + ".asset.backup")) AssetDatabase.DeleteAsset(databaseDir + databaseName + ".asset.backup");
			if (File.Exists(databaseDir + databaseName + ".asset")) AssetDatabase.CopyAsset(databaseDir + databaseName + ".asset", databaseDir + databaseName + ".asset.backup");
		}

		const int sidebarWidth = 200, viewInset = 4;

		Rect thumbnailRect { get { return new Rect(sidebarWidth + viewInset + 10, viewInset + 24, 300, 100); }}
		Rect imageRect { get { return new Rect(sidebarWidth + viewInset + 10, viewInset + 144, 300, 300); }}
		Rect SidebarRect { get { return new Rect(viewInset, viewInset, sidebarWidth, position.height - viewInset*2); }}
		Rect EditorRect { get { return new Rect (sidebarWidth + viewInset + 10, viewInset, position.width - sidebarWidth - viewInset*2, position.height - viewInset*2); }}

		Vector2 scrollpos;

		static Texture2D defaultImg, workingImg;

		static void CheckInit(){			
			if (!defaultImg) defaultImg = Resources.Load("DefaultTex", typeof(Texture2D)) as Texture2D;
			if (!workingImg) workingImg = Resources.Load("Hardworking", typeof(Texture2D)) as Texture2D;
			if (!itemDatabaseCache){
				if (itemDataEditorDebugging) Debug.Log("ItemDataEditor:: CheckInit():: itemDatabaseCache is null.");
				LoadDatabase();
				filteredList = itemDatabaseCache.ItemList;
			}			
			if (itemNamesCache == null) {
				if (itemDataEditorDebugging) Debug.Log("ItemDataEditor:: CheckInit():: itemNamesCache is null.");
				CacheItemNames();
				filteredItemNamesCache = new List<string>(itemNamesCache);
			}
			if (!itemEditorSkin) itemEditorSkin = Resources.Load<GUISkin>("ItemEditor");

			if (dDrawSidebarContent == null) {
				if (itemDataEditorDebugging) Debug.Log("ItemDataEditor:: CheckInit():: Assigning dDrawSidebarContent to DrawSelections.");
				dDrawSidebarContent = Sidebar_DrawSelections;
			}
			if (dDrawEditorContent == null) {
				if (itemDataEditorDebugging) Debug.Log("ItemDataEditor:: CheckInit():: Assigning dDrawEditorContent to DrawEditor.");
				dDrawEditorContent = EditorContent_DrawEditor;
			}

			if (selected == -1){
				if (itemDataEditorDebugging) Debug.Log("ItemDataEditor:: CheckInit():: selected == -1, set to 0.");
				selected = 0;
			}

			if (entryCache == null) {
				if (itemDataEditorDebugging) Debug.Log("ItemDataEditor:: CheckInit():: entryCache is null. Trying to load filteredList[" + selected + "].");
				if (filteredList.Count > 0) {
					LoadEntry(filteredList[selected]);
					dDrawEditorContent = EditorContent_DrawEditor;
				}
				else {
					if (itemDataEditorDebugging) Debug.Log("ItemDataEditor:: CheckInit():: filteredList is empty.");	
					dDrawEditorContent = EditorContent_NoSelection;
				}
			}
			
		}

        public void AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Export To Json File"), false, new GenericMenu.MenuFunction(OnExportButtonClicked));
            menu.AddItem(new GUIContent("Import From Json File"), false, new GenericMenu.MenuFunction(OnImportButtonClicked));
        }


		[MenuItem ("Window/Data Editors/Item Data")]
		public static ItemDataEditor OpenNodeEditor () 
		{
			_editor = GetWindow<ItemDataEditor>();
			_editor.minSize = new Vector2(800, 600);
			_editor.maxSize = new Vector2(1280, 720);

			_editor.titleContent = new GUIContent ("Item Editor");

			if (itemDataEditorDebugging) Debug.Log("ItemDataEditor:: OpenNodeEditor():: Done.");
			return _editor;
		}

		/// <summary>
		/// OnGUI is called for rendering and handling GUI events.
		/// This function can be called multiple times per frame (one call per event).
		/// </summary>
		void OnGUI()
		{
			CheckInit();

			GUILayout.BeginHorizontal();
				GUILayout.Space(4);
				DrawSidebar();
				
				GUILayout.Space(10);

				DrawEditor();
				GUILayout.Space(20);
			GUILayout.EndHorizontal();

			if (selected != selectedOld) OnSidebarSelectionChanged();

			if (System.DateTime.Now.Ticks - itemDatabaseCache.lastBackupTime > 6000000000) DoBackup();
		}

#region EditorDrawing

		delegate void DrawEditorContent();
		static DrawEditorContent dDrawEditorContent;

		static void EditorContent_Filtering(){
			GUILayout.BeginVertical(GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
				ToggleBoxSkin(true);
				GUILayout.Box(workingImg, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
				ToggleBoxSkin(false);
			GUILayout.EndVertical();
		}

		static void EditorContent_NoSelection(){
			GUILayout.BeginVertical(GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
				ToggleBoxSkin(true);
				GUILayout.Box(workingImg, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
				ToggleBoxSkin(false);
			GUILayout.EndVertical();
		}

		static void EditorContent_DrawEditor(){
			editorScrollPos = GUILayout.BeginScrollView(editorScrollPos);
				GUILayout.BeginVertical();
					GUILayout.Space(4);
					GUILayout.Label("Content");
					GUILayout.BeginHorizontal(GUILayout.Height(140));
						GUILayout.BeginVertical(GUILayout.Width(120));
							GUILayout.FlexibleSpace();
							ToggleBoxSkin(true);
							if (!entryCache.itemThumbnail) GUILayout.Box(defaultImg, GUILayout.Height(100), GUILayout.Width(100));
							else GUILayout.Box(entryCache.itemThumbnail.texture, GUILayout.Height(100), GUILayout.Width(100));
							ToggleBoxSkin(false);
							DrawImageEditButton(GUILayoutUtility.GetLastRect(), ref entryCache.itemThumbnail, 
				EditorGUIUtility.GetControlID(FocusType.Passive));
							GUILayout.FlexibleSpace();
						GUILayout.EndVertical();
						GUILayout.BeginVertical();
							EditorGUI.BeginChangeCheck();
							entryCache.itemID =  EditorGUILayout.TextField("Item ID", entryCache.itemID);
							GUILayout.Label("Item Description");
							entryCache.itemBasicDescription =  EditorGUILayout.TextArea(entryCache.itemBasicDescription, GUILayout.Height(100));
							if (EditorGUI.EndChangeCheck()) OnEntryValueChanged();
						GUILayout.EndVertical();
					GUILayout.EndHorizontal();
					ToggleBoxSkin(true);
					if (!entryCache.itemImage) GUILayout.Box(defaultImg, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
					else GUILayout.Box(entryCache.itemImage.texture, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));	
					ToggleBoxSkin(false);
					DrawImageEditButton(GUILayoutUtility.GetLastRect(), ref entryCache.itemImage, EditorGUIUtility.GetControlID(FocusType.Passive));
				GUILayout.EndVertical();		
			GUILayout.EndScrollView();

		}

		static Vector2 editorScrollPos;
		void DrawEditor(){
			dDrawEditorContent.Invoke();
		}
		
		static GUISkin defaultSkin;
		
		static void ToggleBoxSkin(bool toggle){
			if (toggle){
				defaultSkin = GUI.skin;
				GUI.skin = itemEditorSkin;
			} 
			else{
				GUI.skin = defaultSkin;
			}
		}

#endregion


		#region Sidebar

		static List<ItemDataEntry> filteredList;
		static string filter = "", activeFilter = ""; 
		static int selected = -1, selectedOld;	
		static List<string> itemNamesCache, filteredItemNamesCache; 

		delegate void DrawSidebarContent();
		static DrawSidebarContent dDrawSidebarContent;

		//Worker method compare filter string to id of every items in the item list of the database. 
		static void FilterWorker(string idFilter){
			OnFilteringBegin();
			if (idFilter == "") filteredList = new List<ItemDataEntry>(itemDatabaseCache.ItemList);
			else filteredList = new List<ItemDataEntry>();
			filteredItemNamesCache.Clear();
			if (itemDataEditorDebugging) Debug.Log("ItemDataEditor:: FilterWorker():: filteredItemNamesCache cleared.");
			for (int i = 0; i < itemDatabaseCache.ItemList.Count; i++){
				if (itemDatabaseCache.ItemList[i].itemID.Contains(idFilter)){
					filteredList.Add(itemDatabaseCache.ItemList[i]);
					filteredItemNamesCache.Add(itemDatabaseCache.ItemList[i].itemID);
				}
			}
			activeFilter = idFilter;
			OnFilteringDone();
		}

		static void Sidebar_ShowFiltering(){
			GUILayout.BeginVertical(GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
				GUILayout.Label("Filtering...");
			GUILayout.EndVertical();
		}

		static void Sidebar_ShowNoResult(){
			GUILayout.BeginVertical(GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
				GUILayout.FlexibleSpace();
					GUILayout.Label("No result");
				GUILayout.FlexibleSpace();
			GUILayout.EndVertical();
		}

		static void Sidebar_ShowDatabaseEmpty(){
			GUILayout.BeginVertical(GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
				GUILayout.FlexibleSpace();
					GUILayout.Label("Empty");
				GUILayout.FlexibleSpace();
			GUILayout.EndVertical();
		}
		
		static void Sidebar_DrawSelections(){
			selected = GUILayout.SelectionGrid(selected, filteredItemNamesCache.ToArray(), 1); 
		}

		void DrawSidebar(){

			GUILayout.BeginVertical(GUILayout.MaxWidth(200f));
				GUILayout.Space(4);

				GUILayout.Label("Filter");
				EditorGUI.BeginChangeCheck();
				filter = EditorGUILayout.TextField(filter);
				if (EditorGUI.EndChangeCheck()){ //if filter field is changed
					OnFilterFieldChanged();
				}

				scrollpos = GUILayout.BeginScrollView(scrollpos);		
					dDrawSidebarContent.Invoke();
				GUILayout.EndScrollView();

				GUILayout.Space(6);

				GUILayout.BeginHorizontal();
					if (GUILayout.Button("Add New")) OnAddNewButtonClicked();
					if (GUILayout.Button("Delete Selected")) OnDeleteSelectedButtonClicked();
				GUILayout.EndHorizontal();
				GUILayout.Space(4);
			GUILayout.EndVertical();
		}

		#endregion

		static void DrawImageEditButton(Rect imageRect, ref Sprite target, int pickerID){
			int buttonW = 24, buttonH = 16, margin = 4;
			Rect buttonRect = new Rect(imageRect.left + margin, imageRect.top + margin, buttonW, buttonH);
			GUI.SetNextControlName("BackToHere");
			if (GUI.Button(buttonRect, "..."))
				EditorGUIUtility.ShowObjectPicker<Sprite>(target, false, "", pickerID);

			if (Event.current.commandName == "ObjectSelectorUpdated")
			{
				if (EditorGUIUtility.GetObjectPickerControlID() == pickerID){ 
					target = (Sprite) EditorGUIUtility.GetObjectPickerObject();
					GUI.FocusControl("BackToHere");
					OnEntryValueChanged();
				}
			}
		}

		static void CacheItemNames(){
			if (itemDataEditorDebugging) Debug.Log("ItemDataEditor:: CacheItemNames():: Caching.");
			itemNamesCache = new List<string>();
			for (int i = 0; i < itemDatabaseCache.ItemList.Count; i++){
				itemNamesCache.Add(itemDatabaseCache.ItemList[i].itemID);
			}
		}

#region EventHandler?s
		
		static void OnFilterFieldChanged(){
			if (filter == ""){
				filteredList = new List<ItemDataEntry>(itemDatabaseCache.ItemList);
				filteredItemNamesCache = new List<string>(itemNamesCache);
				OnFilteringDone();
			}
			//If user remove filter string when filter is activated...
			if (filter != activeFilter && filter == ""){
				filteredList = new List<ItemDataEntry>(itemDatabaseCache.ItemList);
				filteredItemNamesCache = new List<string>(itemNamesCache);
				selected = 0;
				activeFilter = filter;
				if (itemDataEditorDebugging) Debug.Log("ItemDataEditor:: OnFilterFieldChanged():: filter is changed and becomes empty. Set filteredItemNamesCache to a cpoy of itemNamesCache, filteredList to a copy of ItemDatabase.cs.ItemList.");
				if (itemDataEditorDebugging) Debug.Log("ItemDataEditor:: OnFilterFieldChanged():: selected is set to 0.");
				OnFilteringDone();

			}
			//If user enter new filter value...
			else if (activeFilter != filter){
				FilterWorker(filter);
				selected = 0;
				if (itemDataEditorDebugging) Debug.Log("ItemDataEditor:: OnFilterFieldChanged():: Detected new filter, selected is set to 0.");
			}
			else if (activeFilter == filter) {

			}
		}

		static void OnAddNewButtonClicked(){

			ItemDataEntry newEntry = ItemDataEntry.Create("New Item");
			AddSubAsset(newEntry, itemDatabaseCache);
			itemDatabaseCache.ItemList.Add(newEntry);
			if (itemDataEditorDebugging) Debug.Log("ItemDataEditor:: OnAddNewButtonClicked():: New entry added to itemDatabaseCache.ItemList.");
			CacheItemNames();

			ForceChangeFilterValue("");
			//itemNamesCache.Add("New Item");
			//if (itemDataEditorDebugging) Debug.Log("ItemDataEditor:: OnAddNewButtonClicked():: \"New Item\" added to itemNamesCache to be optimistic.");
			//filteredItemNamesCache = new List<string>(itemNamesCache);

			selected = itemDatabaseCache.ItemList.Count - 1;		
			LoadEntry(itemDatabaseCache.ItemList[selected]);

			dDrawEditorContent = EditorContent_DrawEditor;
			dDrawSidebarContent = Sidebar_DrawSelections;

			SaveDatabase();
		}

		static void OnDeleteSelectedButtonClicked(){
			if (itemDataEditorDebugging) Debug.Log("ItemDataEditor:: OnDeleteSelectedButtonClicked():: Currently selected index: " + selected);

			//Optimistic change on UI
			filteredList.Remove(entryCache);
			if (itemDataEditorDebugging) Debug.Log("ItemDataEditor:: OnDeleteSelectedButtonClicked():: Removing itemID cache in filteredItemNames.");
			filteredItemNamesCache.RemoveAt(selected);

			//Actually remove reference and destroy
			if (itemDataEditorDebugging) Debug.Log("ItemDataEditor:: OnDeleteSelectedButtonClicked():: Removing reference of deleted entry in itemDatabase and filterList.");
			itemDatabaseCache.ItemList.Remove(entryCache);
			DestroyImmediate(entryCache, true);
			if (itemDataEditorDebugging) Debug.Log("ItemDataEditor:: OnDeleteSelectedButtonClicked():: Designated entry destroyed.");

			CacheItemNames();

			if (filteredList.Count > 0) {
				if (itemDataEditorDebugging) Debug.Log("ItemDataEditor:: OnDeleteSelectedButtonClicked():: Loading filteredList[" + (selected - 1) + "]");
				if (selected >= filteredList.Count) selected -= 1;
				LoadEntry(filteredList[selected]);
			}
			else {
				if (activeFilter == "") {
					dDrawSidebarContent = Sidebar_ShowDatabaseEmpty;
					dDrawEditorContent = EditorContent_NoSelection;
				} 
				else {
					if (itemDataEditorDebugging) Debug.Log("ItemDataEditor:: OnDeleteSelectedButtonClicked():: No entry left in filteredList, clearing filter.");
					ForceChangeFilterValue("");
				}
			}

			SaveDatabase();
		}

		static double lastChangeTime;
		static void OnEntryValueChanged(){
			lastChangeTime = EditorApplication.timeSinceStartup;
			EditorApplication.update -= SaveTimer;
			EditorApplication.update += SaveTimer;
			filteredItemNamesCache[selected] = entryCache.itemID;
		}

		void OnDestroy(){
			SaveDatabase();
		}

		static void OnFilteringBegin(){
			dDrawSidebarContent = Sidebar_ShowFiltering;
			if (itemDataEditorDebugging) Debug.Log("ItemDataEditor:: OnFilteringDone():: Set dDrawSidebarContent to ShowFiltering.");

			dDrawEditorContent = EditorContent_Filtering;
			if (itemDataEditorDebugging) Debug.Log("ItemDataEditor:: OnFilteringDone():: Set dDrawEditorContent to Filtering.");
		}

		static void OnFilteringDone(){
			if (filteredList.Count == 0){
				dDrawSidebarContent = Sidebar_ShowNoResult;
				if (itemDataEditorDebugging) Debug.Log("ItemDataEditor:: OnFilteringDone():: Set dDrawSidebarContent to ShowNoResult.");

				dDrawEditorContent = EditorContent_NoSelection;
				if (itemDataEditorDebugging) Debug.Log("ItemDataEditor:: OnFilteringDone():: Set dDrawEditorContent to NoSelection.");
			}
			else {
				dDrawSidebarContent = Sidebar_DrawSelections;
				if (itemDataEditorDebugging) Debug.Log("ItemDataEditor:: OnFilteringDone():: Set dDrawSidebarContent to DrawSelections.");

				dDrawEditorContent = EditorContent_DrawEditor;
				if (itemDataEditorDebugging) Debug.Log("ItemDataEditor:: OnFilteringDone():: Set dDrawEditorContent to DrawEditor.");
			}
		}

		static void OnSidebarSelectionChanged(){
			if (itemDataEditorDebugging) Debug.Log("ItemDataEditor:: OnSidebarSelectionChanged():: Current selectd == " + selected);			

			if (filteredList.Count > 0 && selected < filteredList.Count){
				GUIUtility.keyboardControl = 0;
				LoadEntry(filteredList[selected]);
				selectedOld = selected;
			}
			else{
				if (itemDataEditorDebugging) Debug.Log("ItemDataEditor:: OnSidebarSelectionChanged():: filteredList is empty. LoadEntry Cancelled.");
			}
		}


		static void OnExportButtonClicked(){
			File.WriteAllText(EditorUtility.SaveFilePanelInProject("Save as JSON file", "export.json", "json", "Select where to save"), itemDatabaseCache.ExportToJsonArray());
			AssetDatabase.Refresh();
		}

		static void OnImportButtonClicked(){
			string path = EditorUtility.OpenFilePanelWithFilters( "Open JSON file", Application.dataPath, new string[]{ "Json File", "json"});
			if (string.IsNullOrEmpty(path)) return;
			string temp = File.ReadAllText(path);
			Debug.Log(temp);
			itemDatabaseCache.CreateFromJsonArray( itemDatabaseCache.ParseJsonArray(temp));
			CacheItemNames();
			ForceChangeFilterValue("");

		}

#endregion

		static void SaveTimer(){
			double temp = EditorApplication.timeSinceStartup - lastChangeTime;
			if (temp > 2){
				if (itemDataEditorDebugging) Debug.Log("Delayed saving done after: " + temp);
				SaveDatabase();
				EditorApplication.update -= SaveTimer;
			}
		}

		static public void ForceChangeFilterValue(string filterValue){
			filter = filterValue;
			OnFilterFieldChanged();
		}

	}
}