using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client{
    class Program{
        private static void sendOrderConfirmation(Socket s, string fName){
			Console.WriteLine("\nClient sending 855 Confirmation to Server");
			s.SendFile(fName);
			Console.WriteLine("\t*Client has set 855 Confirmation to Server");
		}
		
		private static string receivePurchaseOrder(Socket s){
			Console.WriteLine("Receiving an 850 PO from the Server");
			byte[] fileReceived = new byte[1024];
			int fileBytes = s.Receive(fileReceived);
			return Encoding.ASCII.GetString(fileReceived,0,fileBytes);
		}
		
		private static void ExecuteClient(string fileName){
				try{
					IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
					IPAddress ipAdd = ipHost.AddressList[0];
					IPEndPoint localEp = new IPEndPoint(ipAdd,5050);
					Socket server = new Socket(ipAdd.AddressFamily,SocketType.Stream,ProtocolType.Tcp);
					try{
						server.Connect(localEp);
						Console.WriteLine("Client requested to connect to Server at: "+server.RemoteEndPoint.ToString());
						string ediStr = receivePurchaseOrder(server);                   //Receive 850 PO from Server as string
						Console.WriteLine("\t*850 PO received:\n\n"+ediStr);
						sendOrderConfirmation(server,fileName);                         //Send 855 Confirmation .txt file to Server
						Console.WriteLine("Client closing connection with the Server");
						server.Shutdown(SocketShutdown.Both);
						server.Close();
					}catch(ArgumentNullException ane){
						Console.WriteLine("ArgumentNullException: {0}",ane.ToString());
					}catch(SocketException se){
						Console.WriteLine("SocketException: {0}",se.ToString());
					}catch(Exception e){
						Console.WriteLine("Unexpected Exception: {0}",e.ToString());
					}
				}catch(Exception e){
						Console.WriteLine(e.ToString());
				}
		}
		
		public static void Main(string[] args){
            ExecuteClient("C:\\Code\\cSharp\\sample_855.txt");
        }
    }
}