namespace Looksfamiliar.d2c2d.MessageModels
{
    public enum CommandTypeEnum
    {
        Ping = 0,
        Start = 1,
        Stop = 2,
        UpdateFirmeware = 3
    }

    public class Command : MessageBase
    {
        public Command()
        {
            CommandType = CommandTypeEnum.Ping;
            CommandParameters = string.Empty;
            MessageType = MessageTypeEnum.Command;
        }

        public CommandTypeEnum CommandType { get; set; }
        public string CommandParameters { get; set; }
    }
}
