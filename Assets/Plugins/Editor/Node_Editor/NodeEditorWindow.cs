using System;
using System.IO;
using System.Linq;

using UnityEngine;
using UnityEditor;

using NodeEditorFramework;
using NodeEditorFramework.Utilities;

namespace NodeEditorFramework.Standard
{
	public class NodeEditorWindow : EditorWindow 
	{
		// Information about current instance
		private static NodeEditorWindow _editor;
		public static NodeEditorWindow editor { get { AssureEditor(); return _editor; } }
		public static void AssureEditor() { if (_editor == null) OpenNodeEditor(); }

		// Opened Canvas
		public static NodeEditorUserCache canvasCache;

		// GUI
		private string sceneCanvasName = "";
		private Rect loadSceneUIPos;
		private Rect createCanvasUIPos;
		private int sideWindowWidth = 400;

		public Rect sideWindowRect { get { return new Rect (position.width - sideWindowWidth, 0, sideWindowWidth, position.height); } }
		public Rect canvasWindowRect { get { return new Rect (0, 0, position.width - sideWindowWidth, position.height); } }

		#region General 

		/// <summary>
		/// Opens the Node Editor window and loads the last session
		/// </summary>
		[MenuItem("Window/Node Editor")]
		public static NodeEditorWindow OpenNodeEditor () 
		{
			_editor = GetWindow<NodeEditorWindow>();
			_editor.minSize = new Vector2(800, 600);
			NodeEditor.ReInit (false);

			Texture iconTexture = ResourceManager.LoadTexture (EditorGUIUtility.isProSkin? "Textures/Icon_Dark.png" : "Textures/Icon_Light.png");
			_editor.titleContent = new GUIContent ("Node Editor", iconTexture);

			return _editor;
		}
		
		[UnityEditor.Callbacks.OnOpenAsset(1)]
		private static bool AutoOpenCanvas(int instanceID, int line)
		{
			if (Selection.activeObject != null && Selection.activeObject is NodeCanvas)
			{
				string NodeCanvasPath = AssetDatabase.GetAssetPath(instanceID);
				NodeEditorWindow.OpenNodeEditor();
				canvasCache.LoadNodeCanvas(NodeCanvasPath);
				return true;
			}
			return false;
		}

		private void OnEnable()
		{            
			_editor = this;
			NodeEditor.checkInit(false);

			NodeEditor.ClientRepaints -= Repaint;
			NodeEditor.ClientRepaints += Repaint;

			EditorLoadingControl.justLeftPlayMode -= NormalReInit;
			EditorLoadingControl.justLeftPlayMode += NormalReInit;
			// Here, both justLeftPlayMode and justOpenedNewScene have to act because of timing
			EditorLoadingControl.justOpenedNewScene -= NormalReInit;
			EditorLoadingControl.justOpenedNewScene += NormalReInit;

			SceneView.onSceneGUIDelegate -= OnSceneGUI;
			SceneView.onSceneGUIDelegate += OnSceneGUI;

			// Setup Cache
			canvasCache = new NodeEditorUserCache(Path.GetDirectoryName(AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject (this))));
			canvasCache.SetupCacheEvents();
		}

	    private void NormalReInit()
		{
			NodeEditor.ReInit(false);
		}

		private void OnDestroy()
		{
			EditorUtility.SetDirty(canvasCache.nodeCanvas);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

			NodeEditor.ClientRepaints -= Repaint;

			EditorLoadingControl.justLeftPlayMode -= NormalReInit;
			EditorLoadingControl.justOpenedNewScene -= NormalReInit;

			SceneView.onSceneGUIDelegate -= OnSceneGUI;

			// Clear Cache
			canvasCache.ClearCacheEvents ();
		}

        #endregion

        #region GUI

        private void OnSceneGUI(SceneView sceneview)
        {
            DrawSceneGUI();
        }

	    private void DrawSceneGUI()
	    {
			if (canvasCache.editorState.selectedNode != null)
				canvasCache.editorState.selectedNode.OnSceneGUI();
            SceneView.lastActiveSceneView.Repaint();
        }

