using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularBuffer 
{

    
    // class members the float array that stores the values
    private float[] array;
    //length that shows how long the buffer is 
    private int length;
    //current index of the buffer
    private int current;
    
    //constructor
    public CircularBuffer(int a)
    {
        array = new float[a];
        length = a;
        current = 0;
    }

    ~CircularBuffer() { }

    //code to add to the buffer at position a
    public void Add(float a)
    {
        array[current % length] = a;
        current++;
    }

    //reading from the buffer at position a
    public float read(int a)
    {
        
        return array[mod(a, length)];
    }

    //returns the current index value
    public int getCurrent()
    {
        return current;
    }

    //mod function that can take negative inputs and handle them correctly 
    int mod(int x, int m)
    {
        return (x % m + m) % m;
    }

}