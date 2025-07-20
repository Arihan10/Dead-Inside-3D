using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class AudioRecorder : MonoBehaviour
{
    public int sampleRate = 44100;
    private AudioClip micClip;
    private int micStartPos;
    private bool isRecording;

    void StartRecording()
    {
        if (Microphone.devices.Length > 0)
        {
            micClip = Microphone.Start(null, true, 300, sampleRate); // up to 5 minutes (300s)
            micStartPos = Microphone.GetPosition(null);
            isRecording = true;
        }
        else
        {
            Debug.LogWarning("No microphone found");
        }
    }
    
    AudioClip StopRecording()
    {
        int micEndPos = Microphone.GetPosition(null);
        Microphone.End(null);
        isRecording = false;

        int samplesRecorded = micEndPos - micStartPos;
        if (samplesRecorded < 0)
            samplesRecorded += micClip.samples;

        if (samplesRecorded <= 0)
        {
            Debug.LogWarning("No audio was recorded.");
            return null;
        }

        float[] rawData = new float[samplesRecorded * micClip.channels];
        micClip.GetData(rawData, micStartPos);

        AudioClip trimmedClip = AudioClip.Create("TrimmedRecording", samplesRecorded, micClip.channels, sampleRate, false);
        trimmedClip.SetData(rawData, 0);

        return trimmedClip;
    }

    /*
    void SaveRecording()
    {
        var recordedClip = StopRecording();
        bool saved = SavWav.Save("yeah",recordedClip);
    }
    */
    
    string SaveRecording(string name, Action<string> postAction)
    {
        //Debug.Log("Saving");
        var recordedClip = StopRecording();
        if (recordedClip == null) return null; // fail

        Debug.Log($"{recordedClip.samples} and {recordedClip.channels}");
        float[] samples = new float[recordedClip.samples * recordedClip.channels];
        recordedClip.GetData(samples, 0);

        int channels = recordedClip.channels;
        int frequency = recordedClip.frequency;

        string path;
        var filepath = Path.Combine(Application.persistentDataPath, name);
        Directory.CreateDirectory(Path.GetDirectoryName(filepath));
        
        if (!filepath.ToLower().EndsWith(".wav")) filepath += ".wav";
        
        // Task.Run(() => { }); ... perhaps do this later (?)
        // SavWav.Save(filepath, samples, frequency, channels);
        
        // on another thread ...
        Task.Run(() =>
        {
            SavWav.Save(filepath, samples, frequency, channels);
            MainThreadDispatcher.RunOnMainThread(() =>
            {
                Debug.Log("Hello vro");
                postAction(filepath);
            });
        });
        
        return filepath;
    }


    [SerializeField] private bool recording = false;
    private void Update()
    {
        if (Keyboard.current[Key.Space].wasPressedThisFrame)
        {
            recording = !recording;
            if (recording)
            {
                //Debug.Log("A");
                StartRecording();
            }
            else
            {
                //Debug.Log("B");
                SaveRecording("save", (string outputPath) =>
                {
                    if (outputPath != null)
                    {
                        var data = new { file_path = outputPath };
                        WebReq.inst.Post("stt", data);
                    }
                });
            }
        }
    }
}

