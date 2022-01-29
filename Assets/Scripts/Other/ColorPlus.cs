using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPlus
{
    public static string ColorToName(Color color)
    {
        Dictionary<Color, string> dict = new Dictionary<Color, string>();
        dict.Add(Color.black, "black");
        dict.Add(Color.blue, "blue");
        dict.Add(Color.clear, "clear");
        dict.Add(Color.cyan, "cyan");
        dict.Add(Color.green, "green");
        dict.Add(Color.grey, "grey");
        dict.Add(Color.magenta, "magenta");
        dict.Add(Color.red, "red");
        dict.Add(Color.white, "white");
        dict.Add(Color.yellow, "yellow");

        return dict.ContainsKey(color) ? dict[color] : color.ToString();
    }
}