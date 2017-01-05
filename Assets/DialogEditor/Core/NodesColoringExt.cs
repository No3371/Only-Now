using UnityEngine;
using DialogNodeEditor;

namespace NodeEditorFramework{

	public class NodesColoringExt{

		static Color defaultColor;

		public static void ColorOn(System.Type t){
			defaultColor = GUI.backgroundColor;
			if (t.IsSubclassOf(typeof(BaseVariableNode))){
				GUI.backgroundColor = new Color(0.1f, 0.1f, 0.9f);
			}
			else if (t.IsSubclassOf(typeof(BaseConvControlNode))){
				GUI.backgroundColor = new Color(0.9f, 0.1f, 0.1f);
			}
			else if (t.IsSubclassOf(typeof(BaseSystemControlNode))){
				GUI.backgroundColor = new Color(0.1f, 0.6f, 0.1f);
			}
			else if (t.IsSubclassOf(typeof(BaseActionNode))){
				GUI.backgroundColor = new Color(0.8f, 0.8f, 0.3f);
			}
			else{
				GUI.backgroundColor = new Color(0.4f, 0.4f, 0.4f);
			}
		}

		public static void ColorOff(){
			GUI.backgroundColor = defaultColor;
		}

	}
}