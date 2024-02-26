using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;


public enum ObjectType
{
    //오브젝트 상호작용들 여기다 추가 
    Door, Cover
}

public abstract class Strategy
{
    protected ObjectStrategy target;

    public Strategy(ObjectStrategy target, bool value)
    {
        this.target = target;
    }

    public Strategy(ObjectStrategy target, bool value, GameObject[] values)
    {
        this.target = target;
    }

    public abstract void Interaction();

}


public class DoorStrategy : Strategy
{
    public bool isOpen;
    public bool isRunning;
    GameObject[] targets;

    private Vector3 initialPosDoorLeft;
    private Vector3 initialPosDoorRight;

    public DoorStrategy(ObjectStrategy target, bool value, GameObject[] doors) : base(target, value)
    {
        this.target = target;
        isOpen = value;
        
        this.targets = new GameObject[2];
        initialPosDoorLeft = new Vector3();
        initialPosDoorRight = new Vector3();

        this.targets[0] = doors[0];
        this.targets[1] = doors[1];

        initialPosDoorLeft = this.targets[0].transform.position;
        initialPosDoorRight = this.targets[1].transform.position;

      
    }

    

    public override void Interaction()
    {
        if (isOpen != true)
        {
            Debug.Log("열려라 참께 ");
            isOpen = true;
            
            target.StartCoroutine(OpenDoor());

        }

        else if (isOpen == true)
        {
            isOpen = false;
            isRunning = false;
            target.StartCoroutine(CloseDoor());
        }
    }

    IEnumerator OpenDoor()
    {
        while (isOpen != false)
        {
            Vector3 currentPosLeft = targets[0].transform.position;
            Vector3 currentPosRight = targets[1].transform.position;

            Vector3 target = new Vector3(3, 0, 0);

            float doorMoveSpeed = 1f;

            float distanceLeft = Vector3.Distance(currentPosLeft, initialPosDoorLeft);
            float distanceRight = Vector3.Distance(currentPosRight, initialPosDoorRight);


            if (distanceLeft < 3f)
                targets[0].transform.Translate(target * doorMoveSpeed * Time.deltaTime);

            if (distanceRight < 3f)
                targets[1].transform.Translate(target * doorMoveSpeed * Time.deltaTime);


            yield return null;
        }
        
        yield return null;
    }

    IEnumerator CloseDoor()
    {
        while (isOpen != true)
        {
            Vector3 currentPosLeft = targets[0].transform.position;
            Vector3 currentPosRight = targets[1].transform.position;

            targets[0].transform.position = Vector3.Lerp(currentPosLeft, initialPosDoorLeft, Time.deltaTime);
            targets[1].transform.position = Vector3.Lerp(currentPosRight, initialPosDoorRight, Time.deltaTime);
            
            yield return null;
        }

        yield return null;
    }

    

}

public class CoverStrategy : Strategy
{
    public CoverStrategy(ObjectStrategy target, bool value) : base(target, value)
    {
        this.target = target;
    }

    public override void Interaction()
    {
        Debug.Log("cover 상호작용 ");
    }
}

public class ObjectStrategy : MonoBehaviour, IInteractObject
{
    [SerializeField]
    ObjectType type;

    [SerializeField]
    private GameObject[] doors;

    
    public Strategy stratey;

    [SerializeField]
    private bool value;


    ThirdPersonController owner;
    public ThirdPersonController Owner
    {
        get => owner;
        set
        {
            owner = value;
        }
    }

    

    void Awake()
    {
        switch(type)
        {
            case ObjectType.Door:
                stratey = new DoorStrategy(this, value, doors);
                break;
            default:
                break;
        }
    }


    public void ObjectInteraction()
    {
       stratey.Interaction();
    }

    
}
