using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface NavElement
{
    // returns true if reached end of this NavElement
    abstract public bool setNextPos(NavInfo navInfo);
}
