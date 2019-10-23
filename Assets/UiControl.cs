using System;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiControl : MonoBehaviour
{
    public static void scr_add(Text scr, int nbr)
    {
        scr.text = (Convert.ToInt16(scr.text) + nbr).ToString();
    }
    public static string scr_add(Text scr, int nbr, int mult)
    {
        return (Convert.ToInt16(scr.text) + (nbr * mult)).ToString();
    }
}
