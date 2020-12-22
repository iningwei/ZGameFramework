
namespace ZGame.Net
{
    public class Message
    {
        public byte[] Content { get; set; }
        public Message(byte[] data)
        {
            this.Content = data;
        }
    }
}