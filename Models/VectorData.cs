using OpenAI_API.Embedding;

namespace VideoUrlToChatBot.Models
{
    public class VectorData
    {
        public string FileName { get; set; }
        public string FileData { get; set; }
        public EmbeddingResult EmbeddingResult { get; set; }
    }

}
