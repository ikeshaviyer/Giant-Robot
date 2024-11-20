using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftLeg : BodyPart
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckRepairSpot();
    }

    public override void CheckRepairSpot()
    {
        if ((Input.GetKeyDown(KeyCode.A) || SerialReader.Instance.LLeg) && !attemptedToRepair && !isRepaired)
        {
            CheckRepairLogic();
        }
    }
}
