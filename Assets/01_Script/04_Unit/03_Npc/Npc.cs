using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Npc : Unit, IInteractable
{
    public abstract void Interact();
   
}
