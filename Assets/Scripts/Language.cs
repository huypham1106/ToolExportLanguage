using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Language : MonoBehaviour
{

    public string nameLanguage;
    public string content;

    public void initData(string nameLanguage, string content)
    {
        this.nameLanguage = nameLanguage;
        this.content = content;
    }    
}
