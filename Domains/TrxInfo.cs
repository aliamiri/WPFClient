using System;

namespace WpfNotifierClient.Domains
{
    class TrxInfo
    {
        //Because I want to show this fields in grid I have to make this fields public
        public PersianDateTime TrxDate { get; set; }
        public string CardNo { get; set; }
        public int Amount { get; set; }

        public TrxInfo() { }

        public TrxInfo(PersianDateTime date,string card,int amount)
        {
            Amount = amount;
            CardNo = card;
            TrxDate = date;
        }


        public string Details
        {
            get
            {
                return String.Format("در ساعت {0} در تاریخ {1} مبلغ {2} ریال \n با کارت به شماره {3} خریداری شد.", TrxDate.TimeOfDay.ToHHMMSS(), TrxDate , Amount, CardNo);
            }
        }
    }
}
