using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Globalization;

namespace KKMapp
{
    class TicketInfo
    {
        public TicketInfo(string dataraw)
        {
            this.dataraw = dataraw;
            if (dataraw == null)
            {
                isTicketThere = false; 
                return;
            }
            isTicketThere = true;

            if (dataraw.IndexOf("Ulgowy") != -1) isReducedPrice = true; else isReducedPrice = false;

            ticketTypeRaw = dataraw.Substring(5, dataraw.IndexOf("</div>"));


            ticketTypeDescr = ticketTypeRaw.Substring(ticketTypeRaw.IndexOf("<b>") + 3);
            ticketTypeDescr = ticketTypeDescr.Substring(0,ticketTypeDescr.IndexOf("</b>"));

            dataraw = dataraw.Substring(dataraw.IndexOf("Data pocz", 5) + 5);
            string tmp = dataraw.Substring(dataraw.IndexOf("<b>")+3, 10);
            CultureInfo provider = CultureInfo.InvariantCulture;
            validSince = DateTime.ParseExact(tmp, "yyyy-MM-dd", provider);
            
            dataraw = dataraw.Substring(dataraw.IndexOf("Data ko") + 0);
            tmp = dataraw.Substring(dataraw.IndexOf("<b>") + 3, 10);
            expiresAt = DateTime.ParseExact(tmp, "yyyy-MM-dd", provider);

            dataraw = dataraw.Substring(dataraw.IndexOf("Linie mie") + 0);
            cityLinesValid = dataraw.Substring(dataraw.IndexOf("<b>") + 3);
            cityLinesValid = cityLinesValid.Substring(0, cityLinesValid.IndexOf("</b>"));

            dataraw = dataraw.Substring(dataraw.IndexOf("Linie stre") + 0);
            suburbanLinesValid = dataraw.Substring(dataraw.IndexOf("<b>") + 3);
            suburbanLinesValid = suburbanLinesValid.Substring(0, suburbanLinesValid.IndexOf("</b>"));

        }
        
        private string dataraw;

        private bool isTicketThere;
        
        private string ticketTypeRaw; // like "Ulgowy | Wszystkie dni tygodnia"
        
        private bool isReducedPrice;
        private string ticketTypeDescr; //like "Whole week"

        private DateTime validSince;
        private DateTime expiresAt;

        private string cityLinesValid;
        private string suburbanLinesValid;

        public string getLinesValid()
        {
            return "City: " + cityLinesValid + " | Suburban: " + suburbanLinesValid;
        }
        public string getTicketDescr()
        {
            if (ticketTypeDescr != null) return ticketTypeDescr; else return "No data";
        }

        public string getTicketValidSince()
        {
            if (validSince.ToString() != "1/1/0001 12:00:00 AM") return validSince.ToString(); else return "No data";
        }

        public string getTicketExpirationDate()
        {
            if (expiresAt.ToString() != "1/1/0001 12:00:00 AM") return expiresAt.ToString(); else return "No data";
        }
        public int getTicketDaysLeft()
        {
            TimeSpan ts = expiresAt - DateTime.Now;
            return ts.Days;
        }
        public string getTicketExpirationDescr()
        {
            if (getTicketExpirationDate() != "No data")
                return getTicketExpirationDate() + " (" + (
                    getTicketDaysLeft() >= 0
                    ? getTicketDaysLeft() + "days left"
                    : "expired since " + (-getTicketDaysLeft()) + "days"
                ) + ")";
            else
                return "No data";
        }

    }
    class TicketInfoAgregator
    {
        TicketInfo ti;

        public void getTicketInfoFromMpk(ClientInfo ci, DateTime date)
        {
            string url = "http://www.mpk.krakow.pl/pl/sprawdz-waznosc-biletu/index,1.html?" +
                "cityCardType=" + "22"+//ci.cardTypeNum +//fixme
                "&dateValidity=" + date.ToString("yyyy-MM-dd") +
                "&identityNumber=" + ci.clientID +
                "&cityCardNumber=" + ci.cardID;

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "text/plain; charset=utf-8";
            httpWebRequest.Method = "GET";

            httpWebRequest.BeginGetResponse(getTicketInfoFromMpkCallback, httpWebRequest);
        }

        public delegate void LoadedEventHandler(object sender, TicketInfo ti);
        public static event LoadedEventHandler LoadedEvent;

       
        void getTicketInfoFromMpkCallback(IAsyncResult result)
        {
            HttpWebRequest request = result.AsyncState as HttpWebRequest;
            if (request != null)
            {
                try
                {
                    WebResponse response = request.EndGetResponse(result);
                    System.IO.Stream s = response.GetResponseStream();
                    System.IO.StreamReader sr = new System.IO.StreamReader(s);
                    string outraw = sr.ReadToEnd();

                    if (outraw.IndexOf("Nie znaleziono") != -1) ti = new TicketInfo(null);
                    else
                    {

                        outraw = outraw.Substring(outraw.IndexOf("<div class=\"kkm-card\">"));
                        outraw = outraw.Substring(0, outraw.IndexOf("<!-- Index:End -->"));
                        /*outraw = outraw.Substring(outraw.IndexOf("<br />")+6);
                        outraw = outraw.Substring(outraw.IndexOf("<br />")+6);
                        outraw = outraw.Substring(5);*/
                        outraw = outraw.Substring(outraw.IndexOf("<div>Rodzaj"));
                        if (outraw.IndexOf("<div style=", 10) != -1)
                        {
                            outraw = outraw.Substring(0, outraw.IndexOf("<div style=", 10));
                        }

                        ti = new TicketInfo(outraw);
                    }
                }
                catch (WebException e)
                {
                    ti = new TicketInfo(null);
                }
                finally
                {
                    Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                        Windows.UI.Core.CoreDispatcherPriority.Normal,
                        () =>
                        {
                            LoadedEvent.Invoke(this, ti);
                        }
                     );
                }
            }
        }

    }
}
