using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VKAccessToken
{
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window
   {
      public MainWindow()
      {
         InitializeComponent();
         int permissions = 8 + 65536;
         string uriString =
            String.Format(
               "https://oauth.vk.com/authorize?client_id=4697962&scope={0}&redirect_uri=https://oauth.vk.com/blank.html&display=page&v=5.27&response_type=token",
               permissions);
         WebBrowser.Navigated += WebBrowserOnNavigated;
         WebBrowser.Navigate(new Uri(uriString));
      }

      private void WebBrowserOnNavigated(object sender, NavigationEventArgs navigationEventArgs)
      {
         if (navigationEventArgs.Uri.AbsolutePath == "/blank.html")
         {
            NameValueCollection args = HttpUtility.ParseQueryString(navigationEventArgs.Uri.Fragment.Substring(1));
            
            string accessToken = args["access_token"];
            string profileId = args["user_id"];
            VKAccessTokenWindow.Show(profileId, accessToken);
            Close();
         }         
      }
   }
}
