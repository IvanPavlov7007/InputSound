using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine;
using System.IO;

public class MicroListener : MonoBehaviour
{
    AudioSource source;

    public string songPath;

    void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    private void Start()
    {
        //  Using Micro:

        /*source.clip = Microphone.Start(null, true, 10, 44100);
        source.loop = true;
        while (!(Microphone.GetPosition(null) > 0)) { }
        source.Play();*/


        //using (UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(songPath, AudioType.WAV))
        //{
        //    yield return request.SendWebRequest();
        //    if (request.isNetworkError || request.isHttpError)
        //        throw new UnityException("file wasnt loaded");
        //    else
        //        source.clip = DownloadHandlerAudioClip.GetContent(request);
        //}
        
        //source.Play();
    }

    float result = 0f, maxResult, lastResult = 0f, velocity = 0f;
    public float dampTime;

    Coroutine startingClipRoutine;

    public Coroutine PlayClip(string path)
    {
        if (startingClipRoutine != null)
            StopCoroutine(startingClipRoutine);

        startingClipRoutine = StartCoroutine(playingClip(path));
        return startingClipRoutine;
    }

    IEnumerator playingClip(string path)
    {
        using (UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.WAV))
        {
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
                throw new UnityException("file wasnt loaded");
            else

            {
                source.clip = DownloadHandlerAudioClip.GetContent(request);
                source.Play();
            }
        }
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        float sum = 0f;
        foreach(float d in data)
        {
            sum += Mathf.Abs(d);
        }
        result = sum;
        if (result > maxResult)
            maxResult = result;
    }

    public Transform SoundScale, SoundRotation;

    float rotationVelocity;

    private void Update()
    {
        result = Mathf.SmoothDamp(lastResult, result, ref velocity, dampTime);

        float map = Mathf.InverseLerp(0.02f, 1000f, result);
        float height = map * 4 + 1f;
        rotationVelocity = map * 10f;
        SoundScale.localScale = new Vector3(1f, height,1f);
        SoundScale.position = new Vector3(0f, height * 0.5f, 0f);

        SoundRotation.Rotate(Vector3.up, rotationVelocity,Space.World);

        lastResult = result;
    }

    private void OnGUI()
    {
        GUI.Label(new Rect( Screen.width - 70, 50, 300, 30), result.ToString());
        GUI.Label(new Rect( Screen.width - 70, 80, 300, 30), maxResult.ToString());
    }
}
