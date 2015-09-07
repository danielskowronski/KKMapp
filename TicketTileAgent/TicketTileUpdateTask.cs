using KKMapp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.ApplicationModel.Background;

namespace KKMapp
{
    public sealed class TicketTileUpdateTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            zrob();
            
            deferral.Complete();
        }
        public async static void zrob()
        {
            Common.i18n = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView("Resources");

            try
            {
                TicketInfoAgregator tia = new TicketInfoAgregator();
                ClientInfo ci = new ClientInfo(Windows.Storage.ApplicationData.Current.RoamingSettings.Values["clientInfo"].ToString());
                DateTime dt = DateTime.Today;
                tia.getTicketInfoFromMpk(ci, dt, true);
            }
            catch (Exception ex)
            {

            }
        }
        
    }
}
