using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoulderNotes{
    public class CVContent_CanTryRoutes : CVContent
    {
        [SerializeField] private GymRoutesScrollerController scrollerController;
        [SerializeField] private ScrollGradeController gradeController;

        public GymRoutesScrollerController GetRoutesScrollerController(){
            return scrollerController;
        }
        public ScrollGradeController GetGradeScrollerController(){
            return gradeController;
        }
    public void MoveScroller(){
        gradeController.MoveContentByGymRoutesScroller(scrollerController.GetCurentScrollPosition());
    }
        
    }              
}
