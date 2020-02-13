using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoulderNotes{
[CreateAssetMenu]
public class WallTypeMap : ScriptableObject
{
    //walltypenameの順番と対応させる
    public enum Type{Slab, Vertical, LOverHang, HOverHang, Roof, Bulge, Other};
	[SerializeField]
	private string[] wallTypeName;
    private static string PATH = "WallTypeMap";
	public string GetWallTypeName(int index){
		if (index < 0 || wallTypeName.Length <= index){
			return "";
		}
		return wallTypeName[index];
	}

	public string GetWallTypeName(Type t){
		return GetWallTypeName((int)t);
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
}}