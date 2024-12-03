using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("--------- Audio Source ----------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("--------- Clicking ----------")]
    public AudioClip clickUI1;
    public AudioClip clickUI2;
    public AudioClip clickUI3;

    [Header("--------- Win/Lose ----------")]
    public AudioClip win;
    public AudioClip lose;

    [Header("--------- Intro ----------")]
    public AudioClip electricMovement1;
    public AudioClip electricMovement2;
    public AudioClip whirringRobotNoise;

    [Header("--------- Resource Collection ----------")]
    public AudioClip depositCircuitBoards;
    public AudioClip depositEnergyCores;
    public AudioClip depositScanningResource;
    public AudioClip depositScanningError;
    public AudioClip depositScrapMetal;

    [Header("--------- Disaster ----------")]
    public AudioClip disasterAlert;

    [Header("--------- Disaster Bank ----------")]
    public AudioClip cryptoScam;
    public AudioClip recession;
    public AudioClip depression;

    [Header("--------- Disaster Blockade ----------")]
    public AudioClip trafficJam;
    public AudioClip unionBlockade;
    public AudioClip flood;
    public AudioClip asteroidStorm;

    [Header("--------- Disaster Minus Action ----------")]
    public AudioClip officeParty;
    public AudioClip sendHomeEarly;
    public AudioClip companyDownsizing;
    public AudioClip layoffs;

    [Header("--------- Disaster Minus Resource ----------")]
    public AudioClip theft;
    public AudioClip emergencyResourceRegroup;
    public AudioClip taxationWithoutRepresentation;
    public AudioClip roboRevolution;

    [Header("--------- Disaster Minus Time ----------")]
    public AudioClip deadlineShrinks;
    public AudioClip veryLowBattery;
    public AudioClip wormhole;

    [Header("--------- Disaster Revert ----------")]
    public AudioClip systemBugs;
    public AudioClip electromagneticStorm;
    public AudioClip earthquakeAlarm;

    [Header("--------- Disaster Replacement ----------")]
    public AudioClip wrongShipment;

    [Header("--------- Disaster Restriction ----------")]
    public AudioClip fullStorage;

}   
