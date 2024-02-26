using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public interface IPlayerInteractable
{
    void Interaction(int value);

    public ThirdPersonController Owner { get; }

    bool inCover { get; set; }

    Vector3 inCoverMoveDirection { get; set; }

    Vector3 inCoverProhibitedDirection { get; set; }


    void BeginMoveToCover(Vector3 targetPos);

    public Transform pos { get; }
    public int PlayerHp { get; }
   
}

public interface IMonsterInteractable
{
    void Interaction(int value);
}

public interface IInteractObject
{
    void ObjectInteraction();

    

    //public ThirdPersonController Owner { get; set; }
}
