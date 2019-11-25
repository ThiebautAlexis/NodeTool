using System;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;
using UnityEditor; 

public class NodeBasedEditor : EditorWindow
{
    #region Fields and Properties 
    protected List<Node> m_nodes = new List<Node>();
    //protected List<Connection> m_connections = new List<Connection>();

    protected ConnectionPoint m_inSelectedPoint = null;
    protected ConnectionPoint m_outSelectionPoint = null;

    private Vector2 m_drag;
    private Vector2 m_offset; 

    #region Styles
    protected GUIStyle m_defaultNodeStyle = null;
    protected GUIStyle m_defaultConditionStyle = null;
    protected GUIStyle m_selectedNodeStyle = null; 
    protected GUIStyle m_inPointStyle = null;
    protected GUIStyle m_outPointStyle = null;
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
    protected virtual void AddNodeAtPosition(Vector2 _mousePosition)
    {
        if (m_nodes == null) m_nodes = new List<Node>();
        m_nodes.Add(new Node(_mousePosition, 200, 50, m_defaultNodeStyle, m_selectedNodeStyle, m_inPointStyle, m_outPointStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode));
    }

    private void ClearConnectionSelection()
    {
        m_inSelectedPoint = null;
        m_outSelectionPoint = null; 
    }

    private void CreateConnection()
    {        
        m_inSelectedPoint.AddConnection(m_outSelectionPoint);
    }

    private void DrawConnectionLine(Event _e)
    {
        if(m_inSelectedPoint != null && m_outSelectionPoint == null)
        {
            Handles.DrawBezier(
            m_inSelectedPoint.ConnectionPointRect.center,
            _e.mousePosition,
            m_inSelectedPoint.ConnectionPointRect.center + Vector2.left * 50f,
            _e.mousePosition - Vector2.left * 50f,
            Color.white,
            null,
            2f
            );

            GUI.changed = true;
            return; 
        }
        if (m_outSelectionPoint != null && m_inSelectedPoint == null)
        {
            Handles.DrawBezier(
            m_outSelectionPoint.ConnectionPointRect.center,
            _e.mousePosition,
            m_outSelectionPoint.ConnectionPointRect.center - Vector2.left * 50f,
            _e.mousePosition + Vector2.left * 50f,
            Color.white,
            null,
            2f
            );
            GUI.changed = true;
        }
    }

    private void DrawNodes()
    {
        if (m_nodes == null) return;
        for (int i = 0; i < m_nodes.Count; i++)
        {
            m_nodes[i].Draw(); 
        }
    }

    protected void OnClickInPoint(ConnectionPoint _inPoint)
    {
        m_inSelectedPoint = _inPoint; 

        if(m_outSelectionPoint != null)
        {
            if(m_outSelectionPoint.Node != m_inSelectedPoint.Node)
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

    protected void OnClickOutPoint(ConnectionPoint _outPoint)
    {
        m_outSelectionPoint = _outPoint;

        if (m_inSelectedPoint != null)
        {
            if (m_inSelectedPoint.Node != m_outSelectionPoint.Node)
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

    protected void OnClickRemoveNode(Node _node)
    {
        m_nodes.Remove(_node);
        _node = null;
    }

    private void OnDrag(Vector2 _delta)
    {
        m_drag = _delta; 
        m_nodes.ForEach(n => n.Drag(_delta));
        GUI.changed = true; 
    }

    private void ProcessEditorEvents(Event _e)
    {
        m_drag = Vector2.zero;
        switch (_e.type)
        {
            case EventType.MouseDown:
                if (_e.button == 1)
                    ShowContextMenu(_e.mousePosition); 
                break;
            case EventType.MouseDrag:
                if(_e.button == 0 && !m_nodes.Any(n => n.IsSelected))
                {
                    OnDrag(_e.delta); 
                }
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

    protected virtual void ShowContextMenu(Vector2 _mousePosition)
    {
        GenericMenu _genericMenu = new GenericMenu();
        _genericMenu.AddItem(new GUIContent("Add Node"), false, () => AddNodeAtPosition(_mousePosition));
        _genericMenu.ShowAsContext(); 
    }

    private void DrawGrid(float _gridSpacing, float _gridOpacity, Color _gridColor)
    {
        int widthDivs = Mathf.CeilToInt(position.width / _gridSpacing);
        int heightDivs = Mathf.CeilToInt(position.height / _gridSpacing);

        Handles.BeginGUI();
        Handles.color = new Color(_gridColor.r, _gridColor.g, _gridColor.b, _gridOpacity);

        m_offset += m_drag * 0.5f;
        Vector3 newOffset = new Vector3(m_offset.x % _gridSpacing, m_offset.y % _gridSpacing, 0);

        for (int i = 0; i < widthDivs; i++)
        {
            Handles.DrawLine(new Vector3(_gridSpacing * i, -_gridSpacing, 0) + newOffset, new Vector3(_gridSpacing * i, position.height, 0f) + newOffset);
        }

        for (int j = 0; j < heightDivs; j++)
        {
            Handles.DrawLine(new Vector3(-_gridSpacing, _gridSpacing * j, 0) + newOffset, new Vector3(position.width, _gridSpacing * j, 0f) + newOffset);
        }

        Handles.color = Color.white;
        Handles.EndGUI();
    }
    #endregion

    #region Unity Methods
    private void OnEnable()
    {
        m_defaultNodeStyle = new GUIStyle();
        m_defaultNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        m_defaultNodeStyle.border = new RectOffset(12, 12, 12, 12);

        m_selectedNodeStyle = new GUIStyle();
        m_selectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
        m_selectedNodeStyle.border = new RectOffset(12, 12, 12, 12);

        m_inPointStyle = new GUIStyle();
        m_inPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
        m_inPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
        m_inPointStyle.border = new RectOffset(4, 4, 12, 12);

        m_outPointStyle = new GUIStyle();
        m_outPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
        m_outPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
        m_outPointStyle.border = new RectOffset(4, 4, 12, 12);
    }
    private void OnGUI()
    {
        DrawGrid(20, 0.2f, Color.black);
        DrawGrid(100, 0.4f, Color.black);

        DrawNodes();

        DrawConnectionLine(Event.current); 
        ProcessEditorEvents(Event.current);
        ProcessNodesEvents(Event.current); 

        if (GUI.changed) Repaint(); 
    }
    #endregion

    #endregion
}
