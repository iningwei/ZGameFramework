using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TransitionType
{
    None,
    //Transitions for player
    PlayerToIdle,
    PlayerToAttack,
    PlayerToThunder,
    PlayerToDirectionalAttack,
    PlayerToDirectionalFireAttack,
    
    PlayerToHurt,
    PlayerToSkeleton,
    PlayerToDefend,
    PlayerToHealthPortion,
    PlayerToSpinPortion,
    PlayerToVictory,
    PlayerToDeath,
}
