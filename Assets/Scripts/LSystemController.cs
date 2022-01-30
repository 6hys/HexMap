using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum tileDirections
{
    NE,
    NW,
    E,
    W,
    SE,
    SW
}

public class LSystemController : MonoBehaviour
{
    public Tiles tiles;
    public XmlLoader xmlLoader;
    public Tilemap tilemap;

    TileDataContainer tileDataList;

    // LSystem Values.
    public int generations = 10;
    string currentString;

    // Hex rules
    Ruleset rules = new Ruleset();

    public int retryMax = 10;

    bool isGenerating = false;

    bool shouldDraw = false;
    bool drawDebugLines = false;

    // Start is called before the first frame update
    void Start()
    {
        // Load the tile data from the XML
        tileDataList = xmlLoader.Load(tileDataList, "TileData");

        // Get the starting axiom ('X' for the central tile)
        currentString = rules.lSystem.axiom;

        // Generate the L-system based on the number of generations
        Generate();

        // Generate the tile map based on the L-system
        StartCoroutine(GenerateMap());
    }

    // Update is called once per frame
    void Update()
    {
        // Only allow generation of a new L-system if there isn't another one being generated.
        if(!isGenerating && Input.GetKeyDown(KeyCode.R))
        {
            tilemap.ClearAllTiles();
            currentString = rules.lSystem.axiom;
            Generate();
            StartCoroutine(GenerateMap());
        }
    }

    // Returns the number of tiles given the number of generations.
    //int TilesPerGen(int n)
    //{
    //    if(n <= 1) { return 1; }

    //    return TilesPerGen(n - 1) + (6 * (n - 1));
    //}

    // https://stackoverflow.com/questions/2142431/algorithm-for-creating-cells-by-spiral-on-the-hexagonal-field
    int[] h = { 1, 1, 0, -1, -1, 0, 1, 1, 0 };
    Vector3Int GetHexPosition(int i)
    {
        if (i == 0) { return Vector3Int.zero; }

        // Gets the ring layer of the position.
        int layer = (int)Mathf.Round(Mathf.Sqrt(i / 3.0f));

        // Gets the first tile in that layer.
        int firstIdxInLayer = 3 * layer * (layer - 1) + 1;
        int side = (i - firstIdxInLayer) / layer;
        int idx = (i - firstIdxInLayer) % layer;

        int hx = layer * h[side + 0] + (idx + 1) * h[side + 2];
        int hy = layer * h[side + 1] + (idx + 1) * h[side + 3];

        Vector3Int pos = new Vector3Int(hx, hy, 0);

        // Convert tile position to offset coordinates used by Unity.
        pos = AxialToOffset(pos);

        return pos;
    }

    // Converts from axial coordinates to offset.
    private Vector3Int AxialToOffset(Vector3Int axial)
    {
        axial.x = axial.x - (Mathf.CeilToInt(axial.y / 2f));
        return axial;
    }

    // Generates the L-system
    void Generate()
    {
        string newString = currentString;
        string prevGenString = currentString;

        // Each generation = one layer/ring of the hexagon.
        for(int i = 0; i < generations; i++)
        {
            // Only use the previous generations string to determine the new one.
            string genString = "";
            char[] stringChars = prevGenString.ToCharArray();

            for(int c = 0; c < stringChars.Length; c++)
            {
                char currentChar = stringChars[c];

                if(rules.lSystem.rules.ContainsKey(currentChar))
                {
                    string rule = rules.lSystem.rules[currentChar];
                    genString += rule;
                }
            }

            // Adds the new generation to the previous ones rather than completely replacing it.
            prevGenString = genString;
            newString += genString;
        }

        currentString = newString;

        Debug.Log(currentString);
    }

