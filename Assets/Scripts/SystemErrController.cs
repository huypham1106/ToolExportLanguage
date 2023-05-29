using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SystemErrController : MonoBehaviour
{
    public InputField IpContent;
    public Text TxtTitle;
    public static SystemErrController Instance;
    private void Awake()
    {
        Instance = this;
    }
    public void ShowError(string title, string content)
    {
        TxtTitle.text = title;
        IpContent.text = content;
    }    
}
