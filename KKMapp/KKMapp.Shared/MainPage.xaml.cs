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

using Windows.Storage;

namespace KKMapp
{
	public sealed partial class MainPage : Page
	{
		
		
		public MainPage()
		{
			this.InitializeComponent();
			loadCardTypes();
		}


		private void checkTicketsButtonClick(object sender, RoutedEventArgs e)
		{

		}

		

		private void AppBarButton_Click(object sender, RoutedEventArgs e)
		{
			Frame.Navigate(typeof(MainPage));
		}
	}
}
