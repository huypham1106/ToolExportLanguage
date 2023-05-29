using GoogleSheetsToUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleSheetsToUnity.Legacy;
using MiniJSON;
using System;
using TMPro;
using SimpleFileBrowser;
using System.IO;
using UnityEngine.Networking;
using SheetsSample;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System.Threading;
using Google.Apis.Util.Store;
using Google.Apis.Services;
using Google.Apis.Auth.OAuth2.Responses;
using Data = Google.Apis.Sheets.v4.Data;
using OfficeOpenXml;
using System.Text;
using System.Globalization;

public class Controller : MonoBehaviour
{
    public static Controller Instance;
    [HideInInspector]
    public GoogleSheetsToUnityConfig config;
    public GameObject PanelLoading;
    public Transform TransToast;
    public GameObject ToastPre;
    public InputField IpUrl;
    public TextMeshProUGUI TxtMeshShow;
    //public Text TxtMeshShow;
    public Text TxtIsLegacy;
    public Dropdown dropSelectProject;
    public SystemErrController SystemErr;
    public Button BtnShowError;
    public Button BtnSplitAndSave;
    List<string> languageList = new List<string>();
    [SerializeField] private Language language;
    [SerializeField] private GameObject goLanguage;
    [TextArea]
    public string ContentError;
    private Dictionary<string, object> dictLanguage;
    private Dictionary<string, object> dictHelp;
    private Dictionary<string, object> dictSkill;
    private Dictionary<string, object> dictTooltip;
    private List<string> listLanguage = new List<string> { "in", "th", "ru", "de", "ja", "zh-cn", "zh-tw", "pt-br", "pt", "fr-ca", "fr", "ko", "es", "it", "tr", "vi", "en", "vi" };
    private List<string> listkeyInGG = new List<string> { "malay", "thai", "russian", "german", "japanese", "simplified", "traditional", "(brazil)", "portuguese", "(canada)", "french", "korean", "spanish", "italian", "turkish", "vi", "en", "vn" };
    private string[] spearator = { "[-]" };
    private string keyCurrent = "";
    private string keyTemp = "";
    private Dictionary<string, object> languageCurrent;
    private Dictionary<string, object> helpCurrent;
    private Dictionary<string, object> skillCurrent;
    private Dictionary<string, object> tooltipCurrent;
    private string pathfoldersave = "C:\\";
    static string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
    static string ApplicationName = "Google Sheets API .NET Quickstart";
    private IList<IList<System.Object>> valuesSheet;
    private String spreadsheetId = "";
    private string txtPathFolderAsset = "";
    private string pathfilesampleexcel = "";
    List<string> listPM;
    List<string> listValuePM;
    private string contentSave = "";
    private string nameFile = "";
    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        dropSelectProject.onValueChanged.AddListener(delegate {
            dropdownValueChanged(dropSelectProject);
        });
        dropSelectProject.ClearOptions();
        listPM = new List<string>();
        listValuePM = new List<string>();
        pathfilesampleexcel = Application.dataPath + "/StreamingAssets/samplefile.xlsx";
        txtPathFolderAsset = Application.dataPath + "/StreamingAssets/pathfoldersave.txt";
        if(File.Exists(txtPathFolderAsset))
        {
            string content = File.ReadAllText(txtPathFolderAsset);
            pathfoldersave = content;
        }
        IpUrl.text = "https://docs.google.com/spreadsheets/d/1SSooVyOSwdWbY2Kj-EibQNpNiXqccUc5Prmz_T5aYLU/edit#gid=172215109";
        readContentExl();
        UpdateDropDown();
        dropSelectProject.value = 1;
    }
    private void dropdownValueChanged(Dropdown dropchange)
    {
        int idx = dropchange.value;
        IpUrl.text = listValuePM[idx];
    }    
    void UpdateDropDown()
    {
        dropSelectProject.AddOptions(listPM);
    }    
    void OnApplicationQuit()
    {
        if (File.Exists(txtPathFolderAsset))
        {
            File.WriteAllText(txtPathFolderAsset, pathfoldersave);
        }
        Debug.Log("Application ending after " + Time.time + " seconds");
    }
    //
    private void cleanMemory()
    {

    }
    private string getSpreadsheetId()
    {
        string uriIp = IpUrl.text;
        if (!uriIp.Contains("/d/")||!uriIp.Contains("/edit"))
        {
            showToast("Url Invalid");
            Debug.Log("khong hop le");
        }
        else
        {
            int idxfirst = uriIp.IndexOf("/d/") + 3;
            int idxlast = uriIp.IndexOf("/edit");
            string id = uriIp.Substring(idxfirst, (idxlast - idxfirst));
            return id;
        }
        return "";
    }
    private void showToast(string content)
    {
        GameObject objToast = GameObject.Instantiate(ToastPre, TransToast);
        objToast.GetComponent<InforToast>().txtContent.text = content;
        objToast.SetActive(true);
        Destroy(objToast, 2);
    }
    private void excuteReadGGSheet(Dictionary<string, object> dictCurrent, Dictionary<string, object> dictTotal, string sheetName)
    {
        UserCredential credential;
        TextAsset txt = (TextAsset)Resources.Load("config/credentials", typeof(TextAsset));
        Stream stream = new MemoryStream(txt.bytes);
        string credPath = "token.json";
        credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
            GoogleClientSecrets.Load(stream).Secrets,
            Scopes,
            "user",
            CancellationToken.None,
            new FileDataStore(credPath, true)).Result;

        // Create Google Sheets API service.
        var service = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName,
        });

        // Define request parameters.
        String range = sheetName;
        SpreadsheetsResource.ValuesResource.GetRequest request =
                service.Spreadsheets.Values.Get(spreadsheetId, range);

        //System.GC.Collect();

        // Prints the names and majors of students in a sample spreadsheet:
        // https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms/edit
        try
        {
            Google.Apis.Sheets.v4.Data.ValueRange response = request.Execute();
            valuesSheet = response.Values;
        //
        //
        Data.ClearValuesRequest requestBody = new Data.ClearValuesRequest();

            //
            System.GC.Collect();
            //Debug.Log(valuesSheet.Count);
            if (valuesSheet != null && valuesSheet.Count > 0)
            {
                if (sheetName == "help_config_1")
                {
                    getInforSpreadHelpNew();
                }
                else
                {
                    getInforSpreadNew(dictCurrent, dictTotal);
                }
            }
            else
            {
                PanelLoading.SetActive(false);
                showToast("No data found");
            }
            response = null;
        }
        catch (Exception ex)
        {
            PanelLoading.SetActive(false);
            showToast("Url Invalid");
            Debug.Log("Invalid");
        }
        service.Dispose();
        stream.Dispose();
        request = null;
        stream = null;
    }
    private void getInforSpreadNew(Dictionary<string, object> dictCurrent, Dictionary<string, object> dictTotal)
    {
        ContentError = "";
        List<string> listKeyInvalid = new List<string>();
        bool error = false;
        var rowtitle = valuesSheet[0];
        int maxrow = valuesSheet.Count;
        for (int i = 1; i < maxrow; i++)
        {
            keyTemp = "";
            bool haskey = false;
            dictCurrent = new Dictionary<string, object>();
            //GoogleSheetsToUnity.Legacy.RowData rData = data.rows[i];
            var rData = valuesSheet[i];
            //int maxCelli = rData.Count;
            string titleNext = "";
            int idxStart = getIdxOfKeyField(rowtitle);
            int maxCelli = rowtitle.Count;
            for (int j = idxStart; j < maxCelli; j++)
            {
                if(haskey && j>= rowtitle.Count)
                {
                    continue;
                }
                if (j + 1 < rowtitle.Count-1)
                {
                    titleNext = rowtitle[j + 1].ToString();
                }
                else
                {
                    titleNext = "";
                }
                string value = "";
                if(j< rData.Count)
                {
                    value = rData[j].ToString();
                }
                distributeDataLanguage(rowtitle[j].ToString(), titleNext, value, dictCurrent);
                if (rowtitle[j].ToString().Contains("key"))
                {
                    haskey = true;
                }
                //Debug.Log(rData.cells[j].cellColumTitle);
            }
            if (!dictCurrent.ContainsKey("en"))
            {
                dictCurrent.Add("en", "");
            }
            if (!haskey)
            {
                keyCurrent = keyTemp;
            }
            if (keyCurrent == "")
            {
                continue;
            }
            if(dictTotal.ContainsKey(keyCurrent))
            {
                error = true;
                PanelLoading.SetActive(false);
                listKeyInvalid.Add(keyCurrent);
                showToast("Dup key: " + keyCurrent);
            }
            else
            {
                dictTotal.Add(keyCurrent, dictCurrent);
            }
        }
        PanelLoading.SetActive(false);
        if (!error)
        {
            string contentLanguage = Json.Serialize(dictTotal);
            contentSave = contentLanguage;

          //  Debug.Log(DecodeEncodedNonAsciiCharacters(contentLanguage));
          //  Debug.Log("++++" +contentLanguage);

            TxtMeshShow.text = DecodeEncodedNonAsciiCharacters(contentLanguage);          
            BtnShowError.interactable = false;
            valuesSheet = null;
            dictCurrent = null;
            dictTotal = null;
            System.GC.Collect();
        }
        else
        {
            BtnShowError.interactable = true;
            string contenterr = ""; 
            foreach(string keydup in listKeyInvalid)
            {
                contenterr += keydup;
                contenterr += "\n";
            }
            //contenterr.Replace("\\n", "\n");
            SystemErrController.Instance.ShowError("Dupplicate Key!!!", contenterr);
        }    

        
    }
    static string EncodeNonAsciiCharacters(string value)
    {
        StringBuilder sb = new StringBuilder();
        foreach (char c in value)
        {
            if (c > 127)
            {
                // This character is too big for ASCII
                string encodedValue = "\\u" + ((int)c).ToString("x4");
                sb.Append(encodedValue);
            }
            else
            {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }

    static string DecodeEncodedNonAsciiCharacters(string value)
    {
        Debug.Log(value.Length);
        return System.Text.RegularExpressions.Regex.Replace(
            value,
            @"\\u(?<Value>[a-zA-Z0-9]{4})",
            m => {
                return ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString();
            });
    }

    private int getIdxOfKeyField(IList<object> listTitle)
    {
        for(int i = 0;i<listTitle.Count;i++)
        {
            string title = listTitle[i].ToString().ToLower();
            if (title.Contains("vi")|| title.Contains("vn"))
            {
                return i-1;
            }
        }
        return 0;
    }
    private void getInforSpreadHelpNew()
    {
        var rowtitle = valuesSheet[0];
        int maxrow = valuesSheet.Count;
        for (int i = 1; i < maxrow; i++)
        {
            helpCurrent = new Dictionary<string, object>();
            var rData = valuesSheet[i];
            int maxCelli = rData.Count;
            for (int j = 0; j < maxCelli; j++)
            {
                distributeDatahelp(rowtitle[j].ToString(), rData[j].ToString(), helpCurrent);
                //Debug.Log(rData.cells[j].cellColumTitle);
            }
            if (keyCurrent == "")
            {
                continue;
            }
            if (!helpCurrent.ContainsKey("en"))
            {
                helpCurrent.Add("en", "");
            }
            dictHelp.Add(keyCurrent, helpCurrent);
        }
        string contentLanguage = Json.Serialize(dictHelp);
        contentSave = contentLanguage;
        TxtMeshShow.text = contentLanguage;
        PanelLoading.SetActive(false);
    }
    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            //webRequest.SetRequestHeader("Authorization", "AIzaSyBy9By2bKccag9JyTejfucCD9QEX-U8CA8");
            webRequest.SetRequestHeader("API_KEY", "AIzaSyBy9By2bKccag9JyTejfucCD9QEX-U8CA8");
            webRequest.SetRequestHeader("key", "AIzaSyBy9By2bKccag9JyTejfucCD9QEX-U8CA8");
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();
            Debug.Log(webRequest.downloadHandler.text);
            
        }
    }
    public void ShowTestLanguage()
    {
        TxtMeshShow.text = "";
        spreadsheetId = getSpreadsheetId();
        Debug.Log(spreadsheetId);
        if(spreadsheetId!="")
        {
            PanelLoading.SetActive(true);
            dictLanguage = new Dictionary<string, object>();
            nameFile = "language_config";
            StartCoroutine(delayTimeLang());
        }
    }
    private IEnumerator delayTimeLang()
    {
        yield return new WaitForSeconds(0.1f);
        excuteReadGGSheet(languageCurrent, dictLanguage, "language_config_1");
    }
    public void ShowTestSkill()
    {
        TxtMeshShow.text = "";
        spreadsheetId = getSpreadsheetId();
        if (spreadsheetId != "")
        {
            PanelLoading.SetActive(true);
            dictSkill = new Dictionary<string, object>();
            nameFile = "skill_detail";
            StartCoroutine(delayTimeSkill());
        }
    }
    private IEnumerator delayTimeSkill()
    {
        yield return new WaitForSeconds(0.1f);
        excuteReadGGSheet(languageCurrent, dictSkill, "skill_detail_1");
    }
    public void ShowTestTooltip()
    {
        TxtMeshShow.text = "";
        spreadsheetId = getSpreadsheetId();
        if (spreadsheetId != "")
        {
            PanelLoading.SetActive(true);
            dictTooltip = new Dictionary<string, object>();
            nameFile = "tooltip_config";
            StartCoroutine(delayTimeToolTip());
        }
    }
    private IEnumerator delayTimeToolTip()
    {
        yield return new WaitForSeconds(0.1f);
        excuteReadGGSheet(languageCurrent, dictTooltip, "tooltip_config_1");
    }
    public void ShowTestHelp()
    {
        TxtMeshShow.text = "";
        spreadsheetId = getSpreadsheetId();
        if (spreadsheetId != "")
        {
            PanelLoading.SetActive(true);
            dictHelp = new Dictionary<string, object>();
            nameFile = "help_config";
            StartCoroutine(delayTimeHelp());
        }
    }
    private IEnumerator delayTimeHelp()
    {
        yield return new WaitForSeconds(0.1f);
        excuteReadGGSheet(languageCurrent, dictTooltip, "help_config_1");
    }
    private string getKeyLanguageBykeyGG(string title)
    {
        foreach(string language in listkeyInGG)
        {
            if(title.Contains(language))
            {
                return listLanguage[listkeyInGG.IndexOf(language)];
            }
        }
        return "";
    }
    private void distributeDataLanguage(string title, string titleNext, string value, Dictionary<string, object> dictCurrent)
    {
        title = title.ToLower();
        titleNext = titleNext.ToLower();
        value = value.Replace(" ", "");
        value = value.Replace("​", "");
        value = value.Replace("\\n", "\n");
        value = value.Replace("\\r", "\r");
        if ((titleNext.Contains("vi")|| titleNext.Contains("vn")) && (keyTemp==""))
        {
            keyTemp = value;
        }
        if (title.Contains("key"))
        {
            keyCurrent = value;
        }
        else
        {
            string languageType = getKeyLanguageBykeyGG(title);
            if(languageType!="")
            {
                if (!dictCurrent.ContainsKey(languageType))
                {
                    if(value!="")
                    {
                        dictCurrent.Add(languageType, value);
                    }
                }
                else
                {
                    dictCurrent[languageType] = value;
                }
            }
        }
    }

    private void distributeDatahelp(string title, string value, Dictionary<string, object> dictCurrent)
    {
        title = title.ToLower();
        value = value.Replace(" ", "");
        value = value.Replace("​", "");
        value = value.Replace("\\n", "\n");
        value = value.Replace("\\r", "\r");
        if (title.Contains("key"))
        {
            keyCurrent = value;
        }
        else
        {
            string languageType = getKeyLanguageBykeyGG(title);
            if (languageType != "" && value != "")
            {
                string[] listStr = value.Split(spearator,10, StringSplitOptions.RemoveEmptyEntries);
                if (!dictCurrent.ContainsKey(languageType))
                {
                    dictCurrent.Add(languageType, listStr);
                }
            }
        }
    }
    private void getInforSpreadHelp()
    {
        string adu = "";
        SpreadSheetManager manager = new SpreadSheetManager();
        GS2U_Worksheet worksheet = manager.LoadSpreadSheet("(Language)Monsters & Puzzles: Match 3 War").LoadWorkSheet("help_config_1");
        WorksheetData data = worksheet.LoadAllWorksheetInformation();
        Debug.Log(data.rows.Count);
        for (int i=0;i<data.rows.Count;i++)
        {
            helpCurrent = new Dictionary<string, object>();
            GoogleSheetsToUnity.Legacy.RowData rData = data.rows[i];
            for(int j=0;j< rData.cells.Count;j++)
            {
                distributeDatahelp(rData.cells[j].cellColumTitle, rData.cells[j].value, helpCurrent);
            }
            if(keyCurrent=="")
            {
                continue;
            }
            if (!helpCurrent.ContainsKey("en"))
            {
                helpCurrent.Add("en", "");
            }
            dictHelp.Add(keyCurrent, helpCurrent);
           
            adu += rData.cells[1].value;

        }
        string contentLanguage = Json.Serialize(dictHelp);
        TxtMeshShow.text = contentLanguage;
        PanelLoading.SetActive(false);
    }
    private void getInforSpread(Dictionary<string, object> dictCurrent, Dictionary<string, object> dictTotal, string sheet)
    {
        SpreadSheetManager manager = new SpreadSheetManager();
        GS2U_Worksheet worksheet = manager.LoadSpreadSheet("(Language)Monsters & Puzzles: Match 3 War").LoadWorkSheet(sheet);
        WorksheetData data = worksheet.LoadAllWorksheetInformation();
        int maxrow = data.rows.Count;
        for (int i = 0; i < maxrow; i++)
        {
            keyTemp = "";
            bool haskey = false;
            dictCurrent = new Dictionary<string, object>();
            GoogleSheetsToUnity.Legacy.RowData rData = data.rows[i];
            int maxCelli = rData.cells.Count;
            for (int j = 0; j < maxCelli; j++)
            {
                //distributeDataLanguage(rData.cells[j].cellColumTitle, rData.cells[j].value, dictCurrent);
                if(rData.cells[j].cellColumTitle.Contains("key"))
                {
                    haskey = true;
                }
            }
            if (!dictCurrent.ContainsKey("en"))
            {
                dictCurrent.Add("en", "");
            }
            if(!haskey)
            {
                keyCurrent = keyTemp;
            }
            if (keyCurrent=="")
            {
                continue;
            }
            dictTotal.Add(keyCurrent, dictCurrent);
        }
        string contentLanguage = Json.Serialize(dictTotal);
        Debug.Log(contentLanguage);
        TxtMeshShow.text = contentLanguage;
        Debug.Log("Set false");
        PanelLoading.SetActive(false);
    }

    public void OnClickSplitAndSave()
    {
        clearTranform(goLanguage.transform);
        string testShow = TxtMeshShow.text;

        var data = MiniJSON.Json.Deserialize(testShow) as Dictionary<string, object>;
        languageList.Clear();
        countLanguageExist(data, languageList);


         // lấy file tiếngh anh
         // lấy all key ngôn nguwex, : en, vi , fr,....
        Dictionary<string, object> dictLanguageEnglish = new Dictionary<string, object>();

            foreach (string key in data.Keys)
            {
                var languageData = data.GetDictionary(key);
                //  foreach (string keyLanguage in languageData.Keys)

                    if (languageData.ContainsKey( "en"))
                    {
                        dictLanguageEnglish.Add(key, languageData["en"]);
                    }
                    else
                    {
                     dictLanguageEnglish.Add(key, "");
            }        
            }




            Dictionary<string, object> dictTotal = new Dictionary<string, object>();
        for (int i = 0; i < languageList.Count; i++)
        {
            dictTotal.Clear();
            foreach(string key in data.Keys)
            {
                var languageData = data.GetDictionary(key);
                //  foreach (string keyLanguage in languageData.Keys)

                    if (languageData.ContainsKey(languageList[i]))
                    {
                        //if (keyLanguage == languageList[i])
                        //{
                            dictTotal.Add(key, languageData[languageList[i]]);
                          //  break;
                        //}
                    }
                    else
                    {
                        dictTotal.Add(key, dictLanguageEnglish[key]);
                      //  break;
                    }    
                
            }
            string contentLanguage = Json.Serialize(dictTotal);
            var languageIns = Instantiate(language, goLanguage.transform,false);
            //languageIns.transform.SetParent(goLanguage.transform, false);
            languageIns.initData(languageList[i], DecodeEncodedNonAsciiCharacters( contentLanguage));
            languageIns.name = languageList[i];


        }

        SaveAsFileSplitAndSave();



    }  
    
    private void countLanguageExist(Dictionary<string, object>  data, List<string> languageList)
    {
        foreach (string key in data.Keys)
        {
            var languageData = data.GetDictionary(key);
            foreach (string keyLanguage in languageData.Keys)
            {
                if (!languageList.Contains(keyLanguage))
                {
                    languageList.Add(keyLanguage);
                    Debug.Log(keyLanguage);
                }
            }
        }
    }

    public void SaveAsFileSplitAndSave()
    {
        FileBrowser.ShowSaveDialog((paths) => onSuccessSlitAndSave(paths), null, FileBrowser.PickMode.Files, false, pathfoldersave, nameFile+"_vi", "Save As", "Save");


    }
    private void onSuccessSlitAndSave(string[] paths)
    {
        if (languageList.Count > 0)
        {
            Language[] languageObjectList = goLanguage.transform.GetComponentsInChildren<Language>();

            for (int i = 0; i < languageObjectList.Length; i++)
            {
                string[] pathTemp = paths[0].Split('\\');
                string path = "";
                for(int j=0; j<pathTemp.Length-1; j++)
                {
                    if(path=="")
                    {
                        path = pathTemp[j];
                    }    
                    else
                    path += "\\" + pathTemp[j];
                }
                path = path + "\\"+ nameFile +"_" + languageObjectList[i].nameLanguage;
                Debug.Log(path);

                if (path.Length > 0)
                {
                    pathfoldersave = Path.GetDirectoryName(paths[0]);
                    string content = languageObjectList[i].content;
                    byte[] bytes = System.Text.Encoding.UTF8.GetBytes(content);

                    //Debug.Log(paths[0]);
                    FileBrowserHelpers.WriteBytesToFile(path, bytes);
                }
            }
        }
    }
   

    public void SaveAsFile()
    {
        FileBrowser.ShowSaveDialog((paths) => onSuccess(paths), null, FileBrowser.PickMode.Files, false, pathfoldersave, nameFile, "Save As", "Save");
    }
    private void onSuccess(string[] paths)
    {
        if (paths.Length>0)
        {
            pathfoldersave = Path.GetDirectoryName(paths[0]);
            string content = TxtMeshShow.text;
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(content);

            FileBrowserHelpers.WriteBytesToFile(paths[0], bytes);
        }
    }
    private void getPathfolder()
    {
    }
    public void CopyLanguage()
    {
        string contentLanguageCopy = Json.Serialize(dictLanguage);
        TextEditor textEditor = new TextEditor();
        textEditor.text = contentLanguageCopy;
        textEditor.SelectAll();
        textEditor.Copy();
    }
    public void CopyHelp()
    {
        string contenthelpCopy = Json.Serialize(dictHelp);
        TextEditor textEditor = new TextEditor();
        textEditor.text = contenthelpCopy;
        textEditor.SelectAll();
        textEditor.Copy();
    }
    public void CopytoClipboard()
    {
        //TextEditor textEditor = new TextEditor();
        //textEditor.text = contentSave;
        //textEditor.SelectAll();
        //textEditor.Copy();
        GUIUtility.systemCopyBuffer = DecodeEncodedNonAsciiCharacters(contentSave);
        showToast("Copy Success");
    }
    public void CloseApp()
    {
        Application.Quit(0);
    }
    //
    private void readContentExl()
    {
        var package = new ExcelPackage(new FileInfo(pathfilesampleexcel));
        if (package.Workbook.Worksheets.Count <= 0)
        {
            //ShowToast("Invalid Path");
            return;
        }
        ExcelWorksheet wordsheet = package.Workbook.Worksheets[0];
        try
        {
            Dictionary<int, List<List<object>>> dataImport = new Dictionary<int, List<List<object>>>();
            for (int i = wordsheet.Dimension.Start.Row + 1; i <= wordsheet.Dimension.End.Row; i++)
            {
                listPM.Add(wordsheet.Cells[i, 1].Value.ToString());
                listValuePM.Add(wordsheet.Cells[i, 2].Value.ToString());
                Debug.Log(wordsheet.Cells[i, 1].Value.ToString());
                Debug.Log(wordsheet.Cells[i, 2].Value.ToString());
                //int numFloor = int.Parse(wordsheet.Cells[i, 1].Value.ToString());
                //numFloor -= 1;
                //string strContent = wordsheet.Cells[i, 2].Value.ToString();
                //List<List<object>> dataFloor = convertStringToList(strContent);
                //dataImport.Add(numFloor, dataFloor);
            }

        }
        catch (Exception ex)
        {
            showToast("Invalid Content Excel");
        }

    }
    private string removesapce(string text)
    {
        string textConvert = text.Replace(" ", string.Empty);
        return textConvert;
    }

    private void clearTranform(Transform transform)
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}
