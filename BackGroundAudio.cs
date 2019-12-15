using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

public class BackGroundAudio:MonoBehaviour
{
  
    BackGroundDSP bg;

    

    BackGroundAudio()
    {

    }

    ~BackGroundAudio()
    {

    }

    // Initialises backGround audio, loads and plays on Unity Start
    void Start()
    {
        bg = new BackGroundDSP();

        bg.InitialiseDSP();
        bg.LoadMusicStream("Assets//BackGround Music.mp3");
        bg.PlayMusicStream();
    }

    // updates the backGround audio object
    void Update()
    {
        bg.Update();
    }


    //Pause
    public void Pause(bool v)
    {
        bg.pause(v);
    }

}

public class BackGroundDSP
{
    FMOD.RESULT res;
    FMOD.DSP_DESCRIPTION dspDesc;
    FMOD.System system;
    FMOD.Sound sound;

    FMOD.Sound music;
    FMOD.Channel channel;
    FMOD.ChannelGroup channelGroup;
    FMOD.DSP dsp;


    static FIR fir;
    static CircularBuffer buffer;
    static CircularBuffer buffer1;

   static double delay = 44100.0 * 0.003;
    const float amps = 0.7f;
    static int count = 0;

    double time;

    //BackGroundDSP constructor initialises FMOD system objects
   public BackGroundDSP()
    {
        res = new FMOD.RESULT();
        system = new FMOD.System();
        dspDesc = new FMOD.DSP_DESCRIPTION();
        dsp = new FMOD.DSP();
        sound = new FMOD.Sound();
        music = new FMOD.Sound();
        channelGroup = new FMOD.ChannelGroup();

        fir = new FIR();
        fir.SetLowPass();
        fir.interpolateFilter();
        buffer = new CircularBuffer((int)Math.Ceiling(delay));
        buffer1 = new CircularBuffer((int)Math.Ceiling(delay));

    }

    //Destructor
   ~BackGroundDSP()
    {
        channel.stop();
        channelGroup.stop();
        dsp.release();
        sound.release();
        system.release();  
    }

    void Start()
    {
       
    }

    //Updates time variable to use in Flanger filter
    public void Update()
    {
        time = Time.time;
    }
    
    
    //This is the flanger call back it uses a sin curve along with the current elapsed time to...
    //vary the effect over time. it is the multiplied by the maximum delay we then round this value and cast it to an int...
    //that int is used as an index to look back in the buffer and find a value...
    // that value is scaled with amps and mixed with the current value to get our output.
    public FMOD.RESULT Flanger(ref FMOD.DSP_STATE dSP_STATE, IntPtr inbuffer, IntPtr outbuffer, uint length, int inchannels, ref int outchannels)
    {
        unsafe        
        {          
                for (uint samp = 0; samp < length; samp++)
                {
                    for (int chan = 0; chan < outchannels; chan++)
                    {
                        float* fin = (float*)inbuffer;
                        float* fout = (float*)outbuffer;
                        if (chan == 0)
                        {
                            buffer.Add(fin[((samp * inchannels) + chan)]);

                            fout[((samp * outchannels) + chan)] = fin[((samp * inchannels) + chan)] * amps + buffer.read(buffer.getCurrent() - (int)Math.Round(((Math.Sin(time)+1)/2) * delay)) * amps;

                        }
                        else if (chan == 1)
                        {
                            buffer1.Add(fin[((samp * inchannels) + chan)]);

                            fout[((samp * outchannels) + chan)] = fin[((samp * inchannels) + chan)] * amps + buffer1.read(buffer.getCurrent() - (int)Math.Round(((Math.Sin(time) + 1) / 2) * delay)) * amps;
                        }
                    }
                }
            

        }
        
        return FMOD.RESULT.OK;
    }

    //Initialise DSP
   public bool InitialiseDSP()
    {
        //create fmod system
        res = FMOD.Factory.System_Create(out system);
        if (res != FMOD.RESULT.OK)
        {
            Debug.Log("FMOD Error! create");
            //print(FMOD.Error.String(res));
            return false;
        }

        //initialise system -  1024 max channels - more than enough.
        res = system.init(1024, FMOD.INITFLAGS.NORMAL, IntPtr.Zero);
        if (res != FMOD.RESULT.OK)
        {
            Debug.Log("FMOD Error! init");
            //print(FMOD.Error.String(res));
            return false;
        }

        

        //create DSP 
        FMOD.DSP_READCALLBACK CallBack = new FMOD.DSP_READCALLBACK(Flanger);
        char[] m_name = new char[16];

        "DSP".ToCharArray().CopyTo(m_name, 0);
        dspDesc.name = m_name;
        dspDesc.numinputbuffers = 1;
        dspDesc.numoutputbuffers = 1;

        dspDesc.read = CallBack;
        res = system.createDSP(ref dspDesc, out dsp);

        if (res != FMOD.RESULT.OK)
        {
            Debug.Log("FMOD Error! read callback");
           // print(FMOD.Error.String(res));
            return false;
        }

        return true;
    }

    // Load event sound from file
    public bool LoadEventSound(String filePath)
    {
        res = system.createSound(filePath, FMOD.MODE.LOOP_OFF, out sound);
        if (res != FMOD.RESULT.OK)
        {
            Debug.Log("FMOD Error! load event sound");
          //  print(FMOD.Error.String(res));
            return false;
        }

        return true;
    }

    //Play event sound
    bool PlayEventSound()
    {
        res = system.playSound(sound, channelGroup, false, out channel);
        if (res != FMOD.RESULT.OK)
        {
            Debug.Log("FMOD Error! playe event sound");
            //print(FMOD.Error.String(res));
            return false;
        }
        return true;
    }

    //Load music stream from file
    public bool LoadMusicStream(String filePath)
    {
        res = system.createStream(filePath, FMOD.MODE.LOOP_NORMAL, out music);

        if (res != FMOD.RESULT.OK)
        {
            Debug.Log("FMOD Error! load music stream");
           // print(FMOD.Error.String(res));

            return false;
        }

        return true;
    }

    //Play music stream
    public bool PlayMusicStream()
    {
       
        res = system.playSound(music, channelGroup, false, out channel);
        if (res != FMOD.RESULT.OK)
        {
            Debug.Log("FMOD Error! play music stream");
           
            return false;
        }
        channel.setVolume(0.1f);
        channel.addDSP(0, dsp);
        

        return true;
    }

    // Pause
    public void pause(bool v)
    {
        channel.setPaused(v);
    }
}

