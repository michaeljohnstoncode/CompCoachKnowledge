namespace VideoUrlToChatBot.Models
{
    public class MessageAndConversation
    {
        public UserMessage MessageModel { get; set; }
        public List<OpenAI_API.Chat.ChatMessage> ChatConversation { get; set; }
    }
}
