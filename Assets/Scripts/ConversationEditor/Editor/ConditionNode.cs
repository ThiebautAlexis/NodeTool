using System;
using UnityEngine;
using UnityEditor; 

public class ConditionNode : Node
{

    #region CONST 
    public const float INITIAL_RECT_HEIGHT = 100;
    public const float INITIAL_RECT_WIDTH = 200;
    #endregion 

    #region Fields and Properties
    private ConditionType m_conditionType = ConditionType.None; 
    public ConditionType ConditionType
    {
        get
        {
            return m_conditionType;  
        }
        set
        {
            m_conditionType = value; 
        }
    }

    private GUIStyle m_popupStyle = null; 
    #endregion

    #region Constructor
    public ConditionNode(Vector2 _position, float _width, float _height, GUIStyle _nodeStyle, GUIStyle _selectedNodeStyle, GUIStyle _inPointStyle, GUIStyle _outPointStyle, Action<ConnectionPoint> _onClickInPoint, Action<ConnectionPoint> _onClickOutPoint, Action<Node> _onRemoveNodeAction)
     : base(_position, _width, _height, _nodeStyle, _selectedNodeStyle, _inPointStyle, _outPointStyle, _onClickInPoint, _onClickOutPoint, _onRemoveNodeAction)
    {
        OutPoints.Add(new ConnectionPoint(this, ConnectionPointType.Out, m_outPointStyle, m_onClickOutPoint));
        m_popupStyle = EditorStyles.popup;
    }
    #endregion

    #region Methods
    public override void Draw()
    {
        GUI.Box(NodeRect, NodeTitle, m_nodeStyle);
        InPoint.Draw(NodeRect);
        Rect _r = NodeRect;
        _r.height /= 2; 
        for (int i = 0; i < OutPoints.Count; i++)
        {
            _r.y += i*(NodeRect.height/2); 
            OutPoints[i].Draw(_r); 
        }
        _r = new Rect(NodeRect.position.x + 20, NodeRect.position.y + 20, NodeRect.width - 40, 15);
        ConditionType = (ConditionType)EditorGUI.EnumPopup(_r,ConditionType, m_popupStyle); 
    }
    #endregion 
}
public enum ConditionType
{
    None, 
    Object, 
    Dialog
}