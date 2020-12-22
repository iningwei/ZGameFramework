using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TransitionType
{
    None,
    //Transitions for player
    PlayerToMove,
    PlayerToBuild,
    PlayerToStop,

    //Transitions for garbage
    GarbageToCandidate,
    GarbageToHidden,
    GarbageToTarget,
    GarbageToMoveByCleaner,
    GarbageToMove,
    GarbageToDrop,

    //Transitions for cleaner
    CleanerToInit,
    CleanerToFly,
    CleanerToMoveForthToTarget,
    CleanerToMoveBackWithTarget,
    CleanerToMoveFollowTarget,
    CleanerToPlayDropAnim,
    CleanerToDropTarget,
    CleanerToMoveBackToCar,
    CleanerToPatrolFollowCar,
}