        private void OnGUI()
		{            
			// Initiation
			NodeEditor.checkInit(true);
			if (NodeEditor.InitiationError)
			{
				GUILayout.Label("Node Editor Initiation failed! Check console for more information!");
				return;
			}
			AssureEditor ();
			canvasCache.AssureCanvas ();

			// Specify the Canvas rect in the EditorState
			canvasCache.editorState.canvasRect = canvasWindowRect;
			// If you want to use GetRect:
//			Rect canvasRect = GUILayoutUtility.GetRect (600, 600);
//			if (Event.current.type != EventType.Layout)
//				mainEditorState.canvasRect = canvasRect;
			NodeEditorGUI.StartNodeGUI ();

			// Perform drawing with error-handling
			try
			{
				NodeEditor.DrawCanvas (canvasCache.nodeCanvas, canvasCache.editorState);
			}
			catch (UnityException e)
			{ // on exceptions in drawing flush the canvas to avoid locking the ui.
				canvasCache.NewNodeCanvas ();
				NodeEditor.ReInit (true);
				Debug.LogError ("Unloaded Canvas due to an exception during the drawing phase!");
				Debug.LogException (e);
			}

			// Draw Side Window
			sideWindowWidth = Math.Min(600, Math.Max(200, (int)(position.width / 5)));
			GUILayout.BeginArea(sideWindowRect, GUI.skin.box);
			DrawSideWindow();
			GUILayout.EndArea();

			NodeEditorGUI.EndNodeGUI();
		}

