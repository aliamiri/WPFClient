using System;

namespace WpfApplication2
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
    }
}
