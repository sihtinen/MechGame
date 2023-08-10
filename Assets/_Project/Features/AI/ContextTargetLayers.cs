using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum ContextTargetLayers
{
    None = 0,

    Faction_0 = 1 << 0,
    Faction_1 = 1 << 1,
    Faction_2 = 1 << 2,
    Faction_3 = 1 << 3,

    Faction_Player = 1 << 4,

    EnvironmentHazard = 1 << 5,
    WeaponHazard = 1 << 6,
    LineOfSightHazard = 1 << 7,

    MoveTarget_0 = 1 << 8,
    MoveTarget_1 = 1 << 9,
    MoveTarget_2 = 1 << 10,
    MoveTarget_3 = 1 << 11,
}
