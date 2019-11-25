using System;
using UnityEditor;
using UnityEngine;

public class Connection
{
    #region Fields and Properties
    public ConnectionPoint InPoint { get; private set;}
    public ConnectionPoint OutPoint { get; private set; }

    private Action<Connection> m_onClickConnection = null;

    private int m_id = 0; 
    #endregion

    #region Constructor
    public Connection(ConnectionPoint _inPoint, ConnectionPoint _outPoint, Action<Connection> _onClickConnectionAction)
    {
        InPoint = _inPoint;
        OutPoint = _outPoint;
        m_onClickConnection = _onClickConnectionAction;
        m_id = UnityEngine.Random.Range(0, 10000);
        InPoint.Connections.Add(this);
        OutPoint.Connections.Add(this); 
    }
    #endregion

    #region Methods
    public void Draw()
    {
        Handles.DrawBezier(
            InPoint.ConnectionPointRect.center,
            OutPoint.ConnectionPointRect.center,
            InPoint.ConnectionPointRect.center + Vector2.left * 50.0f,
            OutPoint.ConnectionPointRect.center - Vector2.left * 50.0f,
            Color.white,
            null,
            2.0f
            );
        if (Handles.Button((InPoint.ConnectionPointRect.center + OutPoint.ConnectionPointRect.center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap))
        {
            Debug.Log("Click"); 
            m_onClickConnection?.Invoke(this);
        }
    }
    #endregion
}
