using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.SpeechRecognition;
using NAudio.Wave;

namespace speech_to_text
{
    public class SpeechToTextConverter
    {
        public event EventHandler<Phrase> Progress; 

        public ConversionResult ConvertToText(string audioFileUrl, string languageCode, string subscriptionKey)
        {
            // download mp4 to local client
            Console.WriteLine("Downloading {0}...", audioFileUrl);
            var client = new WebClient();
            client.DownloadFile(new Uri(audioFileUrl), "movie.mp4");
            
            // convert file to wav
            Console.WriteLine("Converting to wav file...");
            using (var reader = new MediaFoundationReader("movie.mp4"))
            {
                WaveFileWriter.CreateWaveFile("soundtrack.wav", reader);
            }
            // get overall length of audio file
            var overallDuration = GetWavFileDuration("soundtrack.wav");

            // remove old split files
            Console.WriteLine("Cleaning up...");
            var files = Directory.GetFiles(".", "split*.wav", SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                File.Delete(file);
            }

            // split file into chunks based on silence detection with sox
            Console.WriteLine("Splitting audio file...");
            var startInfo = new ProcessStartInfo("sox-14.4.2\\sox.exe",
                "soundtrack.wav split.wav silence 0 1 0.33 1% : newfile : restart");
            startInfo.UseShellExecute = false;

            var p = Process.Start(startInfo);
            p.WaitForExit();

            // create result structure
            var result = new ConversionResult();
            result.DetectedPhrases = new List<DetectedPhrase>();
            result.Title = audioFileUrl;
            result.TotalLength = overallDuration.ToString("g");

            var currentPosition = TimeSpan.Zero;

            // enumerate all split files and do the speech to text
            files = Directory.GetFiles(".", "split*.wav", SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                var textPhrase = new SpeechEngineHelper().ConvertToText(file, languageCode, subscriptionKey);
                var splitLength = GetWavFileDuration(file);
                currentPosition += splitLength;
                if (textPhrase != null)
                {
                    var phrase = new DetectedPhrase();
                    phrase.Phrase = new List<Phrase>();
                    textPhrase.Offset = currentPosition.ToString("g");
                    phrase.Phrase.Add(textPhrase);
                    result.DetectedPhrases.Add(phrase);
                    if (Progress != null)
                    {
                        Progress(this, textPhrase);
                    }
                }
            }

            return result;
        }

        public static TimeSpan GetWavFileDuration(string fileName)
        {
            WaveFileReader wf = new WaveFileReader(fileName);
            return wf.TotalTime;
        }
    }
}
