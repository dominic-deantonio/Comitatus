using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Culture {

    public enum Name {
        Strovian, Boskarish, Kaltan, Montisan, Estish, Afonic
    }

    //The people who live there-this might not end up being used.
    public enum Demonym {
        Strovians, Boskars, Kaltans, Montisans, Estish, Afons
    }

    public static Color[] color = new Color[] {
        new Color(1,0,0),
        new Color(0,1,0),
        new Color(0,0,1),
        new Color(1,1,0),
        new Color(1,0,1),
        new Color(0,1,1)
    };




}
