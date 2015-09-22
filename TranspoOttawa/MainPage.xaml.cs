using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using TranspoOttawa.Resources;


namespace TranspoOttawa
{



    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            //ThemeManager.ToLightTheme();

            //Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();

            for (int i = 0; i < 5; i++)
            {

            }
        }
        
        private void FindBusButton_Click(object sender, RoutedEventArgs e)
        {
            if (StopBox.Text != "") {
                NavigationService.Navigate(new Uri("/Pages/StopInfo/StopInfoPivotPage.xaml?stop="+StopBox.Text, UriKind.Relative));
            }
            else
            {
                MessageBox.Show("Please enter a stop number.");
            }
        }




        

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}