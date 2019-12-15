using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;




public class Audio : MonoBehaviour
{
    public GameObject player;
    DSP test;
    Audio()
    {

    }

    ~Audio()
    {

    }

    //initialises DSP object when Unity is Played
    void Start()
    {
        test = new DSP();   
        
        test.InitialiseDSP();
        test.LoadMusicStream("Assets\\Hawk.wav");
        test.PlayMusicStream();
    }

    void Update()
    {
        test.update(player);
    }


    public void Pause(bool v)
    {
        test.pause(v);
    }


}


public class DSP
{
    FMOD.RESULT res;
    FMOD.DSP_DESCRIPTION dspDesc;
    FMOD.System system;
    FMOD.Sound sound;

    FMOD.Sound music;
    FMOD.Channel channel;
    FMOD.ChannelGroup channelGroup;
    FMOD.DSP dsp;
    static int count;
    static int dCount;
    static FIR fir;
    static CircularBuffer buffer;
    static CircularBuffer buffer1;

    Vector3 pos;
    
    const double delay = 44100.0 * 0.2;
    const float amps = 0.7f;

    double time;

    //DSP construction initialises FMOD system objects, sets filters and creates new buffer objects
    public DSP()
    {
        fir = new FIR();
        fir.SetLowPass();
        fir.interpolateFilter();
        buffer = new CircularBuffer((int)Math.Ceiling(delay));
        buffer1 = new CircularBuffer((int)Math.Ceiling(delay));
        count++;
        dsp = new FMOD.DSP();

        res = new FMOD.RESULT();
        system = new FMOD.System();
        dspDesc = new FMOD.DSP_DESCRIPTION();

        sound = new FMOD.Sound();
        music = new FMOD.Sound();
        channelGroup = new FMOD.ChannelGroup();
    }

    //Destructor
    ~DSP()
    {
        channel.stop();
        channelGroup.stop();
        dsp.release();
        sound.release();
        system.release();
       
    }

    // Initialise DSP and use custom filter with the callback
    public bool InitialiseDSP()
    {
        //create fmod system
        res = FMOD.Factory.System_Create(out system);
        if (res != FMOD.RESULT.OK)
        {
            Debug.Log("FMOD Error! create");
            
            return false;
        }

        //initialise system -  1024 max channels - more than enough.
        res = system.init(1024, FMOD.INITFLAGS.NORMAL, IntPtr.Zero);
        if (res != FMOD.RESULT.OK)
        {
            Debug.Log("FMOD Error! init");
           // print(FMOD.Error.String(res));
            return false;
        }

        //create DSP 
        FMOD.DSP_READCALLBACK CallBack = new FMOD.DSP_READCALLBACK(Filter);
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
           
            return false;
        }

        return true;
    }

    //Load event sound
    public bool LoadEventSound(String filePath)
    {
        res = system.createSound(filePath, FMOD.MODE.LOOP_OFF, out sound);
        if (res != FMOD.RESULT.OK)
        {
            Debug.Log("FMOD Error! load event sound");
           
            return false;
        }

        return true;
    }

    //Play event sound
    public bool PlayEventSound()
    {
        res = system.playSound(sound, channelGroup, false, out channel);
        if (res != FMOD.RESULT.OK)
        {
            Debug.Log("FMOD Error! playe event sound");
            
            return false;
        }
        return true;
    }

    //Load music stream file
    public bool LoadMusicStream(String filePath)
    {
        res = system.createStream(filePath, FMOD.MODE.LOOP_NORMAL, out music);

        if (res != FMOD.RESULT.OK)
        {
            Debug.Log("FMOD Error! load music stream");
           
            return false;
        }

        return true;
    }

    //Plays music stream
    public bool PlayMusicStream()
    {
        res = system.playSound(music, channelGroup, false, out channel);
        if (res != FMOD.RESULT.OK)
        {
            Debug.Log("FMOD Error! play music stream");
          
            return false;
        }

        channel.addDSP(0, dsp);

        return true;
    }

    //  Custom Filter reads in the interpolated values from two txt files 
    //  with Low pass filter values that were generated from MatLab
    public FMOD.RESULT Filter(ref FMOD.DSP_STATE dSP_STATE, IntPtr inbuffer, IntPtr outbuffer, uint length, int inchannels, ref int outchannels)
    {

        //unsafe flag is set to use pointers in C# so we can read in and out from the buffers
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
                        float sum;
                        sum = new float();
                        buffer.Add(fin[((samp * inchannels) + chan)]);

                        for (int i = 0; i < fir.filtercoSize(); i++)
                        {
                            sum += buffer.read(-i + buffer.getCurrent()) * fir.getInterpolatedco(i);
                        }

                        fout[((samp * outchannels) + chan)] = sum;

                    }
                    else if (chan == 1)
                    {
                        float sum1;
                        sum1 = new float();
                        buffer1.Add(fin[((samp * inchannels) + chan)]);
                        for (int i = 0; i < fir.filtercoSize(); i++)
                        {
                            sum1 += buffer1.read(-i + buffer.getCurrent()) * fir.getInterpolatedco(i);
                        }

                        fout[((samp * outchannels) + chan)] = sum1;
                    }
                }
            }


        }

        return FMOD.RESULT.OK;
    }

    //Unity Update
    public void update(GameObject gameObject)
    {
        time = Time.time;

        fir.Distance = Vector3.Distance(pos, gameObject.transform.position);
        fir.UpdateFilter();
    }

    // Pauses sound channel
    public void pause(bool v)
    {
        channel.setPaused(v);
    }

}



