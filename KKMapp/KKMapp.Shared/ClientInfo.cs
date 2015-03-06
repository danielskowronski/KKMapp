using System;
using System.Collections.Generic;
using System.Text;

namespace KKMapp
{
    class ClientInfo
    {
        public string cardType="";
        public string cardTypeNum = "0";//temporal hack - will be replaced by CardType instance
        public int cardTypePosition=0; //temporal hack
        public string clientID="";
        public string cardID="";
        public string pesel="";

        public string Serialize()
        {
            return cardType + "|" + cardTypeNum + "|" +cardTypePosition.ToString() + "|" + clientID + "|" + cardID + "|" + pesel;
        }
        public ClientInfo(string serializedObject)
        {
            string[] fields = serializedObject.Split('|'); int i = 0;
            cardType = fields[i++];
            cardTypeNum = fields[i++];
            cardTypePosition = Int32.Parse(fields[i++]);
            clientID = fields[i++];
            cardID = fields[i++];
            pesel = fields[i++];
        }
        public ClientInfo()
        {
             //empty constructor
        }
    }
}
