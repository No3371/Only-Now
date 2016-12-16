using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItemDataEditor.Data{
	public class ItemDatabase : ScriptableObject {

		public long lastBackupTime;

		public List<ItemDataEntry> ItemList;

		public static ItemDatabase Create(){
			ItemDatabase created  = ScriptableObject.CreateInstance<ItemDatabase>();
			created.ItemList = new List<ItemDataEntry>();
			return created;
		} 
	}

}