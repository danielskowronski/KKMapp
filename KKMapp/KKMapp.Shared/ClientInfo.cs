using System;
using System.Collections.Generic;
using System.Text;

namespace KKMapp
{
    class ClientInfo
    {
        public CardType card = new CardType("Karta KKM", "0");
        public int cardTypePosition=0; //temporal hack
        public string clientID="";
        public string cardID="";
        public string pesel="";

        public string Serialize()
        {
            return card.descr + "|" + card.id + "|" +cardTypePosition.ToString() + "|" + clientID + "|" + cardID + "|" + pesel;
        }
        public ClientInfo(string serializedObject)
        {
            string[] fields = serializedObject.Split('|'); int i = 0;
            card = new CardType(fields[i++], fields[i++]);
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
