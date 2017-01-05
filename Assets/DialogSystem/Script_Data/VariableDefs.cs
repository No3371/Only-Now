using StorySystem.Runtime;
using System.Collections.Generic;

namespace StorySystem.Data{
		public enum VariableOperation{
			Add,
			Subtract,
			Multiply,
			Devide,
			Set
		}

		public enum VariableCondition{
			IsEqual,
			IsNotEqual,
			IsGreater,
			IsSmaller,
			IsGreaterOrEqual,
			IsSmallerOrEqual
		}
		
		public enum VariableType{
			Int,
			Float,
			String,
			Bool
		}

		[System.Serializable]
        public class VariableControlItem{
            public string key = "Key";

            public VariableType type = VariableType.Int;
            public int vInt = 0;

            public float vFloat = 0f;
            
            public VariableOperation opt = VariableOperation.Set;

            public string vString = "";
            public bool vBool = false;

            public void Control(){
                switch (this.type){
                    case VariableType.Bool:
                        ControlBool();
                        break;
                    case VariableType.Int:
                        ControlInt();
                        break;
                    case VariableType.Float:
                        ControlFloat();
                        break;
                    case VariableType.String:
                        ControlString();
                        break;
                    default:
                        break;

                }
            }

            public void ControlBool(){
                VariableProfileCache.Instance.SetVariable(this.key, this.vBool);
                return;
            }

            public void ControlInt(){
                VariableProfileCache.Instance.SetVariable(this.key, this.vInt);
                return;
            }

            public void ControlFloat(){
                VariableProfileCache.Instance.SetVariable(this.key, this.vFloat);
                return;
            }

            public void ControlString(){
                VariableProfileCache.Instance.SetVariable(this.key, this.vString);
                return;
            }

            public VariableControlItem(string key, VariableOperation opt, int value){
                this.key = key;
                this.type = VariableType.Int;
                this.opt = opt;
                this.vInt = value;
            }
            public VariableControlItem(string key, VariableOperation opt, float value){
                this.key = key;
                this.type = VariableType.Float;
                this.opt = opt;
                this.vFloat = value;
            }
            public VariableControlItem(string key, VariableOperation opt, string value){
                this.key = key;
                this.type = VariableType.String;
                this.opt = opt;
                this.vString = value;
            }
            public VariableControlItem(string key, VariableOperation opt, bool value){
                this.key = key;
                this.type = VariableType.Bool;
                this.opt = opt;
                this.vBool = value;
            }

        }
		
        [System.Serializable]
        public enum VariableConditionGroupType{
            And,
            Or
        }


        [System.Serializable]
        public class VariableConditionItem{
            public string key = "Key";

            public VariableType type = VariableType.Int;
            public int vInt = 0;

            public float vFloat = 0f;
            
            public VariableCondition cond = VariableCondition.IsEqual;

            public string vString = "";
            public bool vBool = false;

            public bool Check(){
                if (VariableProfileCache.Instance.CheckIfVariableExist(this.key)) return false;
                switch (this.type){
                    case VariableType.Bool:
                        return CheckBool();
                    case VariableType.Float:
                        return CheckFloat();
                    case VariableType.Int:
                        return CheckInt();
                    case VariableType.String:
                        return CheckString();
                    default:
                        return false;
                }

            }

            public bool CheckFloat(){                
                float value = this.vFloat;
                float profileValue = VariableProfileCache.Instance.GetFloat(this.key);
                switch (this.cond){
                    case VariableCondition.IsEqual:
                        return profileValue == value;
                    case VariableCondition.IsGreater:
                        return profileValue > value;
                    case VariableCondition.IsGreaterOrEqual:
                        return profileValue >= value;
                    case VariableCondition.IsNotEqual:
                        return profileValue != value;
                    case VariableCondition.IsSmaller:
                        return profileValue < value;
                    case VariableCondition.IsSmallerOrEqual:
                        return profileValue <= value;   
                    default:
                        return false;
                }
            }

            public bool CheckString(){
                string value = this.vString;
                string profileValue = VariableProfileCache.Instance.GetString(this.key);
                switch (this.cond){
                    case VariableCondition.IsEqual:
                        return profileValue == value;
                    case VariableCondition.IsNotEqual:
                        return profileValue != value;
                    default:
                        return false;
                }
            }

            public bool CheckBool(){
                return VariableProfileCache.Instance.GetBool(this.key);
            }

            public bool CheckInt(){
                int value = this.vInt;
                int profileValue = VariableProfileCache.Instance.GetInt(this.key);
                switch (this.cond){
                    case VariableCondition.IsEqual:
                        return profileValue == value;
                    case VariableCondition.IsGreater:
                        return profileValue > value;
                    case VariableCondition.IsGreaterOrEqual:
                        return profileValue >= value;
                    case VariableCondition.IsNotEqual:
                        return profileValue != value;
                    case VariableCondition.IsSmaller:
                        return profileValue < value;
                    case VariableCondition.IsSmallerOrEqual:
                        return profileValue <= value;   
                    default:
                        return false;
                }
            }

            public VariableConditionItem(string key, VariableCondition cond, int value){
                this.key = key;
                this.type = VariableType.Int;
                this.cond = cond;
                this.vInt = value;
            }
            public VariableConditionItem(string key, VariableCondition cond, float value){
                this.key = key;
                this.type = VariableType.Float;
                this.cond = cond;
                this.vFloat = value;
            }
            public VariableConditionItem(string key, VariableCondition cond, string value){
                this.key = key;
                this.type = VariableType.String;
                this.cond = cond;
                this.vString = value;
            }
            public VariableConditionItem(string key, VariableCondition cond, bool value){
                this.key = key;
                this.type = VariableType.Bool;
                this.cond = cond;
                this.vBool = value;
            }

        }
}