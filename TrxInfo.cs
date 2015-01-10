using System;

namespace WpfNotifierClient
{
    class TrxInfo
    {
        private DateTime _trxDate;
        private string _cardNo;
        private int _amount;

        public void SetAmount(int inAmount)
        {
            this._amount = inAmount;
        }

        public void SetCardNo(string card)
        {
            this._cardNo = card;
        }

        public void SetTrxDate(DateTime time)
        {
            this._trxDate = time;
        }

        public int GetAmount()
        {
            return this._amount;
        }

        public string GetCardNo()
        {
            return this._cardNo;
        }

        public DateTime GetTrxDate()
        {
            return this._trxDate;
        }

        public string Details
        {
            get
            {
                return String.Format("dartarikhe {0} mablaghe {1} bakarteIn {2} kharidarishod", _trxDate.ToShortTimeString(), _amount , _cardNo);
            }
        }
    }
}
