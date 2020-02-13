using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BoulderNotes{
public class RecordScrollerData : RecordScrollerDataBase{
    public string date;
    public BNRecord record;
    public int completeRate;
    public string comment;
    public int tryNumber;
    public BNRecord.Condition bodyCondition; 
}
public class RecordOverviewScrollerData : RecordScrollerDataBase {
    public int days;
    public int tryCount;
    public int completeRate;
}

public class RecordSubTitleScrollerData : RecordScrollerDataBase {
}

public class RecordDateTitleScrollerData : RecordScrollerDataBase {
    public string date;
}

public class RecordLineScrollerData : RecordScrollerDataBase{
    
}
public class RecordScrollerDataBase{

}

}