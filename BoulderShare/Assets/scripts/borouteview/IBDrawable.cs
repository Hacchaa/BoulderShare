using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBDrawable
{
    void Draw();
    void DontDraw();
    void Focus();
    void DontFocus();
}
