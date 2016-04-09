using System;

namespace Looksfamiliar.d2c2d.MessageModels
{
    public enum MessageTypeEnum
    {
        NotSet = 0,
        Ping = 1,
        Climate = 2,
        Command = 3
    }

    public class MessageBase
    {
        public MessageBase()
        {
            Id = Guid.NewGuid().ToString();
            DeviceId = string.Empty;
            MessageType = MessageTypeEnum.NotSet;
            Longitude = 0.0;
            Latitude = 0.0;
            Timestamp = DateTime.Now;
        }

        public string Id { get; set; }
        public string DeviceId { get; set; }
        public MessageTypeEnum MessageType { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
