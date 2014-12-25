using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VKAccessToken
{
   /// <summary>
   /// Interaction logic for VKAccessTokenWindow.xaml
   /// </summary>
   public partial class VKAccessTokenWindow : Window
   {
      public VKAccessTokenWindow()
      {
         InitializeComponent();
      }

      public static void Show(string profileId, string accessToken)
      {
         VKAccessTokenWindow window = new VKAccessTokenWindow
         {
            txtProfileId = {Text = profileId},
            txtAccessToken = {Text = accessToken}
         };
         window.ShowDialog();
      }

      private void btnOk_OnClick(object sender, RoutedEventArgs e)
      {
         Close();
      }
   }
}
