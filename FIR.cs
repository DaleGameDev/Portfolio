using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using UnityEngine;

public class FIR  
{
    //Distance and max distance for interpolate method
    const float max_distnce = 20;
    float distance;
    public float Distance
    {
        get { return distance; }
        set { distance = value / max_distnce; }
    }

    //list of floats for each txt file read and a final list with the interpolated values
    List<float> filter1Co;
    List<float> filter2Co;
    List<float> finalCo;

    public FIR()
    {
        filter1Co = new List<float>();
        filter2Co = new List<float>();
        finalCo = new List<float>();
    }

    // Start is called before the first frame update
    void Start()
    {     

    }
    // Update is called once per frame
    void Update()
    {
        
    }

    // parses in filter coefficients line by line from txt files and pushes values into the filter lists
    public void SetLowPass()
    {
        float b;
        b = new float();
        
        StreamReader File;
        File = new StreamReader("Assets\\filter1.txt");
        while (File.ReadLine() != null)
        {
            float.TryParse(File.ReadLine(), out b);

            filter1Co.Add(b);
        }
        File = new StreamReader("Assets\\filter2.txt");
        while (File.ReadLine() != null)
        {
            float.TryParse(File.ReadLine(), out b);

            filter2Co.Add(b);
        }

    }

    // Adds filter1 coefficients into finalCo list
    public void interpolateFilter()
    {    
        for(int i = 0; i < filter1Co.Count; i++)
        {
            finalCo.Add(filter1Co[i]);
        }
    }

    // Returns coefficient at the index given
    public float getInterpolatedco(int i)
    {
        return finalCo[i];
    }   

    // Returns filter1 size
    public int filtercoSize()
    {
        return filter1Co.Count;
    }   

    // Upatdes filter, clamps distance variable between 0 - 1, the uses MonoBehaviour Lerp function to interpolate between the filterCo1 list and filterCo2 list
    // then multiplies by distance before assiging the value to the finalCo list
    public void UpdateFilter()
    {        
        for (int i = 0; i < filter1Co.Count; i++)
        {
            distance = Mathf.Clamp(distance, 0, 1);
            finalCo[i] = Mathf.Lerp(filter2Co[i], filter1Co[i], distance );
          
        }
    }


}
