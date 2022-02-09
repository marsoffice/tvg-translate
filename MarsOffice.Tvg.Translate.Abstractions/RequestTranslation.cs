using System.Collections.Generic;

namespace MarsOffice.Tvg.Translate.Abstractions
{
    public class RequestTranslation
    {
        public string VideoId { get; set; }
        public string JobId { get; set; }
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public IEnumerable<string> Sentences { get; set; }
        public string FromLangCode { get; set; }
        public string ToLangCode { get; set; }
    }
}