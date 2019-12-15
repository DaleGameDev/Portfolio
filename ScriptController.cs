using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class ScriptController : MonoBehaviour
{
    public GameObject drone;
    public GameObject fps;
    public GameObject bgAudio;
    public GameObject birdsChirping;
    public GameObject Telephone;

    bool droneOn;
    bool fpsOn;

    bool bgAudioOn;
    bool birdsOn;

  public bool telephoneOn;
    //key buttons to turn sounds on and of and controls for player
    #region key buttons
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        //drone Control
        if(Input.GetKeyDown(KeyCode.Keypad1)|| Input.GetKeyDown(KeyCode.Alpha1))
        {
            droneOn = true;
            fpsOn = false;
            
        }

        if(droneOn)
        {
            drone.GetComponent<Drone>().enabled = true;
        }
        else
        {
            drone.GetComponent<Drone>().enabled = false;
        }
        //FPS control
        if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2))
        {
            fpsOn = true;
            droneOn = false;
        }

        if(fpsOn)
        {
            fps.GetComponent<RigidbodyFirstPersonController>().enabled = true; 
        }

        else
        {
            fps.GetComponent<RigidbodyFirstPersonController>().enabled = false;

        }
        //background
        if(Input.GetKeyDown(KeyCode.Keypad0)||Input.GetKeyDown(KeyCode.Alpha0))
        {
            bgAudioOn = !bgAudioOn;
        }

        if(bgAudioOn)
        {

            
            bgAudio.GetComponent<BackGroundAudio>().Pause(false);
        }
        else
        {
            bgAudio.GetComponent<BackGroundAudio>().Pause(true);
         
        }

        //birds
        if(Input.GetKeyDown(KeyCode.Keypad9)||Input.GetKeyDown(KeyCode.Alpha9))
        {
            birdsOn = !birdsOn;
        }

        if(birdsOn)
        {
            birdsChirping.GetComponent<Audio>().Pause(false);
         
        }

        else
        {
            birdsChirping.GetComponent<Audio>().Pause(true);
          
        }

        if(Input.GetKeyDown(KeyCode.T))
        {
            telephoneOn = !telephoneOn;
        }

        if (telephoneOn)
        {
            Telephone.GetComponent<Occlusion1>().Pause(false);
        }

       else
        {
            Telephone.GetComponent<Occlusion1>().Pause(true);
          
        }
    }
}
#endregion
