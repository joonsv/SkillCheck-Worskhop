using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System;

public class NodeBasedEditor : EditorWindow
{
    private List<Node> nodes;

    private Rect skilltreeRect;
    private Rect nodeDetailRect;
    private Rect resizer;
    private GUIStyle resizerStyle;
    private bool isResizing;
    private Node selectedNode;
    private GUIStyle detailPanelStyle;

    private float sizeRatio = 0.7f;
    private List<Connection> connections;

    private GUIStyle nodeStyle;
    private GUIStyle selectedNodeStyle;
    private GUIStyle inPointStyle;
    private GUIStyle outPointStyle;

    private ConnectionPoint selectedInPoint;
    private ConnectionPoint selectedOutPoint;

    private Vector2 offset;
    private Vector2 drag;

    // Rect for buttons to Clear, Save and Load 
    private Rect rectButtonClear;
    private Rect rectButtonSave;
    private Rect rectButtonLoad;

    // Count for nodes created
    private int nodeCount;

    // Where we store the skilltree that we are managing with this tool
    private SkillTree skillTree;

    // Dictionary with the skills in our skilltree
    private Dictionary<int, Skill> skillDictionary;

    //length and width of a node
    private float nodeHeight = 150;
    private float nodeWidth = 250;
    

    [MenuItem("Window/Node Based Editor")]
    private static void OpenWindow()
    {
        NodeBasedEditor window = GetWindow<NodeBasedEditor>();
        window.titleContent = new GUIContent("Node Based Editor");
    }

    private void OnEnable()
    {
        // Create the skilltree
        skillTree = new SkillTree();

        nodeStyle = new GUIStyle();
        nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/lightskin/images/node5.png") as Texture2D;
        nodeStyle.border = new RectOffset(12, 12, 12, 12);

        selectedNodeStyle = new GUIStyle();
        selectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node5 on.png") as Texture2D;
        selectedNodeStyle.border = new RectOffset(12, 12, 12, 12);
        
        inPointStyle = new GUIStyle();
        inPointStyle.normal.background = Resources.Load("green") as Texture2D;
        inPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
        inPointStyle.border = new RectOffset(4, 4, 12, 12);

        outPointStyle = new GUIStyle();
        outPointStyle.normal.background = Resources.Load("red") as Texture2D;
        outPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
        outPointStyle.border = new RectOffset(4, 4, 12, 12);

        // Create buttons for clear, save and load
        rectButtonClear = new Rect(new Vector2(10, 10), new Vector2(60,20));
        rectButtonSave = new Rect(new Vector2(80, 10), new Vector2(60, 20));
        rectButtonLoad = new Rect(new Vector2(150, 10), new Vector2(60, 20));

        //Resizer for detailPanel
        resizerStyle = new GUIStyle();
        resizerStyle.normal.background = EditorGUIUtility.Load("icons/d_AvatarBlendBackground.png") as Texture2D;

        //Style for detailpanel
        Texture2D tex;
        var fillColor = Color.grey;
        tex = new Texture2D((int)position.width, (int)position.height, TextureFormat.RGBA32, false);
        var fillColorArray = new Color[tex.width * tex.height];
        for(var i = 0; i < fillColorArray.Length; ++i)
        {
            fillColorArray[i] = fillColor;
        }
        tex.SetPixels(fillColorArray);
        tex.Apply();
        detailPanelStyle = new GUIStyle();
        detailPanelStyle.normal.background = tex;

        // Initialize nodes with saved data
        LoadNodes();
    }

    private void OnGUI()
    {
        DrawGrid(20, 0.2f, Color.gray);
        DrawGrid(100, 0.4f, Color.gray);

        // We draw our new buttons (Clear, Load and Save)
        DrawButtons();

        DrawNodes();
        DrawConnections();

        DrawConnectionLine(Event.current);

        ProcessNodeEvents(Event.current);
        ProcessEvents(Event.current);

        DrawNodeDetailPanel();
        DrawResizer();

        if (GUI.changed)
            Repaint();
    }

