using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DomoticaApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : TabbedPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
    }
}