using System;
using UnityEngine;

public static class FactionDataGeneration {

    public static void AssignDivisionalCulture() {

        //First get the relevant percentages from the data previously collected
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

            switch (i) {
                case 0:
                    MapData.regions[MapData.regions.IndexOf(marsh)].startCulture = (int)Culture.Name.Estish;
                    break;
                case 1:
                    MapData.regions[MapData.regions.IndexOf(tundra)].startCulture = (int)Culture.Name.Kaltan;
                    break;
                case 2:
                    MapData.regions[MapData.regions.IndexOf(desert)].startCulture = (int)Culture.Name.Boskari;
                    break;
                case 3:
                    MapData.regions[MapData.regions.IndexOf(grass)].startCulture = (int)Culture.Name.Montisan;
                    break;
                case 4:
                    MapData.regions[MapData.regions.IndexOf(taiga)].startCulture = (int)Culture.Name.Strovian;
                    break;
                case 5:
                    MapData.regions[MapData.regions.IndexOf(forest)].startCulture = (int)Culture.Name.Afonic;
                    break;

            }
        }
    }
}

