using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StorySystem.Runtime{

	public class VariableProfileCache : MonoBehaviour {

		public static VariableProfileCache Instance;
		
		[SerializeField] Dict_float dFloat;
		[SerializeField] Dict_int dInt;
		[SerializeField] Dict_bool dBool;
		[SerializeField] Dict_string dString;
		[SerializeField] List<string> allKeyCache;

		void Awake(){
			if (Instance == null) Instance = this;
			if (Instance != this) Destroy(this);
			DontDestroyOnLoad(this);

			TryInit();
			
		}

		public void TryInit(){
			if (allKeyCache == null) allKeyCache = new List<string>();
			if (dFloat == null) dFloat = new Dict_float();
			if (dInt == null) dInt = new Dict_int();
			if (dBool == null) dBool = new Dict_bool();
			if (dString == null) dString = new Dict_string();
		}

		public bool SetVariable(string key, int value, bool overwrite = true){
			if (allKeyCache.Contains(key)){
				if (!overwrite) return false;
				else {
					dInt[key] = value;
					return true;
				}
			}
			else {
				allKeyCache.Add(key);
				dInt.Add(key, value);
				return true;
			}
		}

		public bool SetVariable(string key, float value, bool overwrite = true){
			if (allKeyCache.Contains(key)){
				if (!overwrite) return false;
				else {
					dFloat[key] = value;
					return true;
				}
			}
			else {
				allKeyCache.Add(key);
				dFloat.Add(key, value);
				return true;
			}
		}

		public bool SetVariable(string key, bool value, bool overwrite = true){
			if (allKeyCache.Contains(key)){
				if (!overwrite) return false;
				else {
					dBool[key] = value;
					return true;
				}
			}
			else {
				allKeyCache.Add(key);
				dBool.Add(key, value);
				return true;
			}
		}

		public bool SetVariable(string key, string value, bool overwrite = true){
			if (allKeyCache.Contains(key)){
				if (!overwrite) return false;
				else {
					dString[key] = value;
					return true;
				}
			}
			else {
				allKeyCache.Add(key);
				dString.Add(key, value);
				return true;
			}
		}

		public int GetInt(string key){
			return dInt[key];
		}
		public float GetFloat(string key){
			return dFloat[key];
		}
		public string GetString(string key){
			return dString[key];
		}

		public bool GetBool(string key){
			return dBool[key];
		}

		public bool CheckIfVariableExist(string key){
			return allKeyCache.Contains(key);
		}

		public bool RemoveVariable(string key){
			if (CheckIfVariableExist(key)){
				if (dFloat.ContainsKey(key)) dFloat.Remove(key);
				else if (dInt.ContainsKey(key)) dInt.Remove(key);
				else if (dBool.ContainsKey(key)) dBool.Remove(key);
				else if (dString.ContainsKey(key)) dString.Remove(key);
				allKeyCache.Remove(key);
				return true;
			}
			else return false;
		}

	}
}