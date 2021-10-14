using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;


public class SkillTreeGeneration : MonoBehaviour
{
    private static SkillTreeReader _instance;

    public static SkillTreeReader Instance
    {
        get
        {
            return _instance;
        }
        set
        {
        }
    }

    public GameObject skillButton;
    public Transform parent;

    private Dictionary<int, Skill> _skills;
    private Skill _skillInspected;

    Skill[] _skillTree;

    public int availablePoints = 100;
    // Start is called before the first frame update
    private void Awake()
    {
        
    }
    void Start()
    {
        _skills = new Dictionary<int, Skill>();
        GenerateSkills();
    }

    private void GenerateSkills()
    {
        string path = "Assets/SkillTree/Data/nodeData.json";
        string dataAsJson;
        NodeDataCollection loadedData;
        if (File.Exists(path))
        {
            dataAsJson = File.ReadAllText(path);

            loadedData = JsonUtility.FromJson<NodeDataCollection>(dataAsJson);

            List<Skill> originNode = new List<Skill>();
            path = "Assets/SkillTree/Data/skilltree.json";

            if (File.Exists(path))
            {
                // Read the json from the file into a string
                dataAsJson = File.ReadAllText(path);

                // Pass the json to JsonUtility, and tell it to create a SkillTree object from it
                SkillTree skillData = JsonUtility.FromJson<SkillTree>(dataAsJson);

                // Store the SkillTree as an array of Skill
                _skillTree = new Skill[skillData.skilltree.Length];
                _skillTree = skillData.skilltree;

                for (int i = 0; i < _skillTree.Length; ++i)
                {
                    _skills.Add(_skillTree[i].id_Skill, _skillTree[i]);
                }
                RectTransform menuRect = parent.GetComponent<RectTransform>();
                RectTransform skillRect = skillButton.GetComponent<RectTransform>();
                List<GameObject> buttons = new List<GameObject>();
                for (int i = 0; i < _skillTree.Length; i++)
                {
                    Vector3 v3 = loadedData.nodeDataCollection[i].position;
                    v3 = new Vector3(v3.x, v3.y / -1);
                    skillButton.name = "skillButton" + i;
                    skillButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = _skillTree[i].name ;
                    if (Resources.Load<Sprite>(_skillTree[i].image) != null) { 
                        skillButton.transform.GetChild(0).GetComponent<Image>().sprite= Resources.Load<Sprite>(_skillTree[i].image);
                    }
                    //circumvent closure
                    int tempvar = i;
                    //convert to same location as in editor
                    buttons.Add(Instantiate(skillButton, parent.position + v3 - new Vector3(menuRect.sizeDelta.x / 4, -menuRect.sizeDelta.y / 4) + new Vector3(skillRect.sizeDelta.x / 4, -skillRect.sizeDelta.y / 4), parent.rotation, parent));
                    buttons[tempvar].GetComponent<Button>().onClick.AddListener(delegate { UnlockSkill(tempvar); });

                }
            }
        }
    }




    public bool UnlockSkill(int id_Skill)
    {
        if (_skills.TryGetValue(id_Skill, out _skillInspected))
        {
            if (_skillInspected.cost <= availablePoints)
            {
                availablePoints -= _skillInspected.cost;
                _skillInspected.unlocked = true;

                // We replace the entry on the dictionary with the new one (already unlocked)
                _skills.Remove(id_Skill);
                _skills.Add(id_Skill, _skillInspected);
                saveSkillTree();
                return true;
            }
            else
            {
                return false;   // The skill can't be unlocked. Not enough points
            }
        }
        else
        {
            return false;   // The skill doesn't exist
        }
    }

    public void saveSkillTree()
    {
        SkillTree skillTree = new SkillTree();
        skillTree.skilltree = new Skill[_skills.Count];
        for(int i=0; i<_skills.Count; i++)
        {
            skillTree.skilltree[i] = _skills[i];
        }
        string json = JsonUtility.ToJson(skillTree);
        string path = null;

        path = "Assets/SkillTree/Data/skilltree.json";

        // Finally, we write the JSON string with the SkillTree data in our file
        using (FileStream fs = new FileStream(path, FileMode.Create))
        {
            using (StreamWriter writer = new StreamWriter(fs))
            {
                writer.Write(json);
            }
        }
        parent.parent.GetComponent<SkillTreeReader>().SetUpSkillTree();
        UnityEditor.AssetDatabase.Refresh();
    }

    public bool IsSkillUnlocked(int id_skill)
    {
        if (_skills.TryGetValue(id_skill, out _skillInspected))
        {
            return _skillInspected.unlocked;
        }
        else
        {
            return false;
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