		private void DrawSideWindow()
		{
			GUILayout.Label (new GUIContent ("Node Editor (" + canvasCache.nodeCanvas.name + ")", "Opened Canvas path: " + canvasCache.openedCanvasPath), NodeEditorGUI.nodeLabelBold);

//			EditorGUILayout.ObjectField ("Loaded Canvas", canvasCache.nodeCanvas, typeof(NodeCanvas), false);
//			EditorGUILayout.ObjectField ("Loaded State", canvasCache.editorState, typeof(NodeEditorState), false);

			if (GUILayout.Button(new GUIContent("New Canvas", "Loads an Specified Empty CanvasType")))
			{
				NodeEditorFramework.Utilities.GenericMenu menu = new NodeEditorFramework.Utilities.GenericMenu();
				NodeCanvasManager.FillCanvasTypeMenu(ref menu, canvasCache.NewNodeCanvas);
				menu.Show(createCanvasUIPos.position, createCanvasUIPos.width);
			}
			if (Event.current.type == EventType.Repaint)
			{
				Rect popupPos = GUILayoutUtility.GetLastRect();
				createCanvasUIPos = new Rect(popupPos.x + 2, popupPos.yMax + 2, popupPos.width - 4, 0);
			}

			GUILayout.Space(6);

			if (GUILayout.Button(new GUIContent("Save Canvas", "Saves the Canvas to a Canvas Save File in the Assets Folder")))
            {
                string saving = "Canvas";
                if (canvasCache.nodeCanvas.nodes.Find(n => n.GetID == "dialogStartNode") != null)
                {
                    //Auto canvas name
                    saving = (canvasCache.nodeCanvas.nodes.Find(n => n.GetID == "dialogStartNode") as DialogNodeEditor.DialogStartNode).dialogName;
                }

                if (String.IsNullOrEmpty(saving)) UnityEditor.EditorUtility.DisplayDialog("Warning", "The \"Name\" field in Start Node is empty, please give this dialog tree a name then try again.", "Close");
                else {
                    string path = "Assets/Plugins/DialogSystem/Canvas/" + saving + ".canvas.asset";
                    if (!Directory.Exists("Assets/Plugins/DialogSystem/Canvas/")) Directory.CreateDirectory("Assets/Plugins/DialogSystem/Canvas/");
                    if (!string.IsNullOrEmpty(path))
                        canvasCache.SaveNodeCanvas(path);
                    ShowNotification(new GUIContent("\"" + saving + "\"(Canvas) is Saved!"));
                }
			}

			if (GUILayout.Button(new GUIContent("Load Canvas", "Loads the Canvas from a Canvas Save File in the Assets Folder")))
			{
				string path = EditorUtility.OpenFilePanel("Load Node Canvas", "Assets/Plugins/DialogSystem/Canvas/", "asset");
				if (!path.Contains(Application.dataPath))
				{
					if (!string.IsNullOrEmpty(path))
						ShowNotification(new GUIContent("You should select an asset inside your project folder!"));
				}
				else
					canvasCache.LoadNodeCanvas (path);
			}            

			GUILayout.Space (6);
            /*
			if (GUILayout.Button (new GUIContent ("Recalculate All", "Initiates complete recalculate. Usually does not need to be triggered manually.")))
				NodeEditor.RecalculateAll (canvasCache.nodeCanvas);

			if (GUILayout.Button ("Force Re-Init"))
				NodeEditor.ReInit (true);
			
			NodeEditorGUI.knobSize = EditorGUILayout.IntSlider (new GUIContent ("Handle Size", "The size of the Node Input/Output handles"), NodeEditorGUI.knobSize, 12, 20);
			canvasCache.editorState.zoom = EditorGUILayout.Slider (new GUIContent ("Zoom", "Use the Mousewheel. Seriously."), canvasCache.editorState.zoom, 0.6f, 2);
            */
            GUILayout.Space(12);

            if (GUILayout.Button(new GUIContent("Export to Converter", "Call Converter to convert this canvas to dialog tree file.")))
            {
                string saving = "Tree";
                if (canvasCache.nodeCanvas.nodes.Find(n => n.GetID == "dialogStartNode") != null)
                {
                    //Auto canvas name
                    saving = (canvasCache.nodeCanvas.nodes.Find(n => n.GetID == "dialogStartNode") as DialogNodeEditor.DialogStartNode).dialogName;
                }

                if (String.IsNullOrEmpty(saving)) UnityEditor.EditorUtility.DisplayDialog("Warning", "The \"Name\" field in Start Node is empty, please give this dialog tree a name then try again.", "Close");
                else if (DialogSystem.Data.Converter.TryDialogTree(canvasCache.nodeCanvas) != DialogSystem.Data.Converter.tryOutputCount) UnityEditor.EditorUtility.DisplayDialog("Warning", "The nodes count does not match, please check again if all outputs connected. Note: you should not left outputs empty, if it's the end of the dialog, you should add End Node to it.", "Close");
                else {
                    string path = "Assets/Plugins/DialogSystem/Resources/Trees/" + saving + ".tree.asset";
                    if (!Directory.Exists("Assets/Plugins/DialogSystem/Resources/Trees/")) Directory.CreateDirectory("Assets/Plugins/DialogSystem/Resources/Trees/");

                    bool conf = true;
                    if (File.Exists(path)) conf = EditorUtility.DisplayDialog("Warning", "There's already a dialog tree and a canvas with same name, are you sure to overwrite it?", "Yes", "Cancel");

                    if (conf)
                    {
                        if (!string.IsNullOrEmpty(path))
                        {
                            if (!Directory.Exists("Assets/Plugins/DialogSystem/Canvas/")) Directory.CreateDirectory("Assets/Plugins/DialogSystem/Canvas/");
                            canvasCache.SaveNodeCanvas("Assets/Plugins/DialogSystem/Canvas/" + saving + ".canvas.asset");
                            DialogSystem.Data.Converter.Save(path, canvasCache.nodeCanvas);
                        }

                        ShowNotification(new GUIContent("\"" + saving + "\"(Tree) is Saved!"));
                    }
                    else
                    {

                    }
                }
            }
            GUILayout.Space(6);

            if (GUILayout.Button(new GUIContent("Try", "")))
            {
                DialogSystem.Data.Converter.TryDialogTree(canvasCache.nodeCanvas);
            }

            if (canvasCache.editorState.selectedNode != null && Event.current.type != EventType.Ignore)
				canvasCache.editorState.selectedNode.DrawNodePropertyEditor();
		}

		public void LoadSceneCanvasCallback (object canvas) 
		{
			canvasCache.LoadSceneNodeCanvas ((string)canvas);
		}

		#endregion
	}
}