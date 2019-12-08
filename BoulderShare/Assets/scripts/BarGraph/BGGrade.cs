using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoulderNotes{
    [CreateAssetMenu]
    public class BGGrade : BarLabelsBase{

        [SerializeField]
        private string[] gradeNames;

        public override string GetLabelName(int index){
            if (index < 0 || gradeNames.Length <= index){
                return "";
            }
            return gradeNames[index];
        }
    }
}
