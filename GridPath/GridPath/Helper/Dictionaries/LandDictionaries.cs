namespace GridPath.Helper.Dictionaries
{
    public class LandDictionaries
    {
        
        public static readonly Dictionary<(string, int), int> NotGoodLandTypes = new Dictionary<(string, int), int>
    {
        {("zastavěná plocha a nádvoří", 13), 5},
        {("vodní plocha", 11),20},
        {("ostatní plocha", 13), 100},   
        {("lesní pozemek", 10), 40},
        {("chmelnice", 3), 50},
        {("vinice", 4), 50}
    };

        public static readonly Dictionary<(string, int), int> NotPreferedLandUsages = new Dictionary<(string, int), int>
{
    { ("ostatní komunikace", 17), +100 },
    { ("ostatní dopravní plocha", 18), +70 },
    { ("zeleň", 19), +90 },
    { ("manipulační plocha", 23), +60 },
    { ("dobývací prostor", 24), +40 },
    { ("skládka", 25), +50 },
    { ("jiná plocha", 26), +70 },
    { ("neplodná plocha", 27), +90 },
    { ("mez, stráň", 30), +80 }
};
        public static readonly Dictionary<(string, int), int> ProtectionScores = new Dictionary<(string, int), int>
    {
        { ("menší chráněné území", 1), +70 },
        { ("rozsáhlé chráněné území", 2), +70 },
        { ("národní park - III.zóna", 8), +50 },
        { ("ochranné pásmo národního parku", 9), +50 },
        { ("chráněná krajinná oblast - II.-IV.zóna", 11), +70 },
        { ("zemědělský půdní fond", 27), +90 },
        { ("ochranné pásmo vodního díla", 28), +50 },
        { ("ochranné pásmo vodního díla 1.stupně", 30), +50 },
        { ("ochranné pásmo vodního díla 2.stupně", 31), +50 },
        { ("pozemek určený k plnění funkcí lesa - dočasně odňato", 36), +40 },
        { ("zemědělský půdní fond - dočasně odňato", 37), +100 },
        { ("chráněná krajinná oblast", 39), +60 },
        { ("chráněná krajinná oblast - II. zóna", 44), +60 },
        { ("chráněná krajinná oblast - III. zóna", 45), +60 },
        { ("chráněná krajinná oblast - IV. zóna", 46), +60 }
    };
    }
}
