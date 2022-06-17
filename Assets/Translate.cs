using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Translate : MonoBehaviour
{
    public TMP_InputField Input;
    public TMP_InputField Output;
    public string sentance;
    public List<char> ignoredChars;
    public TextAsset textJson;
    public float timer;
    public bool hastusks;

    [System.Serializable]
    public struct OrcishConversions
    {
        public string From;
        public string To;
    }
    [System.Serializable]
    public class OrcishSoundSet
    {
        public OrcishConversions[] orcishConversion;
    }
    public OrcishSoundSet orcishConversion = new OrcishSoundSet();

    [System.Serializable]
    public class DictionaryOfLanguage
    {
        public string word;
        public string translateFrom;
    }
    
    [System.Serializable]
    public class DictionaryList
    {
        public DictionaryOfLanguage[] dictionaryOfLanguage;
    }

    public DictionaryList LanguageDictionary = new DictionaryList();
    int currentTextSize;

    // Start is called before the first frame update
    void Start()
    {
        ReadCurrentJson();
    }

    void ReadCurrentJson()
    {
        LanguageDictionary = JsonUtility.FromJson<DictionaryList>(textJson.text);
        
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer<-1)
        {
            timer = -1;
        }
    }

    

    public void translateText()
    {
        if (Input != null)
        {
            sentance = Input.text;
            
           
        }
        string[] words = sentance.Split(' ');
        

        if (words.Length != currentTextSize && timer <= 0)
        {
            timer = 0.1f;
            string translatedSentance = FullTranslation(words);
            string OutputWords = CorrectPreAndSuffixes(translatedSentance);
            Debug.Log(OutputWords);
            Output.text = OutputWords;
        }
        currentTextSize = words.Length;




    }

    string CorrectPreAndSuffixes(string sentance)
    {
        //splits sentance based off of a space
        string[] words = sentance.Split(' ');
        for (int i = 0; i < words.Length; i++)
        {
            int index = words[i].IndexOf('-');
           
            if (index == 0 && i > 0 && i < words.Length - 1 && words[i].Length > 1)
            {
                words[i] = words[i].Substring(1, words[i].Length-1);
                words[i + 1] =  words[i + 1]+ words[i];
                words[i] = "";
            }
            else if(index==0 && words[i].Length > 1)
            {
                words[i] = words[i].Substring(1, words[i].Length-1);             
            }
            if (index == words[i].Length-1 && i > 0 && i < words.Length - 1 && words[i].Length > 0)
            {
                words[i] = words[i].Remove(words[i].Length - 1);
                words[i + 1] = words[i] + words[i+1];
                words[i] = "";
            }
            else if (index == words[i].Length-1 && words[i].Length > 0)
            {
                words[i] = words[i].Remove(words[i].Length - 1);
            }
            
        }
        string fullstring = "";
        foreach (string Word in words)
        {
            fullstring += Word + " ";
        }
        return fullstring;
       
    }

    



    string FullTranslation(string[] sentance)
    {
        string ConvertedSentance = "";
        if (sentance.Length>1)
        {
            sentance = PhraseTranslation(sentance);
        }
       

        foreach (string Word in sentance)
        {
            bool translated = false;
            foreach (var item in LanguageDictionary.dictionaryOfLanguage) 
            {
                
                string[] SpecificWords = item.translateFrom.Split(',');
                foreach (string WordCheck in SpecificWords)
                {
                    if (RemoveInvalidChars(WordCheck.ToUpper().Trim()) == RemoveInvalidChars(Word.ToUpper().Trim()))
                    {
                        ConvertedSentance += item.word + " ";
                        translated = true;
                        
                    }
                    if (translated)
                    {
                        break;
                    }
                   
                }
                if (translated)
                {
                    break;
                }
                
                
            }
            if (translated==false)
            {
                string newword = Word;
                if (hastusks)
                {
                    newword = AddTuskSounds(newword);
                }
              
                ConvertedSentance += newword + " ";
            }
        }
        return ConvertedSentance;
    }

    string AddTuskSounds(string word)
    {
        char[] wordSplit = word.ToCharArray();
        for (int i = 0; i < wordSplit.Length; i++)
        {
            for (int j = wordSplit.Length; j >= i; j--)
            {
                string NewWord = "";
                for (int x = i; x < j; x++)
                {
                    
                    NewWord += wordSplit[x].ToString() + " ";
                }
                foreach (var OrcishConvertedCharSet in orcishConversion.orcishConversion)
                {
                    if (NewWord.Trim().ToUpper() == OrcishConvertedCharSet.From.Trim().ToUpper())
                    {
                        word = word.Remove(i, j - i);
                        word = word.Insert(i, OrcishConvertedCharSet.To);
                        wordSplit = word.ToCharArray();
                        break;
                    }
                    
                }
                
                
            }
        }
        return word;
    }

    string RemoveInvalidChars(string word)
    {
        char[] wordChars = word.ToCharArray();
        string newWord = "";
        foreach (char c in wordChars)
        {
            if (ignoredChars.Contains(c))
            {
                newWord += "";
            }
            else
            {
                newWord += c;
            }
        }
        return newWord;
    }

    string[] PhraseTranslation(string[] sentance)
    {
        for (int i = 0; i < sentance.Length; i++)
        {
            
            bool translated = false;
            for (int j = sentance.Length; j >= i; j--)
            {
                string newsentance = "";
                for (int x = i; x < j; x++)
                {
                    newsentance += sentance[x] + " ";
                }
                foreach (var item in LanguageDictionary.dictionaryOfLanguage)
                {
                    string[] SpecificWords = item.translateFrom.Split(',');
                    foreach (string WordCheck in SpecificWords)
                    {
                        if (RemoveInvalidChars(WordCheck.ToUpper().Trim()) == RemoveInvalidChars(newsentance.ToUpper().Trim()))
                        {
                            
                            sentance[i] = item.word + " ";
                            translated = true;
                            for (int x = i+1; x < j; x++)
                            {
                                sentance[x] = "";
                            }

                        }
                        if (translated)
                        {
                            break;
                        }

                    }
                    if (translated)
                    {
                        break;
                    }


                }
                


                
            }
            if (translated == false)
            {
                sentance[i] = sentance[i];
            }
        }
       
        return sentance;
    }

    Dictionary<int, string> AddToDictionary(Dictionary<int, string> Dict, int value, string item)
    {
        if (Dict.ContainsKey(value))
        {
            Dict[value] = item;
        }
        else
        {
            Dict.Add(value, item);
        }
        return Dict;
        
        
    }
    
}
