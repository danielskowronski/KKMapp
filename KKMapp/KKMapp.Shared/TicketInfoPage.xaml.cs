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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace KKMapp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TicketInfoPage : Page
    {
        public TicketInfoPage()
        {
            this.InitializeComponent();
            TicketInfoAgregator.LoadedEvent += upd;
        }

        private void upd(object sender, TicketInfo ti)
        {
            int a = 0;
            ticketTypeTextBlock.Text = ti.getTicketDescr();
            ticketValidSinceTextBlock.Text = ti.getTicketValidSince();
            ticketExpiresTextBlock.Text = ti.getTicketExpirationDescr();
            ticketLinesTextBlock.Text = ti.getLinesValid();
        }


        private void BackAppBarButtonBack_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void PinAppBarButtonPin_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ticketExpiresTextBlock.Text = Common.i18n.GetString("NoDataYet");
            ticketLinesTextBlock.Text = Common.i18n.GetString("NoDataYet");
            ticketTypeTextBlock.Text = Common.i18n.GetString("NoDataYet");
            ticketValidSinceTextBlock.Text = Common.i18n.GetString("NoDataYet");
        }

    }
}
