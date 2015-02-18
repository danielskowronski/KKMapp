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

namespace KKMapp
{
	public sealed partial class MainPage : Page
	{
		
		public MainPage()
		{
			this.InitializeComponent();
            loadCardTypes();

            Object val;
            val = Windows.Storage.ApplicationData.Current.RoamingSettings.Values["alternativeDataSource"];
            if (val == null) Windows.Storage.ApplicationData.Current.RoamingSettings.Values["alternativeDataSource"]=false;
            val = Windows.Storage.ApplicationData.Current.RoamingSettings.Values["selectedCardTypeIdx"];
            if (val == null) Windows.Storage.ApplicationData.Current.RoamingSettings.Values["selectedCardTypeIdx"] = 0; //trick
            val = Windows.Storage.ApplicationData.Current.RoamingSettings.Values["providedClientID"];
            if (val == null) Windows.Storage.ApplicationData.Current.RoamingSettings.Values["providedClientID"] = "";
            val = Windows.Storage.ApplicationData.Current.RoamingSettings.Values["providedCardNumber"];
            if (val == null) Windows.Storage.ApplicationData.Current.RoamingSettings.Values["providedCardNumber"] = "";
            val = Windows.Storage.ApplicationData.Current.RoamingSettings.Values["providedPesel"];
            if (val == null) Windows.Storage.ApplicationData.Current.RoamingSettings.Values["providedPesel"] = "";

            //timeout becouse UI may not be loaded yet
            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            timer.Tick += (sender, args) => { timer.Stop(); setUiElements(); };
            timer.Start();

		}
        private void setUiElements()
        {
            cardTypeComboBox.SelectedIndex = (int)(Windows.Storage.ApplicationData.Current.RoamingSettings.Values["selectedCardTypeIdx"]);
            identityNumberTextBox.Text = (string)(Windows.Storage.ApplicationData.Current.RoamingSettings.Values["providedClientID"]);
            cityCardNumberTextBox.Text = (string)(Windows.Storage.ApplicationData.Current.RoamingSettings.Values["providedCardNumber"]);
            peselNumberTextBox.Text = (string)(Windows.Storage.ApplicationData.Current.RoamingSettings.Values["providedPesel"]);

            if ((bool)(Windows.Storage.ApplicationData.Current.RoamingSettings.Values["alternativeDataSource"]) == false)
                peselNumberTextBox.IsEnabled = false;
            else if ((int)(Windows.Storage.ApplicationData.Current.RoamingSettings.Values["selectedCardTypeIdx"]) != 0)
                    peselNumberTextBox.IsEnabled = true;
        }

        private void CheckAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            //check ticket logic
            if ((bool)(Windows.Storage.ApplicationData.Current.RoamingSettings.Values["alternativeDataSource"])==false)
            {
                //mpk.krakow.pl
            }
            else
            {
                //ebilet.kkm.krakow.pl
            }

            Frame.Navigate(typeof(TicketInfo));
        }

        private void SettingsAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingsPage));
        }

        private void identityNumberTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (identityNumberTextBox.Text.Length > 9) showWarningBox("Client ID too long");
            if ((((CardType)(cardTypeComboBox.SelectedItem)).getType() != "0") && identityNumberTextBox.Text.Length > 7) showWarningBox("Student card ID too long");

            Windows.Storage.ApplicationData.Current.RoamingSettings.Values["providedClientID"] = identityNumberTextBox.Text;
        }

        private void cityCardNumberTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (cityCardNumberTextBox.Text.Length > 10) showWarningBox("City Card ID too long");

            Windows.Storage.ApplicationData.Current.RoamingSettings.Values["providedCardNumber"] = cityCardNumberTextBox.Text;
        }
      

        private void peselNumberTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (peselNumberTextBox.Text.Length > 11) showWarningBox("PESEL too long");

            Windows.Storage.ApplicationData.Current.RoamingSettings.Values["providedPesel"] = peselNumberTextBox.Text;
        }
        private void cardTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CardType sel = (CardType)(cardTypeComboBox.SelectedItem);
            if (sel.getType() != "0")
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
            
            Windows.Storage.ApplicationData.Current.RoamingSettings.Values["selectedCardTypeIdx"] = (cardTypeComboBox.SelectedIndex);
        }

        private void showWarningBox(string text)
        {
            MessageDialog dialog = new MessageDialog(text);
            dialog.ShowAsync();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TicketInfoAgregator tia = new TicketInfoAgregator(); ClientInfo ci = new ClientInfo(); DateTime dt = new DateTime();
            tia.getTicketInfoFromMpk(ci, dt);
        }
	}
}