    // Draw the map based on the L-system & the ruleset.
    IEnumerator GenerateMap()
    {
        char[] stringChars = currentString.ToCharArray();
        int retryIndex = 0;
        int retryCounter = 0;

        isGenerating = true;

        // Iterate over the entire L-system
        for(int i = 0; i < stringChars.Length;i++)
        {
            char currentChar = stringChars[i];
            Vector3Int currentPos = GetHexPosition(i);
            List<TileData> filteredList = tileDataList._tiles;
            List<tileDirections> directions = new List<tileDirections>();
            List<string> tileTypes = new List<string>();

            // Rules for each possible character
            switch (currentChar)
            {
                case 'X':
                    {
                        // First tile - randomly chosen
                        filteredList = tileDataList._tiles;
                        break;
                    }
                case 'A':
                    {
                        // Check left, and upleft if previous character was 'L'
                        Vector3Int left = currentPos;
                        left.x -= 1;

                        filteredList = GetList(left, tileDirections.W, filteredList, ref directions, ref tileTypes);

                        if (stringChars[i - 1] == 'L')
                        {
                            Vector3Int upleft = left;
                            upleft.y += 1;

                            filteredList = GetList(upleft, tileDirections.NW, filteredList, ref directions, ref tileTypes);
                        }

                        break;
                    }
                case 'B':
                    {
                        // Check upleft and upright
                        Vector3Int upleft = currentPos;
                        if(Mathf.Abs(upleft.y) % 2 == 0)
                            upleft.x -= 1;
                        upleft.y += 1;

                        filteredList = GetList(upleft, tileDirections.NW, filteredList, ref directions, ref tileTypes);

                        Vector3Int upright = upleft;
                        upright.x += 1;

                        filteredList = GetList(upright, tileDirections.NE, filteredList, ref directions, ref tileTypes);

                        break;
                    }
                case 'C':
                    {
                        // Check upright, and right
                        Vector3Int right = currentPos;
                        right.x += 1;

                        filteredList = GetList(right, tileDirections.E, filteredList, ref directions, ref tileTypes);

                        Vector3Int upright = currentPos;
                        if(Mathf.Abs(upright.y) % 2 == 1)
                            upright.x += 1;
                        upright.y += 1;

                        filteredList = GetList(upright, tileDirections.NE, filteredList, ref directions, ref tileTypes);

                        break;
                    }
                case 'D':
                    {
                        // Check right, and downright
                        Vector3Int right = currentPos;
                        right.x += 1;

                        filteredList = GetList(right, tileDirections.E, filteredList, ref directions, ref tileTypes);

                        Vector3Int downright = currentPos;
                        downright.y -= 1;

                        filteredList = GetList(downright, tileDirections.SE, filteredList, ref directions, ref tileTypes);

                        break;
                    }
                case 'E':
                    {
                        // Check downright, and downleft
                        Vector3Int downleft = currentPos;
                        if(Mathf.Abs(downleft.y) % 2 == 0)
                            downleft.x -= 1;
                        downleft.y -= 1;

                        filteredList = GetList(downleft, tileDirections.SW, filteredList, ref directions, ref tileTypes);

                        Vector3Int downright = currentPos;
                        if(Mathf.Abs(downright.y) % 2 == 1)
                            downright.x += 1;
                        downright.y -= 1;

                        filteredList = GetList(downright, tileDirections.SE, filteredList, ref directions, ref tileTypes);

                        break;
                    }
                case 'F':
                    {
                        // Check downleft, downright, and left
                        Vector3Int downleft = currentPos;
                        if(Mathf.Abs(downleft.y) % 2 == 0)
                            downleft.x -= 1;
                        downleft.y -= 1;

                        filteredList = GetList(downleft, tileDirections.SW, filteredList, ref directions, ref tileTypes);

                        Vector3Int downright = downleft;
                        downright.x += 1;

                        filteredList = GetList(downright, tileDirections.SE, filteredList, ref directions, ref tileTypes);

                        Vector3Int left = currentPos;
                        left.x -= 1;

                        filteredList = GetList(left, tileDirections.W, filteredList, ref directions, ref tileTypes);

                        break;
                    }
                case 'L':
                    {
                        // Check left, downleft, and upleft if previous character was 'L'
                        Vector3Int left = currentPos;
                        left.x -= 1;

                        filteredList = GetList(left, tileDirections.W, filteredList, ref directions, ref tileTypes);

                        Vector3Int downleft = currentPos;
                        if (Mathf.Abs(downleft.y) % 2 == 0)
                            downleft.x -= 1;
                        downleft.y -= 1;

                        filteredList = GetList(downleft, tileDirections.SW, filteredList, ref directions, ref tileTypes);

                        if (stringChars[i-1] == 'L')
                        {
                            Vector3Int upleft = currentPos;
                            if (Mathf.Abs(upleft.y) % 2 == 0)
                                upleft.x -= 1;
                            upleft.y += 1;

                            filteredList = GetList(upleft, tileDirections.NW, filteredList, ref directions, ref tileTypes);
                        }

                        break;
                    }
                case 'M':
                    {
                        // Check upleft, upright, and left
                        Vector3Int upleft = currentPos;
                        if (Mathf.Abs(upleft.y) % 2 == 0)
                            upleft.x -= 1;
                        upleft.y += 1;

                        filteredList = GetList(upleft, tileDirections.NW, filteredList, ref directions, ref tileTypes);

                        Vector3Int upright = upleft;
                        upright.x += 1;

                        filteredList = GetList(upright, tileDirections.NE, filteredList, ref directions, ref tileTypes);

                        Vector3Int left = currentPos;
                        left.x -= 1;

                        filteredList = GetList(left, tileDirections.W, filteredList, ref directions, ref tileTypes);

                        break;
                    }
                case 'N':
                    {
                        // Check upleft upright, and right
                        Vector3Int upleft = currentPos;
                        if (Mathf.Abs(upleft.y) % 2 == 0)
                            upleft.x -= 1;
                        upleft.y += 1;

                        filteredList = GetList(upleft, tileDirections.NW, filteredList, ref directions, ref tileTypes);

                        Vector3Int upright = upleft;
                        upright.x += 1;

                        filteredList = GetList(upright, tileDirections.NE, filteredList, ref directions, ref tileTypes);

                        Vector3Int right = currentPos;
                        right.x += 1;

                        filteredList = GetList(right, tileDirections.E, filteredList, ref directions, ref tileTypes);

                        break;
                    }
                case 'O':
                    {
                        // Check upright, right, and downright
                        Vector3Int upright = currentPos;
                        if (Mathf.Abs(upright.y) % 2 == 1)
                            upright.x += 1;
                        upright.y += 1;

                        filteredList = GetList(upright, tileDirections.NE, filteredList, ref directions, ref tileTypes);

                        Vector3Int downright = currentPos;
                        if (Mathf.Abs(downright.y) % 2 == 1)
                            downright.x += 1;
                        downright.y -= 1;

                        filteredList = GetList(downright, tileDirections.SE, filteredList, ref directions, ref tileTypes);

                        Vector3Int right = currentPos;
                        right.x += 1;

                        filteredList = GetList(right, tileDirections.E, filteredList, ref directions, ref tileTypes);

                        break;
                    }
                case 'P':
                    {
                        // Check downleft, downright, and right
                        Vector3Int downleft = currentPos;
                        if (Mathf.Abs(downleft.y) % 2 == 0)
                            downleft.x -= 1;
                        downleft.y -= 1;

                        filteredList = GetList(downleft, tileDirections.SW, filteredList, ref directions, ref tileTypes);

                        Vector3Int downright = currentPos;
                        if (Mathf.Abs(downright.y) % 2 == 1)
                            downright.x += 1;
                        downright.y -= 1;

                        filteredList = GetList(downright, tileDirections.SE, filteredList, ref directions, ref tileTypes);

                        Vector3Int right = currentPos;
                        right.x += 1;

                        filteredList = GetList(right, tileDirections.E, filteredList, ref directions, ref tileTypes);

                        break;
                    }
                case 'Q':
                    {
                        // Check left, downleft, and downright
                        Vector3Int downleft = currentPos;
                        if (Mathf.Abs(downleft.y) % 2 == 0)
                            downleft.x -= 1;
                        downleft.y -= 1;

                        filteredList = GetList(downleft, tileDirections.SW, filteredList, ref directions, ref tileTypes);

                        Vector3Int downright = currentPos;
                        if (Mathf.Abs(downright.y) % 2 == 1)
                            downright.x += 1;
                        downright.y -= 1;

                        filteredList = GetList(downright, tileDirections.SE, filteredList, ref directions, ref tileTypes);

                        Vector3Int left = currentPos;
                        left.x -= 1;

                        filteredList = GetList(left, tileDirections.W, filteredList, ref directions, ref tileTypes);

                        break;
                    }
                default:
                    break;
            }

            // Get probability for every possible/valid tile.
            Dictionary<string, float> tileProbabilities = GetCombinations(filteredList, directions, tileTypes);
            int tileCount = filteredList.Count;

            if (tileProbabilities == null)
            {
                // Select random tile - should only be for the first tile.
                int index = Random.Range(0, tileCount);
                TileData newTileData = filteredList[index];
                TileBase newTile = tiles.tileList.Where(t => t.name == newTileData._name).SingleOrDefault();
                tilemap.SetTile(currentPos, newTile);

            }
            else if(tileCount > 0)
            {
                // Select based on probability
                float prob = Random.value;
                float count = 0;
                TileData newTileData = new TileData();

                // Iterate until we get to the probability value.
                foreach(TileData data in filteredList)
                {
                    count += tileProbabilities[data._name];
                    if(prob < count)
                    {
                        newTileData = data;
                        break;
                    }
                }

                TileBase newTile = tiles.tileList.Where(t => t.name == newTileData._name).SingleOrDefault();
                tilemap.SetTile(currentPos, newTile);

                // Reset the retry counter
                if (retryIndex == i)
                {
                    retryCounter = 0;
                }
            }
            else
            {
                // Go back a step. Retry up to the max
                if (retryCounter < retryMax)
                {
                    retryCounter++;
                    retryIndex = i;
                    tilemap.SetTile(GetHexPosition(i - 1), null);

                    i -= 2;
                }
                else
                {
                    // Place grass as temp replacement.
                    TileBase newTile = tiles.tileList.Where(t => t.name == "grass_N").SingleOrDefault();
                    tilemap.SetTile(currentPos, newTile);

                    retryCounter = 0;
                }
            }

            yield return new WaitForSeconds(0.05f);
        }

        isGenerating = false;
    }

