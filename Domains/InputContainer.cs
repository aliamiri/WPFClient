namespace WpfNotifierClient.Domains
{
    class InputContainer
    {
        public Commands Command { get; set; }

        public TrxInfo Info { get; set; }

        public bool Type { get; set; }
    }
}
