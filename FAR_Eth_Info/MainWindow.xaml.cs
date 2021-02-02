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
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Diagnostics;

namespace FAR_Eth_Info
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public static ListBox Interfaces_List_LB;
		public static ListBox gPing_List;
		public static CheckBox OnlyUP_CheckBox;
		public static CheckBox AutoRefCB;
		public static TextBlock InternetStatus_TB;
		public static ListBox Log_LB;
		public static bool autoref = false;
		static Timer timer;
		public MainWindow()
		{
			InitializeComponent();
			Interfaces_List_LB = Interfaces_List;
			OnlyUP_CheckBox = OnlyUP_CB;
			InternetStatus_TB = Status_Internet_TB;
			AutoRefCB = AutoRefresh_CB;
			Log_LB = Log_Listbox;
			gPing_List = Ping_list;

			log("Welcome to FAR Eth Info");
			log("Developed by FAR747 (Twitter @FAR747, GitHub FAR747)");

			loadinterfaces();
			//ConnectionsCheck();
			TimerCallback tm = new TimerCallback(Timer_tick);
			timer = new Timer(tm, 0, 0, 10000); //2mins 120000
		}

		public static void loadinterfaces()
		{
			IPGlobalProperties computerProperties = IPGlobalProperties.GetIPGlobalProperties();
			NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
			Interfaces_List_LB.Items.Clear();
			if (nics == null || nics.Length < 1)
			{
				Label errmess = new Label();
				errmess.Content = "No network interfaces found!";
				Interfaces_List_LB.Items.Add(errmess);
				return;
			}
			foreach (NetworkInterface adapter in nics)
			{
				if ((bool)OnlyUP_CheckBox.IsChecked)
				{
					if (adapter.OperationalStatus == OperationalStatus.Up)
					{
						UI_Items.Interfaces_ListItem adapteritem = new UI_Items.Interfaces_ListItem(adapter);
						Interfaces_List_LB.Items.Add(adapteritem);
					}
				}
				else
				{
					UI_Items.Interfaces_ListItem adapteritem = new UI_Items.Interfaces_ListItem(adapter);
					Interfaces_List_LB.Items.Add(adapteritem);
				}
				
			}
		}

		public static void ConnectionsCheck()
		{
			Ping pinger = new Ping();
			PingReply reply = pinger.Send("8.8.8.8", 1000, new Byte[32], null);
			log("Check Internet");
			var pingable = reply.Status == IPStatus.Success;
			if (reply.Status == IPStatus.Success)
			{
				
				MainWindow.Interfaces_List_LB.Dispatcher.BeginInvoke((Action)(() =>
					MainWindow.InternetStatus_TB.Text = String.Format("Connected ({0}ms)", reply.RoundtripTime)
				));
			}
			else
			{
				
				MainWindow.Interfaces_List_LB.Dispatcher.BeginInvoke((Action)(() =>
					MainWindow.InternetStatus_TB.Text = String.Format("Disconnected ({0}, {1})", reply.Status.ToString(), reply.RoundtripTime)
				));
			}
			log(String.Format("Internet Status: {0}, {1}", reply.Status, reply.Address));

			foreach(UI_Items.Ping_ListItem item in MainWindow.gPing_List.Items)
			{
				//reply = pinger.Send(item., 10000, new Byte[32], null);
				try
				{
					log(String.Format("Check {0}", item.domain));
					reply = pinger.Send(item.domain, 800, new Byte[32], null);
					item.Label_Status.Dispatcher.BeginInvoke((Action)(() =>
						item.updatelabel(String.Format("{0} - {1} ({2}ms)", item.domain, reply.Status.ToString(), reply.RoundtripTime))
					));
				}
				catch (Exception ex)
				{
					log(String.Format("Error/CheckConnect: {0}", ex.Message));
				}
			}
		}

		private void Connection_Ref_BUTTON_Click(object sender, RoutedEventArgs e)
		{
			loadinterfaces();
		}

		static void Timer_tick(object obj)
		{
			//Dispatcher.BeginInvoke((Action)(() => MainWindow.gMini_Message.Content = "ESO Running"));
			if (autoref)
			{
				MainWindow.Interfaces_List_LB.Dispatcher.BeginInvoke((Action)(() => MainWindow.loadinterfaces()));
			}
			ConnectionsCheck();
		}

		private void AutoRefresh_CB_Checked(object sender, RoutedEventArgs e)
		{
			autoref = (bool)AutoRefresh_CB.IsChecked;
		}
		public static void log(string text)
		{
			Trace.WriteLine(text);
			
			MainWindow.Log_LB.Dispatcher.BeginInvoke((Action)(() => {
				TextBlock TB = new TextBlock();
				TB.Text = text;
				TB.FontSize = 9;
				Log_LB.Items.Add(TB);
				//Log_LB.ScrollIntoView(Log_LB.Items.Count - 1);
				if (Log_LB.Items.Count > 0)
				{
					Log_LB.ScrollIntoView(Log_LB.Items[Log_LB.Items.Count - 1]);
				}
				}
			));
		}

		private void Ping_add_BT_Click(object sender, RoutedEventArgs e)
		{
			if (IPPing_add_TB.Text.Replace(" ", "") != "")
			{
				addPingList(IPPing_add_TB.Text);
				IPPing_add_TB.Text = "";
			}
		}
		public static void addPingList(string IPorDomain)
		{
			UI_Items.Ping_ListItem item = new UI_Items.Ping_ListItem(IPorDomain);
			gPing_List.Items.Add(item);
		}
	}
}
