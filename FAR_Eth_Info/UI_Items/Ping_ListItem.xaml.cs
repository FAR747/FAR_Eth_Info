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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FAR_Eth_Info.UI_Items
{
	/// <summary>
	/// Логика взаимодействия для Ping_ListItem.xaml
	/// </summary>
	
	public partial class Ping_ListItem : UserControl
	{
		public string domain = "";
		public Label status_LAB;
		public Ping_ListItem(string IPorDomain)
		{
			InitializeComponent();
			domain = IPorDomain;
			status_LAB = Label_Status;
			updatelabel(String.Format("{0} - Wait...", domain));
		}
		public void updatelabel(string text)
		{
			Label_Status.Content = text;
			MainWindow.log(String.Format("Ping update: {0}", text));
		}

		private void Delete_MI_Item_Click(object sender, RoutedEventArgs e)
		{
			MainWindow.gPing_List.Items.Remove(this);
			MainWindow.log(String.Format("Ping delete: {0}", domain));
		}
	}
}