    private void DrawResizer()
    {
        resizer = new Rect((position.width * sizeRatio) - 5f, 0, 5, position.height);

        GUILayout.BeginArea(new Rect(resizer.position + (Vector2.up * 5f), new Vector2(2, position.height)), resizerStyle);
        GUILayout.EndArea();

        EditorGUIUtility.AddCursorRect(resizer, MouseCursor.ResizeHorizontal);
    }

    private void DrawNodeDetailPanel()
    {
        
        nodeDetailRect = new Rect((position.width * sizeRatio), 0, position.width * (1 - sizeRatio), position.height);

        GUILayout.BeginArea(nodeDetailRect, detailPanelStyle);
        GUILayout.Label("Node details");
        if (selectedNode != null) 
        {
            Skill selectedSkill = selectedNode.skill;

            EditorGUILayout.BeginHorizontal();
            selectedSkill.name = EditorGUILayout.TextField("Name", selectedSkill.name);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            selectedSkill.cost = EditorGUILayout.IntField("Cost", selectedSkill.cost);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            selectedSkill.unlocked = EditorGUILayout.Toggle("Unlocked?", selectedSkill.unlocked);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Skill Image");
            selectedSkill.Image_sprite = (Sprite)EditorGUILayout.ObjectField(selectedSkill.Image_sprite, typeof(Sprite), allowSceneObjects: true);
            EditorGUILayout.EndHorizontal();
        }
        GUILayout.EndArea();
    }

    private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
    {
        int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
        int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

        Handles.BeginGUI();
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        offset += drag * 0.5f;
        Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

        for (int i = 0; i < widthDivs; i++)
        {
            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
        }

        for (int j = 0; j < heightDivs; j++)
        {
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
        }

        Handles.color = Color.white;
        Handles.EndGUI();
    }

