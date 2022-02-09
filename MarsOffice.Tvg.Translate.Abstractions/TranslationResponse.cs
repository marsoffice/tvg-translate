using System.Collections.Generic;

namespace MarsOffice.Tvg.Translate.Abstractions
{
    public class TranslationResponse
    {
        public string VideoId { get; set; }
        public string JobId { get; set; }
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public Dictionary<string, string> TranslatedSentences { get; set; }
        public string FromLangCode { get; set; }
        public string ToLangCode { get; set; }
        public bool Success { get; set; }
        public string Error { get; set; }
    }
}