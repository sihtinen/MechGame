using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechIKSource : MonoBehaviour
{
    public enum SourceType
    {
        LookTarget = 0,
    }

    public SourceType MyType = SourceType.LookTarget;
}
