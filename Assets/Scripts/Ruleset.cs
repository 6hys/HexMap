using System.Collections.Generic;

public class Ruleset
{
    public LSystem lSystem = new LSystem();
    public Dictionary<string, List<Rule>> hexRules = new Dictionary<string, List<Rule>>();

    public Ruleset()
    {
        // L-system generates the hexagon - never changes.
        lSystem = new LSystem();
        lSystem.axiom = "X";
        lSystem.rules = new Dictionary<char, string>();
        lSystem.rules.Add('X', "ABCDEF");
        lSystem.rules.Add('A', "LA");
        lSystem.rules.Add('B', "MB");
        lSystem.rules.Add('C', "NC");
        lSystem.rules.Add('D', "OD");
        lSystem.rules.Add('E', "PE");
        lSystem.rules.Add('F', "QF");
        lSystem.rules.Add('L', "L");
        lSystem.rules.Add('M', "M");
        lSystem.rules.Add('N', "N");
        lSystem.rules.Add('O', "O");
        lSystem.rules.Add('P', "P");
        lSystem.rules.Add('Q', "Q");

        // Rules for the probability of tiles given their neighbours.
        // Can be changed to add types/compatabilities.
        hexRules = new Dictionary<string, List<Rule>>();
        hexRules.Add("StoneTown", new List<Rule>());                // StoneTown rules
        hexRules["StoneTown"].Add(new Rule(0.2f,    "StoneTown" ));
        hexRules["StoneTown"].Add(new Rule(0.4f,    "Stone"     ));
        hexRules["StoneTown"].Add(new Rule(0.2f,    "Town"      ));
        hexRules["StoneTown"].Add(new Rule(0.075f,  "Grass"     ));
        hexRules["StoneTown"].Add(new Rule(0.05f,   "RiverTown" ));
        hexRules["StoneTown"].Add(new Rule(0.05f,   "Water"     ));
        hexRules["StoneTown"].Add(new Rule(0.025f,  "RiverGrass"));

        hexRules.Add("Castle", new List<Rule>());                   // Castle rules
        hexRules["Castle"].Add(new Rule(0.6f,   "Wall"      ));
        hexRules["Castle"].Add(new Rule(0.15f,  "Town"      ));
        hexRules["Castle"].Add(new Rule(0.1f,   "Forest"    ));
        hexRules["Castle"].Add(new Rule(0.05f,  "RiverTown" ));
        hexRules["Castle"].Add(new Rule(0.05f,  "Water"     ));
        hexRules["Castle"].Add(new Rule(0.04f,  "Grass"     ));
        hexRules["Castle"].Add(new Rule(0.01f,  "RiverGrass"));

        hexRules.Add("Water", new List<Rule>());                    // Water rules
        hexRules["Water"].Add(new Rule(0.5f,    "Water"     ));
        hexRules["Water"].Add(new Rule(0.3f,    "Sand"      ));
        hexRules["Water"].Add(new Rule(0.1f,    "Stone"     ));
        hexRules["Water"].Add(new Rule(0.075f,  "Grass"     ));
        hexRules["Water"].Add(new Rule(0.025f,  "RiverGrass"));

        hexRules.Add("Town", new List<Rule>());                     // Town rules
        hexRules["Town"].Add(new Rule(0.35f,    "Town"      ));
        hexRules["Town"].Add(new Rule(0.15f,    "Forest"    ));
        hexRules["Town"].Add(new Rule(0.1f,     "Castle"    ));
        hexRules["Town"].Add(new Rule(0.1f,     "RiverGrass"));
        hexRules["Town"].Add(new Rule(0.1f,     "StoneTown" ));
        hexRules["Town"].Add(new Rule(0.05f,    "Dirt"      ));
        hexRules["Town"].Add(new Rule(0.05f,    "Grass"     ));
        hexRules["Town"].Add(new Rule(0.05f,    "RiverTown" ));
        hexRules["Town"].Add(new Rule(0.05f,    "Water"     ));

        hexRules.Add("RiverTown", new List<Rule>());                 // RiverTown rules
        hexRules["RiverTown"].Add(new Rule(0.35f,   "Town"      ));
        hexRules["RiverTown"].Add(new Rule(0.15f,   "Forest"    ));
        hexRules["RiverTown"].Add(new Rule(0.1f,    "Castle"    ));
        hexRules["RiverTown"].Add(new Rule(0.1f,    "RiverGrass"));
        hexRules["RiverTown"].Add(new Rule(0.1f,    "StoneTown" ));
        hexRules["RiverTown"].Add(new Rule(0.05f,   "Dirt"      ));
        hexRules["RiverTown"].Add(new Rule(0.05f,   "Grass"     ));
        hexRules["RiverTown"].Add(new Rule(0.05f,   "RiverTown" ));
        hexRules["RiverTown"].Add(new Rule(0.05f,   "Water"     ));

        hexRules.Add("Wall", new List<Rule>());                     // Wall rules
        hexRules["Wall"].Add(new Rule(0.4f,     "Wall"      ));
        hexRules["Wall"].Add(new Rule(0.2f,     "Forest"    ));
        hexRules["Wall"].Add(new Rule(0.125f,   "Town"      ));
        hexRules["Wall"].Add(new Rule(0.1f,     "Castle"    ));
        hexRules["Wall"].Add(new Rule(0.1f,     "Grass"     ));
        hexRules["Wall"].Add(new Rule(0.05f,    "RiverGrass"));
        hexRules["Wall"].Add(new Rule(0.025f,   "RiverTown" ));

        hexRules.Add("River", new List<Rule>());                    // River rules
        hexRules["River"].Add(new Rule(0.5f, "River"    ));
        hexRules["River"].Add(new Rule(0.4f, "RiverEnd" ));
        hexRules["River"].Add(new Rule(0.1f, "Water"    ));

        hexRules.Add("Grass", new List<Rule>());                    // Grass rules
        hexRules["Grass"].Add(new Rule(0.25f,   "Grass"     ));
        hexRules["Grass"].Add(new Rule(0.15f,   "Forest"    ));
        hexRules["Grass"].Add(new Rule(0.125f,  "Town"      ));
        hexRules["Grass"].Add(new Rule(0.1f,    "Dirt"      ));
        hexRules["Grass"].Add(new Rule(0.1f,    "Sand"      ));
        hexRules["Grass"].Add(new Rule(0.1f,    "Water"     ));
        hexRules["Grass"].Add(new Rule(0.05f,   "Castle"    ));
        hexRules["Grass"].Add(new Rule(0.05f,   "RiverGrass"));
        hexRules["Grass"].Add(new Rule(0.05f,   "Stone"     ));
        hexRules["Grass"].Add(new Rule(0.025f,  "RiverTown" ));

        hexRules.Add("RiverGrass", new List<Rule>());               // RiverGrass rules
        hexRules["RiverGrass"].Add(new Rule(0.05f,  "RiverGrass"));
        hexRules["RiverGrass"].Add(new Rule(0.25f,  "Grass"     ));
        hexRules["RiverGrass"].Add(new Rule(0.15f,  "Forest"    ));
        hexRules["RiverGrass"].Add(new Rule(0.125f, "Town"      ));
        hexRules["RiverGrass"].Add(new Rule(0.1f,   "Dirt"      ));
        hexRules["RiverGrass"].Add(new Rule(0.1f,   "Sand"      ));
        hexRules["RiverGrass"].Add(new Rule(0.1f,   "Water"     ));
        hexRules["RiverGrass"].Add(new Rule(0.05f,  "Castle"    ));
        hexRules["RiverGrass"].Add(new Rule(0.05f,  "Stone"     ));
        hexRules["RiverGrass"].Add(new Rule(0.025f, "RiverTown" ));

        hexRules.Add("Dirt", new List<Rule>());                     // Dirt rules
        hexRules["Dirt"].Add(new Rule(0.5f,     "Dirt"      ));
        hexRules["Dirt"].Add(new Rule(0.35f,    "Grass"     ));
        hexRules["Dirt"].Add(new Rule(0.08f,    "Town"      ));
        hexRules["Dirt"].Add(new Rule(0.05f,    "RiverGrass"));
        hexRules["Dirt"].Add(new Rule(0.02f,    "RiverTown" ));

        hexRules.Add("Forest", new List<Rule>());                   // Forest rules
        hexRules["Forest"].Add(new Rule(0.4f,   "Forest"    ));
        hexRules["Forest"].Add(new Rule(0.2f,   "Town"      ));
        hexRules["Forest"].Add(new Rule(0.15f,  "Grass"     ));
        hexRules["Forest"].Add(new Rule(0.1f,   "Castle"    ));
        hexRules["Forest"].Add(new Rule(0.05f,  "RiverGrass"));
        hexRules["Forest"].Add(new Rule(0.05f,  "RiverTown" ));
        hexRules["Forest"].Add(new Rule(0.05f,  "Water"     ));

        hexRules.Add("Sand", new List<Rule>());                     // Sand rules
        hexRules["Sand"].Add(new Rule(0.3f,     "Sand"      ));
        hexRules["Sand"].Add(new Rule(0.35f,    "Grass"     ));
        hexRules["Sand"].Add(new Rule(0.3f,     "Water"     ));
        hexRules["Sand"].Add(new Rule(0.05f,    "RiverGrass"));

        hexRules.Add("Stone", new List<Rule>());                    // Stone rules
        hexRules["Stone"].Add(new Rule(0.4f,    "Stone"     ));
        hexRules["Stone"].Add(new Rule(0.35f,   "StoneTown" ));
        hexRules["Stone"].Add(new Rule(0.1f,    "Grass"     ));
        hexRules["Stone"].Add(new Rule(0.1f,    "Water"     ));
        hexRules["Stone"].Add(new Rule(0.05f,   "RiverGrass"));

        hexRules.Add("Dock", new List<Rule>());                     // Dock rules
        hexRules["Dock"].Add(new Rule(1f, "Water"));

        hexRules.Add("RiverEnd", new List<Rule>());                 // RiverEnd rules
        hexRules["RiverEnd"].Add(new Rule(1f, "River"));
    }
}
