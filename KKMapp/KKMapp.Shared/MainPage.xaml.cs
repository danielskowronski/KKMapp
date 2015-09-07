using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Windows.UI.Popups;
using Windows.Storage;
using Windows.ApplicationModel.Background;

namespace KKMapp
{
	public sealed partial class MainPage : Page
	{
		
		public MainPage()
		{
			this.InitializeComponent();
            loadCardTypes();

            Object val;
            val = Windows.Storage.ApplicationData.Current.RoamingSettings.Values["clientInfo"];
            if (val == null) Windows.Storage.ApplicationData.Current.RoamingSettings.Values["clientInfo"] = (new ClientInfo()).Serialize();
            
            val = Windows.Storage.ApplicationData.Current.RoamingSettings.Values["alternativeDataSource"];
            if (val == null) Windows.Storage.ApplicationData.Current.RoamingSettings.Values["alternativeDataSource"]=false;
            
            //timeout becouse UI may not be loaded yet
            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            timer.Tick += (sender, args) => { timer.Stop(); setUiElements(); };
            timer.Start();

            RegisterBackgroundTask();
        }

        private async void RegisterBackgroundTask()
        {
            var taskName = "TicketTileUpdateTask";
            var backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();
            TicketTileUpdateTask.zrob();

            if (backgroundAccessStatus==BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity || backgroundAccessStatus==BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity)
            {
                foreach (var task in BackgroundTaskRegistration.AllTasks)
                {
                    if (task.Value.Name==taskName)
                    {
                        return;
                    }
                }
                BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder();
                taskBuilder.Name = taskName;
                taskBuilder.TaskEntryPoint = typeof(TicketTileUpdateTask).FullName;
                taskBuilder.SetTrigger(new TimeTrigger(360, false));
                
                var registration  = taskBuilder.Register();
            }
        }

        private void setUiElements()
        {
            ClientInfo ci = new ClientInfo(Windows.Storage.ApplicationData.Current.RoamingSettings.Values["clientInfo"].ToString());
            cardTypeComboBox.SelectedIndex = ci.cardTypePosition;
            identityNumberTextBox.Text = ci.clientID;
            cityCardNumberTextBox.Text = ci.cardID;
            peselNumberTextBox.Text = ci.pesel;


            if ((bool)(Windows.Storage.ApplicationData.Current.RoamingSettings.Values["alternativeDataSource"]) == false)
                peselNumberTextBox.IsEnabled = false;
            else if (ci.cardTypePosition != 0)
                    peselNumberTextBox.IsEnabled = true;
        }

        private void CheckAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            TicketInfoAgregator tia = new TicketInfoAgregator(); 
            ClientInfo ci = new ClientInfo(Windows.Storage.ApplicationData.Current.RoamingSettings.Values["clientInfo"].ToString());
            DateTime dt = targetDatePicker.Date.LocalDateTime;

            //check ticket logic
            if ((bool)(Windows.Storage.ApplicationData.Current.RoamingSettings.Values["alternativeDataSource"])==false)
            {
                //mpk.krakow.pl
                tia.getTicketInfoFromMpk(ci, dt);
            }
            else
            {
                //ebilet.kkm.krakow.pl
                tia.getTicketInfoFromMpk(ci, dt); //fixme
            }

            Frame.Navigate(typeof(TicketInfoPage));
        }

        private void SettingsAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingsPage));
        }

        private void identityNumberTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (identityNumberTextBox.Text.Length > 9) showWarningBox(Common.i18n.GetString("ClientIDTooLong"));
            if ((((CardType)(cardTypeComboBox.SelectedItem)).id != "0") && identityNumberTextBox.Text.Length > 7) showWarningBox(Common.i18n.GetString("StudentIDTooLong"));

            //Cannot modify the result of an unboxing conversion
            ClientInfo ci = new ClientInfo(Windows.Storage.ApplicationData.Current.RoamingSettings.Values["clientInfo"].ToString());
            ci.clientID = identityNumberTextBox.Text;
            Windows.Storage.ApplicationData.Current.RoamingSettings.Values["clientInfo"] = ci.Serialize();
        }

        private void cityCardNumberTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (cityCardNumberTextBox.Text.Length > 11) showWarningBox(Common.i18n.GetString("CityCardIDTooLong"));

            //Cannot modify the result of an unboxing conversion
            ClientInfo ci = new ClientInfo(Windows.Storage.ApplicationData.Current.RoamingSettings.Values["clientInfo"].ToString());
            ci.cardID = cityCardNumberTextBox.Text;
            Windows.Storage.ApplicationData.Current.RoamingSettings.Values["clientInfo"] = ci.Serialize();
        }
      

        private void peselNumberTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (peselNumberTextBox.Text.Length > 11) showWarningBox(Common.i18n.GetString("PeselTooLong"));

            //Cannot modify the result of an unboxing conversion
            ClientInfo ci = new ClientInfo(Windows.Storage.ApplicationData.Current.RoamingSettings.Values["clientInfo"].ToString());
            ci.pesel = peselNumberTextBox.Text;
            Windows.Storage.ApplicationData.Current.RoamingSettings.Values["clientInfo"] = ci.Serialize();
        }
        private void cardTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CardType sel = (CardType)(cardTypeComboBox.SelectedItem);
            if (sel.id != "0")
            {
                cityCardNumberTextBox.IsEnabled = false; 
                if ((bool)(Windows.Storage.ApplicationData.Current.RoamingSettings.Values["alternativeDataSource"]) == false)
                    peselNumberTextBox.IsEnabled = false;
                else
                    peselNumberTextBox.IsEnabled = true;
            }
            else
            {
                cityCardNumberTextBox.IsEnabled = true; 
                peselNumberTextBox.IsEnabled = false;
            }

            //Cannot modify the result of an unboxing conversion
            ClientInfo ci = new ClientInfo(Windows.Storage.ApplicationData.Current.RoamingSettings.Values["clientInfo"].ToString());
            ci.cardTypePosition = (cardTypeComboBox.SelectedIndex);
            ci.card = sel;
            Windows.Storage.ApplicationData.Current.RoamingSettings.Values["clientInfo"] = ci.Serialize();
        }

        private void showWarningBox(string text)
        {
            MessageDialog dialog = new MessageDialog(text);
            dialog.ShowAsync();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           
        }
	}
}
