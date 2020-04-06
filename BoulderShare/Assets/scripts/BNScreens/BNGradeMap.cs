using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BoulderNotes{
    [CreateAssetMenu]
    public class BNGradeMap : ScriptableObject{
        public enum Grade{None,Q10, Q9, Q8, Q7, Q6, Q5, Q4, Q3, Q2, Q1, D1, D1p, D2, D2p, D3, D3p, D4, D4p, D5, D5p, D6};
        [SerializeField] private string[] gradeNames;
        private static string PATH = "BNGradeMap";

        public string GetGradeName(int index){
            if (index < 0 || gradeNames.Length <= index){
                return "";
            }
            return gradeNames[index];
        }
        public string GetGradeName(Grade g){
            return GetGradeName((int)g);
        }

        public string[] GetGradeNames(){
            string[] copy = new string[gradeNames.Length];
            Array.Copy(gradeNames, copy, gradeNames.Length);
            return copy;
        }

        public int GetSize(){
            return gradeNames.Length;
        }

        
        private static BNGradeMap _entity;
        public static BNGradeMap Entity{
            get{
                if(_entity == null){
                    _entity = Resources.Load<BNGradeMap>(PATH);

                    if (_entity == null){
                        Debug.LogError(PATH + " not found.");
                    }
                }

                return _entity;
            }
        }
    }
}
