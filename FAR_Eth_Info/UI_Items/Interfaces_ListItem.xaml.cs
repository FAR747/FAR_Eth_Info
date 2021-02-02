using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
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
using System.Diagnostics;

namespace FAR_Eth_Info.UI_Items
{
	/// <summary>
	/// Логика взаимодействия для Interfaces_ListItem.xaml
	/// </summary>
	public partial class Interfaces_ListItem : UserControl
	{
		public string IPv4Address = "";
		public string IPv6Address = "";
		public string IPv4Gateway = "";
		public string IPv6Gateway = "";
		public string Name = "";
		public Interfaces_ListItem(NetworkInterface adapter)
		{
			InitializeComponent();
			Name = adapter.Name;
			update(adapter);
		}

		public void update(NetworkInterface adapter)
		{
			string adapterstatus = adapter.OperationalStatus.ToString();
			Label_Name.Content = String.Format("{0} ({1})", adapter.Name, adapterstatus);
			IPInterfaceProperties properties = adapter.GetIPProperties();
			foreach (IPAddressInformation unicast in properties.UnicastAddresses)
			{
				//unicast.Address.
				if (unicast.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
				{
					IPv4_Label.Content = unicast.Address;
					IPv4Address = unicast.Address.ToString();
					Trace.WriteLine("IPv4: " + unicast.Address);
				}
				if (unicast.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6 && unicast.Address.IsIPv6LinkLocal)
				{
					IPv6_Label.Content = unicast.Address;
					IPv6Address = unicast.Address.ToString();
					Trace.WriteLine("IPv6: " + unicast.Address);
				}
			}
			if (properties.DnsAddresses.Count > 0)
			{
				DNS_Label.Content = "";
				foreach (IPAddress dns in properties.DnsAddresses)
				{
					DNS_Label.Content += dns + Environment.NewLine;
					DNS_Label.ToolTip = DNS_Label.Content;
				}
			}
			string GatewayIPv4 = "127.0.0.1";
			string GatewayIPv6 = "None";
			foreach (GatewayIPAddressInformation address in properties.GatewayAddresses)
			{
				if (address.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
				{
					GatewayIPv4 = address.Address.ToString();
					GateWay_Label.Content = GatewayIPv4;
				}
				if (address.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
				{
					GatewayIPv6 = address.Address.ToString();
				}
			}
			IPv4Gateway = GatewayIPv4;
			IPv6Gateway = GatewayIPv6;
			GateWay_Label.ToolTip = String.Format("IPv4: {0}\nIPv6: {1}", GatewayIPv4, GatewayIPv6);
			//Mask_Label.Content = properties.
			MainWindow.log(String.Format("Adapter {0} updated! IPv4 {1}, Gateway {2}, Mask {3}", Name, IPv4_Label.Content, IPv4Gateway, Mask_Label.Content));
		}

		private void addGateWay_MI_Click(object sender, RoutedEventArgs e)
		{
			MainWindow.Interfaces_List_LB.Dispatcher.BeginInvoke((Action)(() =>
					MainWindow.addPingList(IPv4Gateway)
				));
			MainWindow.log(String.Format("GateWay {0}, adapter {1} added", IPv4Gateway, Name));
		}
	}
}
