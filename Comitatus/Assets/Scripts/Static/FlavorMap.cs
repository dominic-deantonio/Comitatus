﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FlavorMap {

    public static string[] continentNames = { "Gucruace", "Sepreona", "Mobristan", "Bostein", "Snasau", "Smauland", "Ecla", "Crae Glistan", "Pliu Blus", "Gatreaya", "Dospales", "Nafristan", "Nastron", "Smaitan", "Choyria", "Uwhal", "Asmil", "Craob Frium", "Clau Crary", "Qeskoyqua", "Echuynia", "Duskary", "Agrus", "Bruysal", "Gluiqua", "Osta", "Estrijan", "Smaek Gror", "Brour Stad", "Ocriala", "Wocrarus", "Medrax", "Peflor", "Skeoso", "Swuace", "Usmeau", "Ashia", "Fraoi Snen", "Frouk Swen", "Pupruiria", "Masmiagua", "Zuchein", "Wepraria", "Scuycia", "Spubia", "Uscua", "Striaj Plye", "Smeyy Glines", "Dafluysia", "Kafruolor", "Ketraria", "Obria", "Wheque", "Skegua", "Egrax", "Uglana", "Snoep Sca", "Sheuz Plad", "Fescoarhiel", "Cushiysal", "Tafristan", "Preyra", "Troustein", "Aswus", "Etrary", "Fluo Prait", "Praut Swos", "Xetruoton", "Bechuytan", "Iustrines", "Ruspait", "Cleyton", "Droustan", "Edrines", "Aflon", "Glauf Snurg", "Sho Theau", "Hobreiland", "Muscaocia", "Peglos", "Sestrana", "Thietho", "Clainga", "Ogrye", "Ospein", "Pleud Snus", "Yethuayae", "Pawhoydal", "Mastrya", "Cechil", "Scaosil", "Cheuland", "Owha", "Usmil", "Cres Glad", "Whoib Thil", "Losciegua", "Dasnaedan", "Iostein", "Yuskurg", "Blayca", "Draorhiel", "Uwhein", "Ospua", "Sneiq Chia", "Briul Flar" };

    public static string GetRandomContinentName() {
        string s = continentNames[Random.Range(0, continentNames.Length)];
        return s;
    }

}
