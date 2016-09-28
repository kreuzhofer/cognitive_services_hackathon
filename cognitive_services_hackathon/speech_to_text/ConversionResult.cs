using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace speech_to_text
{
    public class Phrase
    {
        public string Confidence { get; set; }
        public string Content { get; set; }
        public bool Corrected { get; set; }
        public string Offset { get; set; }
    }

    public class DetectedPhrase
    {
        public List<Phrase> Phrase { get; set; }
    }

    public class EntityModel
    {
        public string Entity { get; set; }
        public bool Accepted { get; set; }
    }

    public class ConversionResult
    {
        public string Title { get; set; }
        public string TotalLength { get; set; }
        public List<DetectedPhrase> DetectedPhrases { get; set; }
        public List<EntityModel> Entities { get; set; }
    }

}
