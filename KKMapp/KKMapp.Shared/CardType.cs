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
    public class CardType
    {
        public string descr { get; private set; }
        public string id { get; private set; }
        public override string ToString() { return descr; }
        public CardType(string description, string id) { this.descr = description; this.id = id; }

    }

    public sealed partial class MainPage : Page
    {

        public async void loadCardTypes()
        {
            cardTypeComboBox.SelectionChanged -= cardTypeComboBox_SelectionChanged;
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
                    App.CardTypeList.Add(tmp);
                }
            }
            cardTypeComboBox.SelectionChanged += cardTypeComboBox_SelectionChanged;
        }
    }
}
