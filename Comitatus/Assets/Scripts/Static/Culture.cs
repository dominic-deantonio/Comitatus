using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Culture {

    public enum Name {
        Strovian, Boskari, Kaltan, Montisan, Estish, Afonic
    }

    //The people who live there-this might not end up being used.
    public enum Demonym {
        Strovians, Boskari, Kaltans, Montisans, Estish, Afons
    }

    public enum Place {
        Strovia, Boskara, Kalteland, Montisa, Estulnad, Afon
    }

    public static Color[] color = new Color[] {
        new Color(0,1,1),//Strovian
        new Color(1,1,0),//Boskarish
        new Color(0,0,1),//Kaltan
        new Color(1,0,1),//Montisan
        new Color(1,0,0),//Estish
        new Color(0,1,0)//Afonic
    };




}
