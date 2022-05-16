using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace G2T.NCD.Game {
    public enum State: int { 
        None = 0, 
        Idle, 
        Attack,
        Dead,
        Move,
        Interact,
        Aggro,
    }
}
