using System;
using System.Collections.Generic;
using UnityEngine;

public static class FactionDataGeneration {

    public static void AssignDivisionalCulture() {

        //First get the relevant comparator from the data previously collected
        foreach (Region reg in MapData.regions) {
            reg.taigaPercent = Region.GetBiomePercent(reg, Hex.Biome.Taiga);
            reg.desertPercent = Region.GetBiomePercent(reg, Hex.Biome.HotDesert);
            reg.tundraPercent = Region.GetBiomePercent(reg, Hex.Biome.Tundra);
            reg.grassPercent = Region.GetBiomePercent(reg, Hex.Biome.Grassland);
            reg.forestPercent = Region.GetBiomePercent(reg, Hex.Biome.Forest);
            reg.marshPercent = Region.GetBiomePercent(reg, Hex.Biome.Marsh);
        }

        //Then compare the relevant values of each region and assign
        for (int i = 0; i < 6; i++) {
            List<Region> regions = new List<Region>();
            Region taiga = new Region(), desert = new Region(), tundra = new Region(), grass = new Region(), forest = new Region(), marsh = new Region();
            foreach (Region region in MapData.regions) {
                if (region.startCulture == -1) {
                    if (region.taigaPercent > taiga.taigaPercent)
                        taiga = region;

                    if (region.desertPercent > desert.desertPercent)
                        desert = region;

                    if (region.tundraPercent > tundra.tundraPercent)
                        tundra = region;

                    if (region.grassPercent > grass.grassPercent)
                        grass = region;

                    if (region.forestPercent > forest.forestPercent)
                        forest = region;

                    if (region.marshPercent > marsh.marshPercent)
                        marsh = region;
                }
            }

            regions.Add(marsh);
            regions.Add(tundra);
            regions.Add(desert);
            regions.Add(grass);
            regions.Add(taiga);
            regions.Add(forest);

            AssignCultureToRegion(regions, i);
        }
    }

    static void AssignCultureToRegion(List<Region> options, int i) {

        try {
            MapData.regions[MapData.regions.IndexOf(options[i])].startCulture = i;
        } catch (Exception e) {
            var j = e.Source; // This is here just so the compiler doesn't keep warning me about it. Exception was handled.
            foreach (Region region in MapData.regions) {
                if (region.startCulture == -1) {
                    region.startCulture = i;
                }
                Debug.Log("Couldn't place " + ((Culture.Name)i).ToString() + " into related biome - assigned iteratively");
                break;
            }
        }

    }
}

