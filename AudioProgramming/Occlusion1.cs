using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Occlusion1 : MonoBehaviour
{
    MeshFilter cubeMesh;
    public GameObject Cube;

    public FMOD.VECTOR[] quadArr;
    public FMOD.VECTOR[] quadArr1;
    public FMOD.VECTOR[] quadArr2;
    public FMOD.VECTOR[] quadArr3;
    public FMOD.VECTOR[] quadArr4;
    public FMOD.VECTOR[] quadArr5;

    List<FMOD.VECTOR[]> quads;

    FMOD.VECTOR temp;
    FMOD.VECTOR temp1;


    FMOD.VECTOR Input0;

    public GameObject Player;


    FMODocclusion1 fOcc;

    

    // Start is called before the first frame update this initialises classes and lists 
    //The vertexes of the cube are obtained here with the adjustment of the position of vertexes
    //taking place here to ensure winding the quads is correct
    void Start()
    {

        cubeMesh = Cube.GetComponent<MeshFilter>();
        

        quadArr = new FMOD.VECTOR[4];
        quadArr1 = new FMOD.VECTOR[4];
        quadArr2 = new FMOD.VECTOR[4];
        quadArr3 = new FMOD.VECTOR[4];
        quadArr4 = new FMOD.VECTOR[4];
        quadArr5 = new FMOD.VECTOR[4];

        quads = new List<FMOD.VECTOR[]>();

        quads.Add(quadArr); quads.Add(quadArr1); quads.Add(quadArr2); quads.Add(quadArr3); quads.Add(quadArr4); quads.Add(quadArr5);

        for (int j = 0; j < quads.Count; j++)
        {

            for (int i = 0; i < 4; ++i)
            {
                Vector3 v = cubeMesh.mesh.vertices[i+(j*4)];
                VectorToFmod(v, out Input0);
                quads[j][i] = Input0;
            }
        }

        

        temp = quads[0][2];
        temp1 = quads[0][3];

        quads[0][2] = temp1;
        quads[0][3] = temp;


        temp = quads[1][2];
        temp1 = quads[1][3];

        quads[1][2] = temp1;
        quads[1][3] = temp;

        //creating our class that handles FMOD
        fOcc = new FMODocclusion1(this.gameObject, Player, Cube);
        fOcc.InitialiseOcclusion(quads);
        //loads in phone ring 
        fOcc.LoadEventSound("Assets//PhoneRing.wav");
        //plays phone ring
        fOcc.PlayEventSound();


    }

    // Update is called once per frame 
    void Update()
    {
        
        fOcc.occUpdate();
    }
    //our method to change a vector3 to an fmod vector
    public void VectorToFmod(Vector3 input, out FMOD.VECTOR input1)
    {
        input1.x = input.x;
        input1.y = input.y;
        input1.z = input.z;
    }
    //pass the puase message through
    public void Pause(bool v)
    {
        fOcc.pause(v);
    }


}

//Class that handles the occlusion and telephone sounds
public class FMODocclusion1
{
    //memebers of the class
    //game objects passed from the Occlusion1 class
    GameObject cube;
    GameObject player;
    GameObject walls;
    //FMOD requirements
    int[] index;
    FMOD.RESULT res;
    FMOD.System system;
    FMOD.Sound sound;
    FMOD.Geometry geometry;
    FMOD.Channel channel;
    FMOD.ChannelGroup channelGroup;
    //used to keep track of the sound
    public FMOD.VECTOR fpos;
    public FMOD.VECTOR fVec;
    public FMOD.VECTOR fpanPos;
    
    //used for positioning the geometry 
    public FMOD.VECTOR occPosition;
    public FMOD.VECTOR occScale;

    //used to update the listener with
    public FMOD.VECTOR playerPos;
    FMOD.VECTOR upV;
    FMOD.VECTOR forward;

    
    //constructor passes through the game object Cube1 as Walls to get the scale
    //cube as Cube to get the world position 
    // Drone as the player so you can fly over the house 
    public FMODocclusion1(GameObject Cube, GameObject Player, GameObject Walls)
    {
        res = new FMOD.RESULT();
        system = new FMOD.System();
        sound = new FMOD.Sound();
        channel = new FMOD.Channel();
        geometry = new FMOD.Geometry();
        cube = Cube;
        player = Player;
        index = new int[6];
        walls = Walls;
    }

    //updates everyframe the position of the player with a forward
    //this also refreshes the volume as we had issues setting the volume of the channel elsewhere system.update appears to allow us to do this
    public void occUpdate()
    {
        
        playerPos.x = player.transform.position.x;
        playerPos.y = player.transform.position.y;
        playerPos.z = player.transform.position.z;
        forward.x = player.transform.forward.x;
        forward.y = player.transform.forward.x;
        forward.z = player.transform.forward.x;

        channel.set3DAttributes(ref fpos, ref fVec, ref fpanPos);
        channel.setVolume(0.05f);
        res = system.set3DListenerAttributes(0, ref playerPos, ref fpanPos, ref forward, ref upV);
        system.update();
    }
    //destructor stops the sound
    ~FMODocclusion1()
    {
        channel.stop();
        sound.release();
        system.release();

    }
    //Initialise FMOD along with some FMOD vectors that are constant 
    public bool InitialiseOcclusion(List<FMOD.VECTOR[]> geo)
    {

        upV.x = 0;
        upV.y = 1;
        upV.z = 0;

        fpos.x = cube.transform.position.x; fpos.y = cube.transform.position.y; fpos.z = cube.transform.position.z;

        fpanPos.x = 0.0f;
        fpanPos.y = 0.0f;
        fpanPos.z = 0.0f;

        occPosition.x = cube.transform.position.x;
        occPosition.y = cube.transform.position.y;
        occPosition.z = cube.transform.position.z;

        occScale.x = walls.transform.lossyScale.x;
        occScale.y = walls.transform.lossyScale.y;
        occScale.z = walls.transform.lossyScale.z;

        res = FMOD.Factory.System_Create(out system);
        if (res != FMOD.RESULT.OK)
        {
            Debug.Log("FMOD Error! create");
            return false;
        }

        res = system.init(1024, FMOD.INITFLAGS.NORMAL, IntPtr.Zero);
        if (res != FMOD.RESULT.OK)
        {
            Debug.Log("FMOD Error! init");
            return false;
        }
       
        res = system.createGeometry(100, 100, out geometry);
        res = system.set3DSettings(5.0f, 1.0f, 1.0f);
        channel.set3DOcclusion(0.7f, 0.7f);
        geometry.setPosition(ref occPosition);
        geometry.setScale(ref occScale);
        for (int i = 0; i < geo.Count; i++)
        {
            res = geometry.addPolygon(0.7f, 0.7f, true, 4, geo[i], out index[i]);            
        }

        return true;
    }

    //Used above to load our sound 
    public bool LoadEventSound(String filePath)
    {
        res = system.createSound(filePath, FMOD.MODE._3D | FMOD.MODE.LOOP_NORMAL, out sound);
        if (res != FMOD.RESULT.OK)
        {
            Debug.Log("FMOD Error! load event sound");
            return false;
        }
        return true;
    }


    //used to play the sound is defualted to puase
    public bool PlayEventSound()
    {
       res = system.playSound(sound, channelGroup, true, out channel);
        if (res != FMOD.RESULT.OK)
        {
            Debug.Log("FMOD Error! playe event sound");
            return false;
        }
        return true;
    }

    //pauses the sound 
    public void pause(bool v)
    {
        channel.setPaused(v);
        channelGroup.setPaused(v);   
    }

}