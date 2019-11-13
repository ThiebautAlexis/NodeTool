using System;
using UnityEngine;

public class ConnectionPoint
{
    #region Fields and Properties
    public Rect ConnectionPointRect { get; private set; }
    public ConnectionPointType Type { get; private set; } = ConnectionPointType.None;
    public Node Node { get; private set;  }
    private GUIStyle m_connectionPointStyle = null; 
    private Action<ConnectionPoint> m_onClickOnConnectionPoint = null;
    #endregion

    #region Constructor 
    public ConnectionPoint(Node _node, ConnectionPointType _type, GUIStyle _style, Action<ConnectionPoint> _onClickAction)
    {
        Node = _node;
        Type = _type;
        m_connectionPointStyle = _style;
        m_onClickOnConnectionPoint = _onClickAction;
        ConnectionPointRect = new Rect(0, 0, 10.0f, 20.0f); 
    }
    #endregion

    #region Methods
    public void Draw()
    {
        Rect _r = ConnectionPointRect; 
        _r.y = Node.NodeRect.position.y + (Node.NodeRect.height * 0.5f) - ConnectionPointRect.height * 0.5f;

        switch (Type)
        {
            case ConnectionPointType.In:
                _r.x = Node.NodeRect.x - ConnectionPointRect.width + 8f;
                break;

            case ConnectionPointType.Out:
                _r.x = Node.NodeRect.x + Node.NodeRect.width - 8f;
                break;
        }

        ConnectionPointRect = _r; 
        if (GUI.Button(ConnectionPointRect, "" , m_connectionPointStyle))
        {
            m_onClickOnConnectionPoint?.Invoke(this);
        }
    }
    #endregion
}

public enum ConnectionPointType
{
    None, 
    In, 
    Out
}
