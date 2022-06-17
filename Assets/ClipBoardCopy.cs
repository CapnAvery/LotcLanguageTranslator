using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ClipBoardCopy : MonoBehaviour
{
    public TMP_InputField OutputField;
    public TMP_InputField InputField;
    
    public void CopyToClipboard()
    {
        string text = OutputField.text;
        TextEditor te = new TextEditor();
        te.text = text;
        te.SelectAll();
        te.Copy();
    }

    public void PasteToClipboard()
    {
        TextEditor te = new TextEditor();
        te.multiline = true;
        te.Paste();
        InputField.text = te.text;
    }
}
