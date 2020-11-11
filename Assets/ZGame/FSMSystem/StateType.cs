using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StateType
{
    None,
    //States for player
    PlayerInit,
    PlayerMove,
   
    PlayerStop,

    //State for garbage
    GarbageHidden,
    GarbageCandidate,
    GarbageTarget,
    GarbageMoveByCleaner,
    GarbageMove,
    GarbageDrop,

    //State for cleaner
    CleanerInit,
    CleanerFly,
    CleanerMoveForthToTarget,
    CleanerMoveBackWithTarget,
    CleanerMoveFollowTarget,
    CleanerPlayDropAnim,
    CleanerDropTarget,
    CleanerMoveBackToCar,
}
