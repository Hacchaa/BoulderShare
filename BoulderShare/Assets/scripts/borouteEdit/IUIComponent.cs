using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IUIComponent {
 	void Show();
 	void OnPreShow();
	void OnPreHide();
	void Hide(bool isPreProcExecution = false);
}
