using System;
using System.Collections.Generic; 
using UnityEngine;
using UnityEditor;

public class Node
{
    #region Fields and Properties
    public Rect NodeRect { get; protected set; }
    public string NodeTitle { get; private set; }

    protected GUIStyle m_nodeStyle = null; 
    private GUIStyle m_defaultNodeStyle = null;
    private GUIStyle m_selectedNodeStyle = null;

    public ConnectionPoint InPoint  {get; protected set; }
    public List<ConnectionPoint> OutPoints { get; protected set; } = new List<ConnectionPoint>();

    private bool m_isDragged = false;
    public bool IsSelected { get; private set; } = false;

    protected  Action<Node> m_onRemoveNode = null;

    #region Extra Out Point Settings
    protected Action<ConnectionPoint> m_onClickOutPoint = null;
    protected GUIStyle m_outPointStyle = null;
    #endregion 

    #endregion

    #region Constructor
    /// <summary>
    /// Init the node
    /// </summary>
    /// <param name="_position"></param>
    /// <param name="_width"></param>
    /// <param name="_height"></param>
    /// <param name="_nodeStyle"></param>
    /// <param name="_selectedNodeStyle"></param>
    /// <param name="_inPointStyle"></param>
    /// <param name="_outPointStyle"></param>
    /// <param name="_onClickInPoint"></param>
    /// <param name="_onClickOutPoint"></param>
    /// <param name="_onRemoveNodeAction"></param>
    public Node(Vector2 _position, float _width, float _height, GUIStyle _nodeStyle, GUIStyle _selectedNodeStyle, GUIStyle _inPointStyle, GUIStyle _outPointStyle, Action<ConnectionPoint> _onClickInPoint, Action<ConnectionPoint> _onClickOutPoint, Action<Node> _onRemoveNodeAction)
    {
        NodeRect = new Rect(_position.x, _position.y, _width, _height);
        m_defaultNodeStyle = _nodeStyle;
        m_selectedNodeStyle = _selectedNodeStyle;
        m_nodeStyle = m_defaultNodeStyle; 
        InPoint = new ConnectionPoint(this, ConnectionPointType.In, _inPointStyle, _onClickInPoint);
        if (OutPoints == null) OutPoints = new List<ConnectionPoint>();
        m_onClickOutPoint = _onClickOutPoint;
        m_outPointStyle = _outPointStyle; 
        OutPoints.Add(new ConnectionPoint(this, ConnectionPointType.Out, m_outPointStyle, m_onClickOutPoint));

        m_onRemoveNode = _onRemoveNodeAction;
    }   
    #endregion 

    #region Methods
    /// <summary>
    /// Move the position of the Node of the delta
    /// </summary>
    /// <param name="_delta">Where to move the node position</param>
    public void Drag(Vector2 _delta)
    {
        Rect _r = new Rect(NodeRect.position + _delta, NodeRect.size);
        NodeRect = _r; 
    }

    /// <summary>
    /// Draw the Node
    /// </summary>
    public virtual void Draw()
    {
        InPoint.Draw(NodeRect);
        OutPoints.ForEach(p => p.Draw(NodeRect));
        GUI.Box(NodeRect, NodeTitle, m_nodeStyle);
    }

    protected void OnClickRemoveNode()
    {
        InPoint.ClearPoint();
        for (int i = 0; i < OutPoints.Count; i++)
        {
            OutPoints[i].ClearPoint();
        }
        InPoint = null;
        OutPoints = null; 
        m_onRemoveNode?.Invoke(this);       
    }

    protected virtual void ProcessContextMenu()
    {
        GenericMenu _genericMenu = new GenericMenu();
        _genericMenu.AddItem(new GUIContent("Remove Node"), false, OnClickRemoveNode);
        _genericMenu.ShowAsContext(); 
    }

    public bool ProcessEvent(Event _e)
    {
        switch (_e.type)
        {
            case EventType.MouseDown:
                if (_e.button == 0)
                {
                    if (NodeRect.Contains(_e.mousePosition))
                    {
                        m_isDragged = true;
                        GUI.changed = true;
                        IsSelected = true;
                        m_nodeStyle = m_selectedNodeStyle; 
                    }
                    else
                    {
                        GUI.changed = true;
                        IsSelected = false;
                        m_nodeStyle = m_defaultNodeStyle; 
                    }
                }
                else if(_e.button == 1 && IsSelected && NodeRect.Contains(_e.mousePosition))
                {
                    ProcessContextMenu();
                    _e.Use(); 
                }
                else
                    m_isDragged = false; 
                break;
            case EventType.MouseUp:
                m_isDragged = false;
                break;
            case EventType.MouseDrag:
                if(_e.button == 0 && m_isDragged)
                {
                    Drag(_e.delta);
                    _e.Use();
                    return true; 
                }
                break;
            default:
                break;
        }
        return false;
    }
    #endregion
}
