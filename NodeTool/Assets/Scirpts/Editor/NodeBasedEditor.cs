using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor; 

public class NodeBasedEditor : EditorWindow
{
    #region Fields and Properties 
    private List<Node> m_nodes = new List<Node>();
    

    #region Styles
    private GUIStyle m_nodeStyle = null;
    private GUIStyle m_inPointStyle = null;
    private GUIStyle m_outPointStyle = null;
    #endregion
    #endregion

    #region Methods

    #region static Methods
    [MenuItem("Window/NodeBasedEditor")]
    public static void OpenWindow()
    {
        NodeBasedEditor _window = GetWindow<NodeBasedEditor>();
        _window.titleContent = new GUIContent("Node Based Editor");
    }
    #endregion 

    #region Original Methods
    private void AddNodeAtPosition(Vector2 _mousePosition)
    {
        if (m_nodes == null) m_nodes = new List<Node>();
        m_nodes.Add(new Node(_mousePosition, 200, 50, m_nodeStyle, OnClickInPoint, OnClickOutPoint)); 
    }
    private void DrawNodes()
    {
        if (m_nodes == null) return;
        m_nodes.ForEach(n => n.Draw()); 
    }

    private void OnClickInPoint(ConnectionPoint _inPoint)
    {

    }

    private void OnClickOutPoint(ConnectionPoint _inPoint)
    {

    }
    private void ProcessEditorEvents(Event _e)
    {
        switch (_e.type)
        {
            case EventType.MouseDown:
                if (_e.button == 1)
                    ShowContextMenu(_e.mousePosition); 
                break;
            case EventType.MouseUp:
                break;
            case EventType.MouseMove:
                break;
            case EventType.MouseDrag:
                break;
            default:
                break;
        }
    }

    private void ProcessNodesEvents(Event _e)
    {
        if (m_nodes == null) return;
        bool _guiChanged = false; 
        for (int i = 0; i < m_nodes.Count; i++)
        {
            _guiChanged = m_nodes[i].ProcessEvent(_e); 
            if(_guiChanged)
            {
                GUI.changed = true; 
            }
        }
    }

    private void ShowContextMenu(Vector2 _mousePosition)
    {
        GenericMenu _genericMenu = new GenericMenu();
        _genericMenu.AddItem(new GUIContent("Add Node"), false, () => AddNodeAtPosition(_mousePosition));
        _genericMenu.ShowAsContext(); 
    }
    #endregion

    #region Unity Methods
    private void OnEnable()
    {
        m_nodeStyle = new GUIStyle();
        m_nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        m_nodeStyle.border = new RectOffset(12, 12, 12, 12);
    }
    private void OnGUI()
    {
        DrawNodes();

        ProcessEditorEvents(Event.current);
        ProcessNodesEvents(Event.current); 

        if (GUI.changed) Repaint(); 
    }
    #endregion

    #endregion
}
