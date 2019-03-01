using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

interface ISEComponent
{
    Color GetTopSEColor();
    Color GetBotSEColor();
	List<RectTransform> GetMarginList();
}
