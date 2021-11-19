using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[DisallowMultipleComponent]
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T i;
    public static T I
    {
        get
        {
            if (i == null) i = (T) FindObjectOfType(typeof(T));
            return i;
        }
    }
}