    List<TileData> GetList(Vector3Int position, tileDirections dir, List<TileData> dataList, ref List<tileDirections> directionList, ref List<string> typeList)
    {
        // Get data about the tile in position.
        TileBase tile = tilemap.GetTile(position);
        TileData data = tileDataList._tiles.Where(t => t._name == tile.name).SingleOrDefault();

        string dataOpposite = "";

        // Get the data for the opposite direction, AKA the edge pointing towards the tile we want to place.
        switch (dir)
        {
            case tileDirections.E:
                dataOpposite = data._west;
                break;
            case tileDirections.SW:
                dataOpposite = data._northeast;
                break;
            case tileDirections.W:
                dataOpposite = data._east;
                break;
            case tileDirections.NE:
                dataOpposite = data._southwest;
                break;
            case tileDirections.SE:
                dataOpposite = data._northwest;
                break;
            case tileDirections.NW:
                dataOpposite = data._southeast;
                break;
            default:
                break;
        }

        directionList.Add(dir);
        typeList.Add(dataOpposite);

        return FilterTiles(dataList, dir, dataOpposite);
    }

    Dictionary<string, float> GetCombinations(List<TileData> tileList, List<tileDirections> directionList, List<string> typeList)
    {
        List<Combinations> combinations = new List<Combinations>();
        Dictionary<string, Combinations> tileCombinations = new Dictionary<string, Combinations>();
        Dictionary<string, float> tileProbabilities = new Dictionary<string, float>();

        if (directionList.Count == 0) { return null; }

        // Iterate over the filtered list of tiles.
        foreach (TileData tile in tileList)
        {
            Combinations com = new Combinations();

            // Look at each each side that will be facing a current tile.
            for(int i =0; i < directionList.Count; i++)
            {
                tileDirections dir = directionList[i];
                string tileType = "";

                // Get tiles type in that direction.
                switch(dir)
                {
                    case tileDirections.NE:
                        tileType = tile._northeast;
                        break;
                    case tileDirections.SE:
                        tileType = tile._southeast;
                        break;
                    case tileDirections.E:
                        tileType = tile._east;
                        break;
                    case tileDirections.NW:
                        tileType = tile._northwest;
                        break;
                    case tileDirections.SW:
                        tileType = tile._southwest;
                        break;
                    case tileDirections.W:
                        tileType = tile._west;
                        break;
                    default:
                        break;
                }

                // Get the rule for the adjacent tile, which has the probability for the current tile to have tileType. 
                Rule r = rules.hexRules[typeList[i]].Where(r => r.type == tileType).SingleOrDefault();

                // Add the rule to the combination
                com.rules.Add(r);

                // Multiply the current probability by the new one (eg 50% * 50% gives 25% for both to happen)
                com.probability *= r.probability;
            }

            // Has the combination of rules already been found in a previous tile
            // (many tiles have NW Grass + SE River, don't need to add that combination twice)
            bool shouldAdd = true;
            foreach(Combinations c in combinations)
            {
                if (c.Equals(com))
                {
                    shouldAdd = false;
                    c.freq++;
                    com = c;
                    break;
                }
            }
            if (shouldAdd)
                combinations.Add(com);

            tileCombinations.Add(tile._name, com);
        }

        // Work out probability from total
        float total = 0f;
        foreach(Combinations c in combinations)
        {
            // Get total of all combinations.
            total += c.probability;
        }
        foreach(Combinations c in combinations)
        {
            // Divide each probability by the total
            // Not all possible combinations are present in the tileset, so the total will likely be less that 100%/1.0
            // If total is 80%/0.8, we know that a probability of 0.4 actually represents 50% of the chance, so we making it 0.5 here.
            c.probability /= total;
        }

        // Divide by the frequency of the combination.
        // If two tiles have the same combination with probability 50%, we should halve that to give 25% chance for each tile.
        foreach(TileData tile in tileList)
        {
            Combinations com = tileCombinations[tile._name];
            tileProbabilities.Add(tile._name, com.probability / com.freq);
        }

        return tileProbabilities;
    }

