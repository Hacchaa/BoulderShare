using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class WallTypeMap : ScriptableObject
{
	[SerializeField]
	private string[] wallTypeName;
    private static string PATH = "WallTypeMap";
	public string GetWallTypeName(int index){
		if (index < 0 || wallTypeName.Length <= index){
			return "";
		}
		return wallTypeName[index];
	}

	public string[] GetWallTypeName(){
		return wallTypeName;
	}

    private static WallTypeMap _entity;
    public static WallTypeMap Entity{
        get{
            if(_entity == null){
                _entity = Resources.Load<WallTypeMap>(PATH);

                if (_entity == null){
                    Debug.LogError(PATH + " not found.");
                }
            }

            return _entity;
        }
    }
}
