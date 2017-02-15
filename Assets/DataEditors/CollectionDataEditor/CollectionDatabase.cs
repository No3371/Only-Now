using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEditor;

namespace DataEditor.CollectionData{
	public class CollectionDatabase : ScriptableObject {

		public long lastBackupTime;

		public List<CollectionDataEntry> itemList;

		public static CollectionDatabase Create(){
			CollectionDatabase created  = ScriptableObject.CreateInstance<CollectionDatabase>();
			created.itemList = new List<CollectionDataEntry>();
			return created;
		}

		public List<CollectionDataEntry> GetEntriesByTag(string filter){
			return itemList.Where(e => e.tags.Contains(filter)).ToList();
		}

#region Json Import/export

		public void CreateFromJsonArray(string[] jsonArr){
			while (itemList.Count < jsonArr.Length){
				CollectionDataEntry newEntry = CollectionDataEntry.Create("NewItem");
				AddSubAsset(newEntry);
				itemList.Add(newEntry);
			}
			for (int i = 0; i < jsonArr.Length; i++){
				JsonUtility.FromJsonOverwrite(jsonArr[i], itemList[i]);
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
			for (int i = 0; i < itemList.Count; i++){
				string toCut = JsonUtility.ToJson(itemList[i]);
				toCut = toCut.Replace(Regex.Match(toCut, ",\"itemThumbnail\":.*,\"itemImage\":[^}]*}{1}").ToString(), "");
				temp += toCut;
				if (i < itemList.Count - 1) temp += ",\n";
			}
			temp += "\n]";
			return temp;
		}
		

		#endregion
	}

}