using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class QuickTextToJSONParser : MonoBehaviour
{
    public string stringToCopyFrom;
    // Start is called before the first frame update
    public List<string> DissallowedWords;
    public List<char> DissallowedCharacters;
    public Dictionary<string, string> DictionaryOfWords = new Dictionary<string, string>();
    public string[] FinalJSON;
    

    void Start()
    {

        /* 
          copied = RemoveNullLines(copied);
          DictionaryOfWords = SplitWordsAndTranslation(copied);
          DictionaryOfWords = RemovedDissallowedWordsAndCharacters(DictionaryOfWords);
          FinalJSON = ConvertToJsonFormat(DictionaryOfWords);
          FinalJSON = TrimStringArray(FinalJSON);
          CreateJSONFile(FinalJSON);*/
        string[] copied = GetLinesFromTxtFile();
        copied = ApplyJSONFormat(copied);
        CreateJSONFile(copied);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    string[] ApplyJSONFormat(string[] Stuff)
    {
        for (int i = 0; i < Stuff.Length; i++)
        {
            if (i % 2 == 0)
            {
                string line = Stuff[i];
                if (line[line.Length-1]!=',')
                {
                    Stuff[i] += ',';
                }
            }
           
        }
        return Stuff;
    }

    void CreateJSONFile(string[] Lines)
    {
        
        string path = Application.dataPath + "/JSON.txt";
        if (!File.Exists(path))
        {
            foreach (string item in Lines)
            {
                File.AppendAllText(path,item+"\n");
            }
        }
    }
    string[] TrimStringArray(string[] ArrayOfStrings)
    {
        for (int i = 0; i < ArrayOfStrings.Length; i++)
        {
            ArrayOfStrings[i] = ArrayOfStrings[i].Trim();
        }
        return ArrayOfStrings;
    }

    //this function takes a string and find the location of a bracket
    string FindBracketLocation(string Sentance)
    {
        int bracketLocation = 0;
        for (int i = 0; i < Sentance.Length; i++)
        {
            if (Sentance[i] == '{' || Sentance[i] == '(' || Sentance[i] == '[')
            {
                bracketLocation = i;
                break;
            }
        }
        if (bracketLocation>0)
        {
            return Sentance.Remove(Sentance.Length - (Sentance.Length - bracketLocation));
        }
        else
        {
            return Sentance;
        }
        


    }
    
    Dictionary<string, string> RemovedDissallowedWordsAndCharacters(Dictionary<string, string> Dict)
    {
        Dictionary<string, string> NewDict = new Dictionary<string, string>();
        foreach (KeyValuePair<string,string> Entry in Dict)
        {
            string ModifiedValue = FindBracketLocation(Entry.Value) ;
            
            string[] words = ModifiedValue.Split(' ');
            string newTranslation = "";
            foreach (string word in words)
            {
                char[] characters = word.ToCharArray();
                string NewWord = "";
                
                foreach (char character in characters)
                {
                    int DissallowedCharactersCounter = 0;
                    foreach (char DisallowedChar in DissallowedCharacters)
                    {
                        if (character.ToString().ToUpper() == DisallowedChar.ToString().ToUpper())
                        {
                            DissallowedCharactersCounter++;
                        }
                    }
                    if (DissallowedCharactersCounter==0)
                    {
                        NewWord += character.ToString();
                    }
                }
                int counter = 0;                
                foreach (string dissallowedWord in DissallowedWords)
                {
                    
                   
                    if (NewWord.ToUpper() == dissallowedWord.ToUpper())
                    {
                        counter++;
                    }

                }
                if (counter==0)
                {
                    newTranslation += NewWord + " ";
                }
              
            }
            NewDict.Add(Entry.Key, newTranslation);


        }
        return NewDict;
    }

    string[] ConvertToJsonFormat(Dictionary<string, string> Dict)
    {
        List<string> JsonValues = new List<string>();
        foreach (KeyValuePair<string,string> item in Dict)
        {
            /*
        {
			"word" : "utgal",
			"translateFrom" : "seventy,70"
		},
             
             
             
             */
            
            string JsonTop = "{";
            string JSONHead = "\"" + "word" + "\" : \"" + item.Key.Trim() + "\"";
            string JSONBody = "\"" + "translateFrom" + "\" : \"" + item.Value.Trim() + "\"";
            string JsonBottom = "},";
            JsonValues.Add(JsonTop);
            JsonValues.Add(JSONHead);
            JsonValues.Add(JSONBody);
            JsonValues.Add(JsonBottom);
         
        }
        return JsonValues.ToArray();
        
    }

    Dictionary<string, string> SplitWordsAndTranslation(string[] lines)
    {
       
        Dictionary<string, string> DictionaryOfNewlyFormedWords = new Dictionary<string, string>();        
        for (int i = 0; i < lines.Length; i++)
        {
            
            string[] words = lines[i].Split(' ');
            if (words.Length>1)
            {
                string TranslatedWordsCollection = "";
                for (int j = 1; j < words.Length; j++)
                {
                    TranslatedWordsCollection += words[j] +" ";
                }
                DictionaryOfNewlyFormedWords = AddNewWordTo(DictionaryOfNewlyFormedWords, words[0], TranslatedWordsCollection);
               
            }
            
            
        }
        return DictionaryOfNewlyFormedWords;



    }

    Dictionary<string, string> AddNewWordTo(Dictionary<string, string> Dict, string word, string translation)
    {
        if (Dict.ContainsKey(word))
        {
            Dict[word] += " "+ translation;
        }
        else
        {
            Dict.Add(word, translation);
        }
        return Dict;
    }
    

    string[] RemoveNullLines(string[] lines)
    {
        List<string> newLines = new List<string>();
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i] != "")
            {
                newLines.Add(lines[i]);
            }
        }
        return newLines.ToArray();
    }
    
    

    string[] GetLinesFromTxtFile()
    {
        string[] lines = System.IO.File.ReadAllLines(@"C:\ecosystem\LotcLanguageTranslator\Assets\ReadFromThisFile.txt");
        return lines;
    }
}
