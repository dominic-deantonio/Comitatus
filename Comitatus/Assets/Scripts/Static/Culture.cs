using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Culture {
    //Cultures are listed by the difficulty in placing them as required by th factiondatageneration class

    public enum Name {
        Estish, Kaltan, Boskari, Montisan, Strovian, Afonic
    }

    //The people who live there-this might not end up being used.
    public enum Demonym {
        Estish, Kaltans, Boskari, Montisans, Strovians, Afons
    }

    public enum PlaceName {
        Estuland, Kalteland, Boskariye, Montisa, Strovia, Afon
    }

    public static Color[] color = new Color[] {
        new Color(1,0,0),//Estish
        new Color(0,0,1),//Kaltan
        new Color(1,1,0),//Boskarish
        new Color(1,0,1),//Montisan
        new Color(0,1,1),//Strovian
        new Color(0,1,0)//Afonic
    };




}
