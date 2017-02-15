using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEditor;

namespace DataEditor.InfoLogData{
	public class InfoLogDatabase : ScriptableObject {

		public long lastBackupTime;

		public List<InfoLogGroup> entryGroupList;

		public static InfoLogDatabase Create(){
			InfoLogDatabase created  = ScriptableObject.CreateInstance<InfoLogDatabase>();
			created.entryGroupList = new List<InfoLogGroup>();
			return created;
		} 

// #region Json Import/export

	// 	public void CreateFromJsonArray(string[] jsonArr){
	// 		while (entryGroupList.Count < jsonArr.Length){
	// 			InfoLogEntry newEntry = InfoLogEntry.Create();
	// 			AddSubAsset(newEntry);
	// 			entryGroupList.Add(newEntry);
	// 		}
	// 		for (int i = 0; i < jsonArr.Length; i++){
	// 			JsonUtility.FromJsonOverwrite(jsonArr[i], entryGroupList[i]);
	// 		}

	// 	}
	// 	public void AddSubAsset(ScriptableObject subAsset)
	// 	{
	// 		if (subAsset != null)
	// 		{
	// 			AssetDatabase.AddObjectToAsset(subAsset, this);
	// 			subAsset.hideFlags = HideFlags.HideInHierarchy;
	// 		}
	// 	}

	// 	public string[] ParseJsonArray(string json){
	// 		return Regex.Matches(json, "{.*}").Cast<Match>().Select(m => m.Value).ToArray();
	// 	}

	// 	public string ExportToJsonArray(){
	// 		string temp = "[\n";
	// 		for (int i = 0; i < entryGroupList.Count; i++){
	// 			string toCut = JsonUtility.ToJson(entryGroupList[i]);
	// 			toCut = toCut.Replace(Regex.Match(toCut, ",\"itemThumbnail\":.*,\"itemImage\":[^}]*}{1}").ToString(), "");
	// 			temp += toCut;
	// 			if (i < entryGroupList.Count - 1) temp += ",\n";
	// 		}
	// 		temp += "\n]";
	// 		return temp;
	// 	}
		

	// 	#endregion
	}

}