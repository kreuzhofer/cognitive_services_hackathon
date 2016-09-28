using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.SpeechRecognition;
using NAudio.Wave;
using Newtonsoft.Json;

namespace speech_to_text
{
    public class Program
    {
        static int Main(string[] args)
        {
            if (args.Length < 4)
            {
                Console.WriteLine("speech_to_text example app. Converts video and audio source files to text.");
                Console.WriteLine("Usage: speech_to_text <inputfileUrl> <languagecode> <subscriptionkey> <outputfile>");
                return 0;
            }

            string inputFileName = args[0];
            string languageCode = args[1];
            string subscriptionKey = args[2];
            string outputFileName = args[3];

            DoWork(inputFileName, languageCode, subscriptionKey, outputFileName);

            return 0;
        }

        private static void DoWork(string inputFileName, string languageCode, string subscriptionKey, string outputFileName)
        {
            var converter = new SpeechToTextConverter();
            converter.Progress += ReportProgress;
            var result = converter.ConvertToText(inputFileName, languageCode, subscriptionKey);
            var jsonResult = JsonConvert.SerializeObject(result, Formatting.Indented);
            System.IO.File.WriteAllText(outputFileName, jsonResult);
            Console.WriteLine("Successfully created file {0}", outputFileName);
        }

        private static void ReportProgress(object sender, Phrase e)
        {
            Console.WriteLine("# {0}: {1}", e.Confidence, e.Content);
        }
    }
}
