using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerData {
    
    public static string firstName = "";
    public static string lastName = "";
    public static int cultureID;
    public static int childHistory;
    public static int youthHistory;
    public static int adultHistory;
    public static bool createdPlayer;

    public static string GetFullName() {
        string fullName = firstName + " " + lastName;
        return fullName;
    }

    public static void InitializeRandomly() {
        if (!createdPlayer) {
            //Select cultures and backstories
            cultureID = Random.Range(0, Flavor.cultureName.Length);
            childHistory = Random.Range(0, Flavor.childhoodStory.Length);
            youthHistory = Random.Range(0, Flavor.youthStory.Length);
            adultHistory = Random.Range(0, Flavor.adultStory.Length);

            //Randomize first name based on culture
            int randFirstNameIndex = Random.Range(0, Names.firstNames[cultureID].Length);
            string randFirstName = Names.firstNames[cultureID][randFirstNameIndex];
            firstName = randFirstName;

            //Randomize last name based on culture
            int randLastNameIndex = Random.Range(0, Names.lastNames[cultureID].Length);
            string randLastName = Names.lastNames[cultureID][randLastNameIndex];
            lastName = randLastName;
        }
    }

}
