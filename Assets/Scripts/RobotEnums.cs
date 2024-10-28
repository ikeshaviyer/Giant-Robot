using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotEnums : MonoBehaviour
{
    //ROBOT INSTRUCTIONS

//so how everything should work is like this:
//You are given a deadline, the deadline has a certain amount of rounds(public int), at the start of the deadline certain parts of the robot should require resources to be repaired (refer to repair logic). If all the parts are repaired = true; before the last round, the deadlines was successful, else they fail. Between these rounds there should be some flavor text (I will need to make a dialogue of narrative text) and that will need to be implemented. Also a notification when each round goes down saying "Round (number)" should exist. AFter the repairs are successful, the next deadline should start and the difficulty should go up by 1. The higher the difficulty, the more resources should be required to repair parts of the robot. The higher the difficulty the more parts you have to repair in a deadline. Make  threshold for the difficulty, where between certain ranges is easy medium and hard. 

//Repair Logic: Each body part needs a certain amount of resources to be repaired. We need a separate repair script that will keep track of whats needed to repair because we will be using NFC cards to keep track of the resources and scan them in when you are able to repair. An arduino that has photosensors to detect if a player is on a repair part will need to be checked and ableToRepair (part) should be true reflecting that on the screen
}
