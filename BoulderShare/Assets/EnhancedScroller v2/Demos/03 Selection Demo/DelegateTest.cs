using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void TestDel();
public class DelegateTest : MonoBehaviour
{
  private TestDel del;
  public void OnClickButton(){
      StartTest();
  }

  public void StartTest(){
      AA a = new AA();
      a.id = "first";
        del = a.Judge;

    del();

    AA a2 = new AA();
    a2.id = "second";
    del += a2.Judge;

    del();

    del -= a2.Judge;

    del();

    del -= a2.Judge;

    del();

  }
}

public class AA {
    public string id ;
  public void Judge(){
    Debug.Log("a "+ id);
  }
}

public class BB {
  public void Judge(){
    Debug.Log("b "+ this.ToString());
  }
}