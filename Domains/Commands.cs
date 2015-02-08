namespace WpfNotifierClient.Domains
{
    class Commands
    {
        public int Priority { get; set; }
        public CommandTypes Command { get; set; }
    }

    enum CommandTypes
    {
        SendLogToServer,
        LogOffApplication,
        GoFuckYourself //TODO replace this command with appreciated command :D
    }
}
