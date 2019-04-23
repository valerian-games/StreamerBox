using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainmenuManager : MonoBehaviour
{
    public GameObject[] areas;
    int currentArea = 0;
    
    public void setArea(int index)
    {
        areas[currentArea].SetActive(false);
        areas[index].SetActive(true);
        currentArea = index;
    }
}
