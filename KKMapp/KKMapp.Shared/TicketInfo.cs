﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Globalization;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;

namespace KKMapp
{
    class TicketInfo
    {
        public TicketInfo(string dataraw)
        {
            Common.i18n = Windows.ApplicationModel.Resources.ResourceLoader.GetForViewIndependentUse("Resources");

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
            return Common.i18n.GetString("City") + ": " + (String.IsNullOrEmpty(cityLinesValid) ? "-" : cityLinesValid) + "\n" + Common.i18n.GetString("Suburban") + ": " + (String.IsNullOrEmpty(suburbanLinesValid) ? "-" : suburbanLinesValid);
        }
        public string getTicketDescr()
        {
            if (ticketTypeDescr != null) return ticketTypeDescr; else return Common.i18n.GetString("NoData");
        }

        public string getTicketValidSince()
        {
            if (validSince.ToString("yyyy-MM-dd") != "0001-01-01") return validSince.ToString("yyyy-MM-dd"); else return Common.i18n.GetString("NoData");
        }

        public string getTicketExpirationDate()
        {
            if (expiresAt.ToString("yyyy-MM-dd") != "0001-01-01") return expiresAt.ToString("yyyy-MM-dd"); else return Common.i18n.GetString("NoData");
        }
        public int getTicketDaysLeft()
        {
            TimeSpan ts = expiresAt - DateTime.Now;
            return ts.Days;
        }
        public string getTicketExpirationDescr()
        {
            if (getTicketExpirationDate() != Common.i18n.GetString("NoData"))
                return getTicketExpirationDate() + " (" + (
                    getTicketDaysLeft() >= 0
                    ? getTicketDaysLeft() + " " + Common.i18n.GetString("DaysLeft")
                    : Common.i18n.GetString("ExpiredSince") + " " + (-getTicketDaysLeft()) + Common.i18n.GetString("Days")
                ) + ")";
            else
                return Common.i18n.GetString("NoData");
        }

    }
    class TicketInfoAgregator
    {
        TicketInfo ti;
        bool isFromBackend;

        public void getTicketInfoFromMpk(ClientInfo ci, DateTime date, bool isFromBackend=false)
        {
            this.isFromBackend = isFromBackend;

            string url = "http://www.mpk.krakow.pl/pl/sprawdz-waznosc-biletu/index,1.html?" +
                "cityCardType=" +  ci.card.id +
                "&dateValidity=" + date.ToString("yyyy-MM-dd") +
                "&identityNumber=" + ci.clientID +
                "&cityCardNumber=" + ci.cardID;

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            //httpWebRequest.ContentType = "text/plain; charset=utf-8";
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
                        outraw = outraw.Substring(outraw.IndexOf("<div>Rodzaj"));
                        if (outraw.IndexOf("<div style=", 10) != -1)
                            outraw = outraw.Substring(0, outraw.IndexOf("<div style=", 10));

                        ti = new TicketInfo(outraw);
                    }
                }
                catch (WebException e)
                {
                    ti = new TicketInfo(null);
                }
                finally
                {
                    if (!isFromBackend)
                    {
                        Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                            Windows.UI.Core.CoreDispatcherPriority.Normal,
                            () =>
                            {
                                LoadedEvent.Invoke(this, ti);
                            }
                         );
                    }
                    else
                    {
                        string data1 = "KKM: " + Common.i18n.GetString("expires");
                        string data2 = ti.getTicketExpirationDescr();

                        var updater = TileUpdateManager.CreateTileUpdaterForApplication();
                        updater.EnableNotificationQueue(true);
                        updater.Clear();

                        XmlDocument tileXml;

                        tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWideText03);
                        tileXml.GetElementsByTagName("text")[0].InnerText = data1 + "\n" + data2;
                        updater.Update(new TileNotification(tileXml));

                        tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquareText04);
                        tileXml.GetElementsByTagName("text")[0].InnerText = data1 + "\n" + data2;
                        updater.Update(new TileNotification(tileXml));
                    }
                }
            }
        }

    }
}
