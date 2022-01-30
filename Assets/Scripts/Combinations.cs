using System.Collections.Generic;

public class Combinations
{
    public List<Rule> rules = new List<Rule>();
    public float probability = 1f;
    public int freq = 1;

    public bool Equals(Combinations b)
    {
        // Check there's an equal number of rules.
        if (rules.Count != b.rules.Count)
            return false;

        // Check that each rule is the same.
        for(int i = 0; i < rules.Count; i++)
        {
            if (rules[i].type != b.rules[i].type
                || rules[i].probability != b.rules[i].probability)
                return false;
        }

        return true;
    }
}
