using System;

namespace WpfApplication2
{
    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime Birthday { get; set; }

        public string Details
        {
            get
            {
                return String.Format("{0} در تاریخ {1} در خوانواده ای مذهبی در شهر برره به دنیا آمد", this.Name, this.Birthday.ToLongDateString());
            }
        }
    }
}