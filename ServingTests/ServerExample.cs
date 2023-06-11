using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server{
    class Program{
		private static void changeColor(){
			ConsoleColor temp = Console.BackgroundColor;
			Console.BackgroundColor = Console.ForegroundColor;
			Console.ForegroundColor = temp;
		}
		
        private static void sendPurchaseOrder(Socket c, string fName){
			Console.WriteLine("Server sending 850 PO to Client");
			c.SendFile(fName,null,null,TransmitFileOptions.UseDefaultWorkerThread);
			Console.WriteLine("\t*Server has sent the 850 sample EDI to Client");
		}
		
		private static string receiveConfirmation(Socket c){
			byte[] fileReceived = new byte[1024];
			int fileBytes = c.Receive(fileReceived);
			return Encoding.ASCII.GetString(fileReceived,0,fileBytes);
		}
		
		private static void ExecuteServer(string fileName){
			IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
			IPAddress ipAdd = ipHost.AddressList[0];
			IPEndPoint localEp = new IPEndPoint(ipAdd,5050);
			Socket listener = new Socket(ipAdd.AddressFamily,SocketType.Stream,ProtocolType.Tcp);
			try{
				listener.Bind(localEp);
				listener.Listen(10);
				while(true){
					Console.WriteLine("Server awaiting a Client connection...");
					Socket client = listener.Accept();
					sendPurchaseOrder(client,fileName);          //Send 850 PO to Client
					string ediStr = receiveConfirmation(client); //Receive 855 Confirmation from Client as string
					Console.WriteLine("\t*855 Confirmation received:\n\n"+ediStr);
					Console.WriteLine("Transaction Complete!\n\t*Server closing connection with Client\n");
					client.Shutdown(SocketShutdown.Both);
					client.Close();
					changeColor();
				}
			}catch(Exception e){
				Console.WriteLine(e.ToString());
			}
		}
		
		public static void Main(string[] args){
            ExecuteServer("C:\\Code\\cSharp\\sample_850.txt");
        }
    }
}