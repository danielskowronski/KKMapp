using System;
using System.Collections.Generic;
using System.Text;
using Windows.Storage;
using Windows.UI;
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

namespace KKMapp
{
    class CardType
    {
        private string description, id;
        public override string ToString() { return description; }
        public string getType() { return id;  }
        public CardType(string description, string id) { this.description = description; this.id = id; }

    }

    public sealed partial class MainPage : Page
    {
        public async void loadCardTypes()
        {
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(@"ms-appx:///Data/" + "CardTypes.csv"));
            using (Stream stream = (await file.OpenReadAsync()).AsStreamForRead())
            {
                StreamReader sr = new StreamReader(stream); string line;
                while (true)
                {
                    if (sr.EndOfStream) break;
                    line = sr.ReadLine();
                    string[] tokens = line.Split(';');
                    CardType tmp = new CardType(tokens[0],tokens[1]);
                    cardTypeComboBox.Items.Add(tmp);
                }
            }

        }
        private void cardTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CardType sel = (CardType)(cardTypeComboBox.SelectedItem);
            if (sel.getType() != "0")
            {
                cityCardNumberTextBox.IsEnabled = false; cityCardNumberTextBox.Text = "not needed";
            }
            else
            {
                cityCardNumberTextBox.IsEnabled = true; cityCardNumberTextBox.Text = "";
            }
        }
    }
}
