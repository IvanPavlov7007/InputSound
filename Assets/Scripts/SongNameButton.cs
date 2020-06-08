using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SongNameButton : MonoBehaviour
{
    SongSelectionManager manager;
    TextMeshProUGUI text;
    string songName;

    private void Start()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    bool textObjCreated = false;


    //TODO: New update: delete this, TextMeshProUGUI must work in Awake\Update(find out which one)
    private void Update()
    {
        if(text != null && text.text != null && !textObjCreated)
        {
            textObjCreated = true;
            text.text = songName;
        }
    }

    public void Initialize(SongSelectionManager manager)
    {
        this.manager = manager;
    }

    public void ResetName(string name)
    {
        this.songName = name;
        if (text != null && text.text != null)
            text.text = songName;
    }

    public void RespondSongName()
    {
        manager.SelectSong(songName);
    }
}