    /// <summary>
    /// Filters the given list of tiles based on type/direction
    /// </summary>
    /// <param name="list">List of tiles to be filtered</param>
    /// <param name="dir">Direction to be checked on tiles</param>
    /// <param name="type">Type of the tile in that direction</param>
    /// <returns></returns>
    List<TileData> FilterTiles(List<TileData> list, tileDirections dir, string type)
    {
        List<TileData> filteredTiles = new List<TileData>();
        List<Rule> allowedTypes = rules.hexRules[type];

        foreach (TileData tile in list)
        {
            string tileType = "";

            switch (dir)
            {
                case tileDirections.E:
                    tileType = tile._east;
                    break;
                case tileDirections.SE:
                    tileType = tile._southeast;
                    break;
                case tileDirections.NE:
                    tileType = tile._northeast;
                    break;
                case tileDirections.W:
                    tileType = tile._west;
                    break;
                case tileDirections.SW:
                    tileType = tile._southwest;
                    break;
                case tileDirections.NW:
                    tileType = tile._northwest;
                    break;
            }

            // Compare the tile's type against the list of allowed types based on the rules provided.
            Rule r = allowedTypes.Where(t => t.type == tileType).SingleOrDefault();

            if (r != null)
                filteredTiles.Add(tile);
        }

        return filteredTiles;
    }
}
