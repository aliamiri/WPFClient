using System;

namespace WpfNotifierClient
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
                return String.Format("در ساعت {0} مبلغ {1} ریال با کارت به شماره {2} خریداری شد.", TrxDate.TimeOfDay.ToHHMMSS(), Amount , CardNo);
            }
        }
    }
}
