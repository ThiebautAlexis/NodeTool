using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading; 
using UnityEngine;
using UnityEditor;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

public class ConversationEditor : NodeBasedEditor
{
    #region Fields and Properties
    public static string CredentialsDirectory { get { return "Assets/Editor/ConversationEditor"; } }
    public static string CredentialsPath { get { return Path.Combine(CredentialsDirectory, "credentials.json"); }  }

    private UserCredential credential = null; 
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

    private void LinkGoogleAccount()
    {
        if(!Directory.Exists(CredentialsDirectory))
        {
            Directory.CreateDirectory(CredentialsDirectory); 
        }
        if(!File.Exists(CredentialsPath))
        {
            EditorUtility.DisplayDialog("Can't find credentials.json", "Please download your credentials (following this link : https://developers.google.com/sheets/api/quickstart/dotnet) and add them at the root of the Unity Project", "Ok");
            //File.Open(CredentialsDirectory, FileMode.Open, FileAccess.Read); 
            return;
        }
        using (FileStream stream = new FileStream(CredentialsPath, FileMode.Open, FileAccess.Read))
        {

            string credentialPath = Path.Combine(CredentialsDirectory, "token.json");
            string[] scope = { SheetsService.Scope.SpreadsheetsReadonly }; 
            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.Load(stream).Secrets, 
                scope,
                "user", 
                CancellationToken.None,
                new FileDataStore(credentialPath, true)).Result;
        }                       
    }

    private void LoadSheet(string _sheetID)
    {
        Debug.Log("Allo?"); 
        if (_sheetID == string.Empty) return;
        if(credential == null)
        {
            EditorUtility.DisplayDialog("Can't find credentials", $"The credential can't be found. Make sure it is in the {CredentialsDirectory} folder", "Ok");
            return;
        }

        SheetsService service = new SheetsService(new BaseClientService.Initializer() { HttpClientInitializer = credential, ApplicationName = "Google Sheets API .NET Quickstart"});
        SpreadsheetsResource.ValuesResource.GetRequest request = service.Spreadsheets.Values.Get(_sheetID, "Class Data!A2:E");
        ValueRange responses = request.Execute();
        IList<IList<object>> values = responses.Values;

        if (values != null && values.Count > 0)
        {
            foreach(var row in values)
            {
                Debug.Log($"{row[0]} {row[4]}"); 
            }
        }
    }
    private void DrawButtons()
    {
        Rect r = new Rect(5, 5, 200, 50); 
        if(credential == null)
        {
            if (GUI.Button(r, "Link Google Account"))
            {
                LinkGoogleAccount();
            }
        }
        r.y += 75; 
        if (GUI.Button(r, "Load Spread Sheet"))
        {
            LoadSheet("1IwpRX9KW6n-klzuVkoIxXDJzK3q6qFQOCEq8DppJ5z4"); 
        }
    }
    #endregion

    #region Unity Methods
    protected override void OnEnable()
    {
        base.OnEnable(); 
    }

    protected override void OnGUI()
    {
        base.OnGUI();
        DrawButtons(); 
    }
    #endregion

    #endregion
}
