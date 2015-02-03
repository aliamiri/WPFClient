namespace WpfNotifierClient.Domains
{
    class User
    {
        public string UserName{ get; set; }
        public string Password{ get; set; }
        public int AccessLevel{ get; set; }
        public PersianDateTime LastLogin { get; set; }
    }
}
