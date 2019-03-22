using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBEdge
{
    IBNode GetDescendantNode();
    IBNode GetAncestorNode();
    void SetDescendantNode(IBNode node);
    void SetAncestorNode(IBNode node);
    
}
