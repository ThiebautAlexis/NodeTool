using System;
using UnityEngine;

public class Node
{
    #region Fields and Properties
    public Rect NodeRect { get; private set; }
    public string NodeTitle { get; private set; }
    public GUIStyle NodeStyle { get; private set; }
    public GUIStyle InPointStyle  { get; private set; }
    public GUIStyle OutPointStyle { get; private set; }

    public ConnectionPoint InPoint { get; private set; }
    public ConnectionPoint OutPoint { get; private set; }

    private bool m_isDragged = false;
    #endregion

    #region Constructor
    /// <summary>
    /// Init the Node with a rect at _position with a _width and _height and a _nodeStyle 
    /// </summary>
    /// <param name="_position">Position of the node</param>
    /// <param name="_width">Width of the node</param>
    /// <param name="_height">Height of the node</param>
    /// <param name="_nodeStyle">GUI Style of the node</param>
    public Node(Vector2 _position, float _width, float _height, GUIStyle _nodeStyle, Action<ConnectionPoint> _onClickInPoint, Action<ConnectionPoint> _onClickOutPoint)
    {
        NodeRect = new Rect(_position.x, _position.y, _width, _height);
        NodeStyle = _nodeStyle;
        InPoint = new ConnectionPoint(this, ConnectionPointType.In, InPointStyle, _onClickInPoint);
        OutPoint = new ConnectionPoint(this, ConnectionPointType.Out, OutPointStyle, _onClickOutPoint); 
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
    public void Draw()
    {
        InPoint.Draw();
        OutPoint.Draw();
        GUI.Box(NodeRect, NodeTitle, NodeStyle); 
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
                    }
                    else
                    {
                        GUI.changed = true;
                    }
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
