using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IUIComponent {
 	void ShowScreen();
 	void OnPreShow();
	void OnPreHide();
	void HideScreen();
}
