using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoulderNotes{
[CreateAssetMenu]
public class RouteTapePriority : ScriptableObject
{
	[SerializeField] private string[] routeTapes;
    private Dictionary<string, int> map;
    private static string PATH = "RouteTapePriority";
    private void Init(){
        map = new Dictionary<string, int>();
        int i = 0;
        foreach(string str in routeTapes){
            map.Add(str, i);
            i++;
        }
    }
	public string GetRouteTapeName(int index){
		if (index < 0 || routeTapes.Length <= index){
			return "";
		}
		return routeTapes[index];
	}
    public int GetPriority(string str){
        if (map.ContainsKey(str)){
            return map[str];
        }
        return -1;
    }

    public int GetTapesNumber(){
        return routeTapes.Length;
    }

    private static RouteTapePriority _entity;
    public static RouteTapePriority Entity{
        get{
            if(_entity == null){
                _entity = Resources.Load<RouteTapePriority>(PATH);
                _entity.Init();
                if (_entity == null){
                    Debug.LogError(PATH + " not found.");
                }
            }

            return _entity;
        }
    }
}}