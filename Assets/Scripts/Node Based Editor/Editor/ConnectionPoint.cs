using System;
using System.Collections.Generic; 
using UnityEngine;

public class ConnectionPoint
{

    private int m_id = 0; 

    #region Fields and Properties
    public Rect ConnectionPointRect { get; private set; }
    public ConnectionPointType Type { get; private set; } = ConnectionPointType.None;
    public Node Node { get; private set;  }
    private GUIStyle m_connectionPointStyle = null; 
    private Action<ConnectionPoint> m_onClickOnConnectionPoint = null;

    public List<Connection> Connections { get; private set; } = new List<Connection>(); 
    
    #endregion

    #region Constructor 
    public ConnectionPoint(Node _node, ConnectionPointType _type, GUIStyle _style, Action<ConnectionPoint> _onClickAction)
    {
        Node = _node;
        Type = _type;
        m_connectionPointStyle = _style;
        m_onClickOnConnectionPoint = _onClickAction;
        ConnectionPointRect = new Rect(0, 0, 10.0f, 20.0f);

        m_id = UnityEngine.Random.Range(0, 10000);
    }
    #endregion

    #region Methods
    public void ClearPoint()
    {
        for (int i = 0; i < Connections.Count; i++)
        {
            OnClickRemoveConnection(Connections[i]); 
        }
    }
    public void Draw(Rect _pos)
    {
        //Debug.Log("Point " + m_id); 
        Rect _r = ConnectionPointRect; 
        _r.y = _pos.y + (_pos.height * .5f) - ConnectionPointRect.height * 0.5f;

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
        for (int i = 0; i < Connections.Count; i++)
        {
            if (Connections[i] != null)
            {
                Connections[i].Draw();
            }
            else
            {
                Connections.RemoveAt(i);
                return;
            }
        }
    }

    public void AddConnection(ConnectionPoint _outPoint)
    {
        new Connection(this, _outPoint, OnClickRemoveConnection);
    }

    public void OnClickRemoveConnection(Connection _c)
    {
        switch (Type)
        {
            case ConnectionPointType.In:
                _c.OutPoint.RemoveConnection(_c);
                break;
            case ConnectionPointType.Out:
                _c.InPoint.RemoveConnection(_c);
                break;
            default:
                break;
        }
        Connections.Remove(_c);
        _c = null; 
    }

    public void RemoveConnection(Connection _c)
    {
        Connections.Remove(_c); 
    }
    #endregion
}

public enum ConnectionPointType
{
    None, 
    In, 
    Out
}
