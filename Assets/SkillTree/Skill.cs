using UnityEngine;

[System.Serializable]
public class Skill {
    public string name;
    public int id_Skill;
    public int[] skill_Dependencies;
    public bool unlocked;
    public int cost;
    public Texture image;
}
