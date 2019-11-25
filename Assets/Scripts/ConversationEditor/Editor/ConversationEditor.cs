using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor; 

public class ConversationEditor : NodeBasedEditor
{
    #region Fields and Properties
     
    #endregion

    #region Methods 

    #region Static Method
    [MenuItem("Window/ConversationEditor")]
    public static void OpenConversationWindow()
    {
        ConversationEditor _window = GetWindow<ConversationEditor>();
        _window.titleContent = new GUIContent("Conversation Editor");
    }
    #endregion

    #region Original Methods
    protected override void ShowContextMenu(Vector2 _mousePosition)
    {
        GenericMenu _genericMenu = new GenericMenu();
        _genericMenu.AddItem(new GUIContent("Add Node"), false, () => AddNodeAtPosition(_mousePosition));
        _genericMenu.AddItem(new GUIContent("Add Condition"), false, () => AddConditionAtPosition(_mousePosition)); 
        _genericMenu.ShowAsContext();
    }

    protected override void AddNodeAtPosition(Vector2 _mousePosition)
    {
        if (m_nodes == null) m_nodes = new List<Node>();
        m_nodes.Add(new ConversationNode(_mousePosition, ConversationNode.INITIAL_RECT_WIDTH, ConversationNode.INITIAL_RECT_HEIGHT, m_defaultNodeStyle, m_selectedNodeStyle, m_inPointStyle, m_outPointStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode)); 
    }

    protected void AddConditionAtPosition(Vector2 _mousePosition)
    {
        if (m_nodes == null) m_nodes = new List<Node>();
        m_nodes.Add(new ConditionNode(_mousePosition, ConditionNode.INITIAL_RECT_WIDTH, ConditionNode.INITIAL_RECT_HEIGHT, m_defaultNodeStyle, m_selectedNodeStyle, m_inPointStyle, m_outPointStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode)); 
    }
    #endregion

    #region Unity Methods

    #endregion

    #endregion
}
