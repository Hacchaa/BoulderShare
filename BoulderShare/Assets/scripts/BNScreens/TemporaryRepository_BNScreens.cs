using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoulderNotes{
//画面間のデータ受け渡しをするために一時的にデータを保存する場所
public class TemporaryRepository_BNScreens : SingletonMonoBehaviour<TemporaryRepository_BNScreens>
{
    public BNWallImageNames bNWallImageNames;
}
}