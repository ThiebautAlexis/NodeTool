using System; 
using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;
using UnityEditor; 

public class ConversationNode : Node
{
    #region Constants
    public const float INITIAL_RECT_HEIGHT = 200;
    public const float INITIAL_RECT_WIDTH = 300;
    private const float TITLE_HEIGHT = 20; 
    private const float BASIC_CONTENT_HEIGHT = 140;
    private const float MULTIPLE_CONTENT_HEIGHT = 60; 
    #endregion

    #region Fields and Properties
    private string m_title = string.Empty;
    private List<string> m_content = new List<string>();

    private ConversationNodeType m_nodeType = ConversationNodeType.Basic; 
    public ConversationNodeType NodeType
    {
        get
        {
            return m_nodeType;
        }
        set
        {
            m_nodeType = value;
            Rect _r = NodeRect;
            switch (m_nodeType)
            {
                case ConversationNodeType.Basic:
                    for (int i = m_content.Count - 1; i > 0; i--)
                    {
                        OutPoints[i].ClearPoint();
                        OutPoints[i] = null;
                        OutPoints.RemoveAt(i);
                        m_content.RemoveAt(i);
                        Rect _rect = NodeRect;
                        _rect.height -= (10 + MULTIPLE_CONTENT_HEIGHT);
                        NodeRect = _rect;
                    }
                    m_content = new List<string>();
                    m_content.Add("SAMPLE CONTENT");
                    _r.height = INITIAL_RECT_HEIGHT;
                    break;
                case ConversationNodeType.MultipleChoices:
                    _r.height = 55 + MULTIPLE_CONTENT_HEIGHT + 30 ; 
                    break;
                default:
                    break;
            }
            NodeRect = _r; 
        }
    }
    #endregion

    #region Constructor 
    public ConversationNode(Vector2 _position, float _width, float _height, GUIStyle _nodeStyle, GUIStyle _selectedNodeStyle, GUIStyle _inPointStyle, GUIStyle _outPointStyle, Action<ConnectionPoint> _onClickInPoint, Action<ConnectionPoint> _onClickOutPoint, Action<Node> _onRemoveNodeAction) 
         : base( _position,  _width,  _height,  _nodeStyle,  _selectedNodeStyle,  _inPointStyle,  _outPointStyle, _onClickInPoint, _onClickOutPoint, _onRemoveNodeAction)
    {
        m_title = "Speaking Character";
        m_content = new List<string>();
        m_content.Add("SAMPLE CONTENT"); 
    }
    #endregion

    #region Methods
    public override void Draw()
    {
        GUI.Box(NodeRect, NodeTitle, m_nodeStyle);
        InPoint.Draw(NodeRect);
        switch (m_nodeType)
        {
            case ConversationNodeType.Basic:
                DrawBasicNode();
                break;
            case ConversationNodeType.MultipleChoices:
                DrawMultipleChoicesNode(); 
                break;
            default:
                break;
        }
    }

    private void DrawBasicNode()
    {
        Rect _r = new Rect(NodeRect.position.x + 10, NodeRect.position.y + 15, NodeRect.width - 20, TITLE_HEIGHT);
        m_title = EditorGUI.TextField(_r, new GUIContent("Character Speaking"), m_title);
        _r = new Rect(NodeRect.position.x + 10, NodeRect.position.y + 40, NodeRect.width - 20, BASIC_CONTENT_HEIGHT);
        m_content[0] = EditorGUI.TextArea(_r, m_content[0]);
        OutPoints.ForEach(p => p.Draw(NodeRect));
    }

    private void DrawMultipleChoicesNode()
    {
        Rect _r = new Rect(NodeRect.position.x + 10, NodeRect.position.y + 15, NodeRect.width - 20, 20);
        m_title = EditorGUI.TextField(_r, new GUIContent("Character Speaking"), m_title);
        for (int i = 0; i < m_content.Count; i++)
        {
            _r = new Rect(NodeRect.position.x + 30, NodeRect.position.y + 40 + (i * (MULTIPLE_CONTENT_HEIGHT + 10)), NodeRect.width - 50, MULTIPLE_CONTENT_HEIGHT); 
            m_content[i] = EditorGUI.TextArea(_r, m_content[i]);
            OutPoints[i].Draw(_r);
            _r = new Rect(NodeRect.position.x + 10, NodeRect.position.y + 40 + (i * (MULTIPLE_CONTENT_HEIGHT + 10)) + (MULTIPLE_CONTENT_HEIGHT / 2) - 10, 20, 20);
            if(GUI.Button(_r, "-"))
            {
                OutPoints[i].ClearPoint(); 
                OutPoints[i] = null; 
                OutPoints.RemoveAt(i);
                m_content.RemoveAt(i);
                Rect _rect = NodeRect;
                _rect.height -= (10 + MULTIPLE_CONTENT_HEIGHT);
                NodeRect = _rect;
                return; 
            }
        }
        _r = new Rect(NodeRect.position.x + NodeRect.width - 30, NodeRect.position.y + NodeRect.height - 30, 20, 20);
        if (GUI.Button(_r, "+"))
        {
            m_content.Add("");
            OutPoints.Add(new ConnectionPoint(this, ConnectionPointType.Out, m_outPointStyle, m_onClickOutPoint));
            Rect _rect = NodeRect;
            _rect.height += (10 + MULTIPLE_CONTENT_HEIGHT);
            NodeRect = _rect; 
        }
    }

    protected override void ProcessContextMenu()
    {
        GenericMenu _genericMenu = new GenericMenu();
        _genericMenu.AddItem(new GUIContent("Remove Node"), false, OnClickRemoveNode);
        _genericMenu.AddSeparator("");
        switch (m_nodeType)
        {
            case ConversationNodeType.Basic:
                _genericMenu.AddDisabledItem(new GUIContent("Set Node as Basic Node"));
                _genericMenu.AddItem(new GUIContent("Set Node as Multiple choices Node"), false, () => NodeType = ConversationNodeType.MultipleChoices);
                break;
            case ConversationNodeType.MultipleChoices:
                _genericMenu.AddItem(new GUIContent("Set Node as Basic Node"), false, () => NodeType = ConversationNodeType.Basic);
                _genericMenu.AddDisabledItem(new GUIContent("Set Node as Multiple choices Node"));
                break;
            default:
                break;
        }
        _genericMenu.ShowAsContext();
    } 
    #endregion
}

public enum ConversationNodeType
{
    Basic, 
    MultipleChoices 
}
