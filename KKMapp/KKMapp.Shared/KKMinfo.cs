using System;
using System.Collections.Generic;
using System.Text;

namespace KKMapp
{
    class KKMinfo
    {
        private int cardTypeIdx;//temporal solution! //FIXME
        private string clientId;
        private string cardNumber;
        private string pesel;

        public KKMinfo() 
        {
            cardTypeIdx = 0; clientId = cardNumber = pesel = null;
        }
        public KKMinfo(int cardTypeIdx, string clientId, string cardNumber, string pesel)
        {
            if (cardTypeIdx < 0) //this is only check as idx is from code - only bug could be when there is no data read yet
                throw new UserDataInvalidException("Card type");
            else
                this.cardTypeIdx = cardTypeIdx;

            this.clientId = clientId;
            this.cardNumber = cardNumber;
            this.pesel = pesel;
        }

        public class UserDataInvalidException : Exception
        {
            public UserDataInvalidException(string field) : base(field+" data is invalid") { }
        }

        /*
          Object val;
            val = Windows.Storage.ApplicationData.Current.RoamingSettings.Values["selectedCardTypeIdx"];
            if (val == null) Windows.Storage.ApplicationData.Current.RoamingSettings.Values["selectedCardTypeIdx"] = 0; //trick
            val = Windows.Storage.ApplicationData.Current.RoamingSettings.Values["providedClientID"];
            if (val == null) Windows.Storage.ApplicationData.Current.RoamingSettings.Values["providedClientID"] = "";
            val = Windows.Storage.ApplicationData.Current.RoamingSettings.Values["providedCardNumber"];
            if (val == null) Windows.Storage.ApplicationData.Current.RoamingSettings.Values["providedCardNumber"] = "";
            val = Windows.Storage.ApplicationData.Current.RoamingSettings.Values["providedPesel"];
            if (val == null) Windows.Storage.ApplicationData.Current.RoamingSettings.Values["providedPesel"] = ""; 
         */
    }
    
}
