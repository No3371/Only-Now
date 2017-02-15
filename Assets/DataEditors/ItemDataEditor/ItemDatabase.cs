using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEditor;

namespace ItemDataEditor.Data{
	public class ItemDatabase : ScriptableObject {

		public long lastBackupTime;

		public List<ItemDataEntry> ItemList;

		public static ItemDatabase Create(){
			ItemDatabase created  = ScriptableObject.CreateInstance<ItemDatabase>();
			created.ItemList = new List<ItemDataEntry>();
			return created;
		} 

#region Json Import/export

		public void CreateFromJsonArray(string[] jsonArr){
			while (ItemList.Count < jsonArr.Length){
				ItemDataEntry newEntry = ItemDataEntry.Create("NewItem");
				AddSubAsset(newEntry);
				ItemList.Add(newEntry);
			}
			for (int i = 0; i < jsonArr.Length; i++){
				JsonUtility.FromJsonOverwrite(jsonArr[i], ItemList[i]);
			}

		}
		public void AddSubAsset(ScriptableObject subAsset)
		{
			if (subAsset != null)
			{
				AssetDatabase.AddObjectToAsset(subAsset, this);
				subAsset.hideFlags = HideFlags.HideInHierarchy;
			}
		}

		public string[] ParseJsonArray(string json){
			return Regex.Matches(json, "{.*}").Cast<Match>().Select(m => m.Value).ToArray();
		}

		public string ExportToJsonArray(){
			string temp = "[\n";
			for (int i = 0; i < ItemList.Count; i++){
				string toCut = JsonUtility.ToJson(ItemList[i]);
				toCut = toCut.Replace(Regex.Match(toCut, ",\"itemThumbnail\":.*,\"itemImage\":[^}]*}{1}").ToString(), "");
				temp += toCut;
				if (i < ItemList.Count - 1) temp += ",\n";
			}
			temp += "\n]";
			return temp;
		}
		

		#endregion
	}

}