    private void DrawNodes()
    {
        if (nodes != null)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Draw();
            }
        }
    }

    private void DrawConnections()
    {
        if (connections != null)
        {
            for (int i = 0; i < connections.Count; i++)
            {
                connections[i].Draw();
            }
        }
    }

    // Draw our new buttons for managing the skill tree
    private void DrawButtons()
    {
        if (GUI.Button(rectButtonClear, "Clear"))
            ClearNodes();
        if (GUI.Button(rectButtonSave, "Save"))
            SaveSkillTree();
        if (GUI.Button(rectButtonLoad, "Load"))
            LoadNodes();
    }

    private void ProcessEvents(Event e)
    {
        drag = Vector2.zero;

        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    ClearConnectionSelection();
                }

                if (e.button == 1)
                {
                    ProcessContextMenu(e.mousePosition);
                }
                break;

            case EventType.MouseDrag:
                if (e.button == 0 && !resizer.Contains(e.mousePosition) && isResizing == false)
                {
                    OnDrag(e.delta);
                }

                if (e.button == 0 && resizer.Contains(e.mousePosition) && isResizing == false)
                {
                    isResizing = true;
                    Resize(e);
                }

                if (isResizing == true)
                {
                    Resize(e);
                }
                break;

            case EventType.MouseUp:
                if (isResizing == true)
                {
                    isResizing = false;
                }
                break;
        }

        for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].isSelected == true) {
                    selectedNode = nodes[i];
                }
            }
    }

    private void Resize(Event e)
    {
        if (isResizing)
        {
            sizeRatio = e.mousePosition.x / position.width;
            Repaint();
        }
    }

    private void ProcessNodeEvents(Event e)
    {
        if (nodes != null)
        {
            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                bool guiChanged = nodes[i].ProcessEvents(e);

                if (guiChanged)
                {
                    GUI.changed = true;
                }
            }
        }
    }

    private void DrawConnectionLine(Event e)
    {
        if (selectedInPoint != null && selectedOutPoint == null)
        {
            Handles.DrawBezier(
                selectedInPoint.rect.center,
                e.mousePosition,
                selectedInPoint.rect.center + Vector2.left * 50f,
                e.mousePosition - Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }

        if (selectedOutPoint != null && selectedInPoint == null)
        {
            Handles.DrawBezier(
                selectedOutPoint.rect.center,
                e.mousePosition,
                selectedOutPoint.rect.center - Vector2.left * 50f,
                e.mousePosition + Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }
    }

    private void ProcessContextMenu(Vector2 mousePosition)
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Add node"), false, () => OnClickAddNode(mousePosition));
        genericMenu.ShowAsContext();
    }

    private void OnDrag(Vector2 delta)
    {
        drag = delta;

        if (nodes != null)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Drag(delta);
            }
        }

        GUI.changed = true;
    }

    private void OnClickAddNode(Vector2 mousePosition)
    {
        if (nodes == null)
        {
            nodes = new List<Node>();
        }
        

        // We create the node with the default info for the node
        nodes.Add(new Node(mousePosition, nodeWidth, nodeHeight, nodeStyle, selectedNodeStyle,
            inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode,
            "new_name",nodeCount, false, 0, null,null));
        ++nodeCount;
    }

    private void OnClickInPoint(ConnectionPoint inPoint)
    {
        selectedInPoint = inPoint;

        if (selectedOutPoint != null)
        {
            if (selectedOutPoint.node != selectedInPoint.node)
            {
                CreateConnection();
                ClearConnectionSelection();
            }
            else
            {
                ClearConnectionSelection();
            }
        }
    }

    private void OnClickOutPoint(ConnectionPoint outPoint)
    {
        selectedOutPoint = outPoint;

        if (selectedInPoint != null)
        {
            if (selectedOutPoint.node != selectedInPoint.node)
            {
                CreateConnection();
                ClearConnectionSelection();
            }
            else
            {
                ClearConnectionSelection();
            }
        }
    }

    private void OnClickRemoveNode(Node node)
    {
        if (connections != null)
        {
            List<Connection> connectionsToRemove = new List<Connection>();

            for (int i = 0; i < connections.Count; i++)
            {
                if (connections[i].inPoint == node.inPoint || connections[i].outPoint == node.outPoint)
                {
                    connectionsToRemove.Add(connections[i]);
                }
            }

            for (int i = 0; i < connectionsToRemove.Count; i++)
            {
                connections.Remove(connectionsToRemove[i]);
            }

            connectionsToRemove = null;
        }

        nodes.Remove(node);
    }

    private void OnClickRemoveConnection(Connection connection)
    {
        connections.Remove(connection);
    }

    private void CreateConnection()
    {
        if (connections == null)
        {
            connections = new List<Connection>();
        }

        connections.Add(new Connection(selectedInPoint, selectedOutPoint, OnClickRemoveConnection));
    }

    private void ClearConnectionSelection()
    {
        selectedInPoint = null;
        selectedOutPoint = null;
    }
    
    // Function for clearing data from the editor window
    private void ClearNodes()
    {
        nodeCount = 0;
        if (nodes != null && nodes.Count > 0)
        {
            Node node;
            while (nodes.Count > 0)
            {
                node = nodes[0];

                OnClickRemoveNode(node);
            }
        }
    }
    
    // Save data from the window to the skill tree
    private void SaveSkillTree()
    {
        if (nodes.Count > 0)
        {
            // We fill with as many skills as nodes we have
            skillTree.skilltree = new Skill[nodes.Count];
            int[] dependencies;
            List<int> dependenciesList = new List<int>();

            // Iterate over all of the nodes. Populating the skills with the node info
            for (int i = 0; i < nodes.Count; ++i)
            {
                if (connections != null)
                {
                    List<Connection> connectionsToRemove = new List<Connection>();
                    List<ConnectionPoint> connectionsPointsToCheck = new List<ConnectionPoint>();

                    for (int j = 0; j < connections.Count; j++)
                    {
                        if (connections[j].inPoint == nodes[i].inPoint)
                        {
                            for (int k = 0; k < nodes.Count; ++k)
                            {
                                if (connections[j].outPoint == nodes[k].outPoint)
                                {
                                    dependenciesList.Add(k);
                                    break;
                                }
                            }
                            connectionsToRemove.Add(connections[j]);
                            connectionsPointsToCheck.Add(connections[j].outPoint);
                        }
                    }
                }
                dependencies = dependenciesList.ToArray();
                dependenciesList.Clear();
                skillTree.skilltree[i] = nodes[i].skill;
                skillTree.skilltree[i].skill_Dependencies = dependencies;
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
            UnityEditor.AssetDatabase.Refresh();

            SaveNodes();
        }
    }

    // Save data from the nodes (position in our custom editor window)
    private void SaveNodes()
    {
        NodeDataCollection nodeData = new NodeDataCollection();
        nodeData.nodeDataCollection = new NodeData[nodes.Count];

        for (int i = 0; i < nodes.Count; ++i)
        {
            nodeData.nodeDataCollection[i] = new NodeData();
            nodeData.nodeDataCollection[i].id_Node = nodes[i].skill.id_Skill;
            nodeData.nodeDataCollection[i].position = nodes[i].rect.position;
        }

        string json = JsonUtility.ToJson(nodeData);
        string path = "Assets/SkillTree/Data/nodeData.json";

        using (FileStream fs = new FileStream(path, FileMode.Create))
        {
            using (StreamWriter writer = new StreamWriter(fs))
            {
                writer.Write(json);
            }
        }
        UnityEditor.AssetDatabase.Refresh();
    }
    
    private void LoadNodes()
    {
        ClearNodes();

        string path = "Assets/SkillTree/Data/nodeData.json";
        string dataAsJson;
        NodeDataCollection loadedData;
        if (File.Exists(path))
        {
            // Read the json from the file into a string
            dataAsJson = File.ReadAllText(path);

            // Pass the json to JsonUtility, and tell it to create a SkillTree object from it
            loadedData = JsonUtility.FromJson<NodeDataCollection>(dataAsJson);

            Skill[] _skillTree;
            List<Skill> originNode = new List<Skill>();
            skillDictionary = new Dictionary<int, Skill>();
            path = "Assets/SkillTree/Data/skilltree.json";
            Vector2 pos = Vector2.zero;
            if (File.Exists(path))
            {
                // Read the json from the file into a string
                dataAsJson = File.ReadAllText(path);

                // Pass the json to JsonUtility, and tell it to create a SkillTree object from it
                SkillTree skillData = JsonUtility.FromJson<SkillTree>(dataAsJson);

                // Store the SkillTree as an array of Skill
                _skillTree = new Skill[skillData.skilltree.Length];
                _skillTree = skillData.skilltree;

                // Create nodes
                for (int i = 0; i < _skillTree.Length; ++i)
                {
                    for (int j = 0; j < loadedData.nodeDataCollection.Length; ++j)
                    {
                        if (loadedData.nodeDataCollection[j].id_Node == _skillTree[i].id_Skill)
                        {
                            pos = loadedData.nodeDataCollection[j].position;
                            break;
                        }
                    }
                    LoadSkillCreateNode(_skillTree[i], pos);
                    if (_skillTree[i].skill_Dependencies.Length == 0)
                    {
                        originNode.Add(_skillTree[i]);
                    }
                    skillDictionary.Add(_skillTree[i].id_Skill, _skillTree[i]);
                }

                Skill outSkill;
                Node outNode = null;
                // Create connections
                for (int i = 0; i < nodes.Count; ++i)
                {
                    for (int j = 0; j < nodes[i].skill.skill_Dependencies.Length; ++j)
                    {
                        if (skillDictionary.TryGetValue(nodes[i].skill.skill_Dependencies[j], out outSkill))
                        {
                            for (int k = 0; k < nodes.Count; ++k)
                            {
                                if (nodes[k].skill.id_Skill == outSkill.id_Skill)
                                {
                                    outNode = nodes[k];
                                    OnClickOutPoint(outNode.outPoint);
                                    break;
                                }
                            }
                            OnClickInPoint(nodes[i].inPoint);
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("Cannot load game data!");
            }
        }        
    }

    private void LoadSkillCreateNode(Skill skill, Vector2 position)
    {
        if (nodes == null)
        {
            nodes = new List<Node>();
        }

        nodes.Add(new Node(position, nodeWidth, nodeHeight, nodeStyle, selectedNodeStyle,
            inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode, 
            skill.name ,skill.id_Skill, skill.unlocked, skill.cost, skill.skill_Dependencies,skill.Image_sprite));
        ++nodeCount;
    }
}