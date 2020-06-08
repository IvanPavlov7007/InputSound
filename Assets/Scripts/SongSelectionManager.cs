using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class SongSelectionManager : MonoBehaviour
{
    public TMP_InputField inputField;
    public Scrollbar scrollbar;
    public RectTransform contentsTransform;

    public MicroListener soundGenerator;

    public GameObject songButtonPrefab;

    public List<SongNameButton> generatedButtons;

    SongsFilesManager fileManager;

    string saveEnvironmentVariableName = "SoundsPath";

    private void Awake()
    {
        fileManager = SongsFilesManager.instance;
        string lastPath = Environment.GetEnvironmentVariable(saveEnvironmentVariableName);
        inputField.text = lastPath;
        FolderPathChanged(lastPath);
        RefreshButtons();
    }

    private void Start()
    {
    }

    public void FolderPathChanged(string newPath)
    {
        fileManager.setCurrentFolder(newPath);
    }

    public void RefreshButtons()
    {
        var names = fileManager.getSongsNamesInCurrentFolder();

        if(names.Count > generatedButtons.Count)
        {
            addButtons(names.Count - generatedButtons.Count);
        }
        else if (names.Count < generatedButtons.Count)
        {
            hideButtons(generatedButtons.Count - names.Count);
        }

        for (int i = 0; i < names.Count; i++)
        {
            generatedButtons[i].gameObject.SetActive(true);
            generatedButtons[i].ResetName(names[i]);
        }
    }

    [SerializeField]
    float distanceBetweenButtons;
    void addButtons(int count)
    {
        int earlierButtons = generatedButtons.Count;
        for(int i = earlierButtons; i < count + earlierButtons; i++)
        {
            SongNameButton b = Instantiate(songButtonPrefab, contentsTransform).GetComponent<SongNameButton>();
            b.transform.localPosition = new Vector3(0f, distanceBetweenButtons * i,0f);
            generatedButtons.Add(b);
            b.Initialize(this);
        }
    }

    void hideButtons(int count)
    {
        int earlierLastIndex = generatedButtons.Count - 1;
        for (int i = earlierLastIndex; i > earlierLastIndex - count; i--)
        {
            generatedButtons[i].gameObject.SetActive(false);
        }
    }

    public void SelectSong(string name)
    {
        soundGenerator.PlayClip(fileManager.getSongPath(name));
    }

    private void OnApplicationQuit()
    {
        Environment.SetEnvironmentVariable(saveEnvironmentVariableName, fileManager.folderPath);
    }
}

public class SongsFilesManager
{
    static SongsFilesManager _instance;
    public static SongsFilesManager instance
    {
        get
        {
            if (_instance == null)
                _instance = new SongsFilesManager();
            return _instance;
        }
        private set { }
    }

    public string folderPath    {
        get;
        private set;
    }

    public void setCurrentFolder(string path)
    {
        folderPath = path.Replace(@"\","/");
    }

    public List<string> getSongsNamesInCurrentFolder()
    {
        List<string> result = new List<string>();
        var info = new DirectoryInfo(folderPath);
        foreach(var fileName in info.GetFiles())
        {
            if (fileName.Name.EndsWith(".wav"))
                result.Add(fileName.Name);
        }
        return result;
    }

    public string getSongPath(string songName)
    {
        return folderPath + "/" + songName;
    }
}
