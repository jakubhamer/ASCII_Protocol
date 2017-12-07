using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
public static class extensions
{
    public static int IndexOfNth(this string str, string value, int nth = 1)
    {
        if (nth <= 0)
            throw new ArgumentException("Can not find the zeroth index of substring in string. Must start with 1");
        int offset = str.IndexOf(value);
        for (int i = 1; i < nth; i++)
        {
            if (offset == -1) return -1;
            offset = str.IndexOf(value, offset + 1);
        }
        return offset;
    }
    public static string FindingInfo(string type, string message)
    {
        string info = "";
        int index;
        string firstChar = "";
        int i = 1;
        int questionTypeSize = 4;
        index = extensions.IndexOfNth(message, type + ": ", 1);
        firstChar = message.Substring(index + questionTypeSize, 1);
        while (firstChar != " ")
        {
            info += firstChar;
            firstChar = message.Substring(index + questionTypeSize + i, 1);
            i++;
        }
        return info;
    }
}

class UDPClient
{
    static List<string> IDs = new List<string>();
    static string sessionID = "";
    static string ID = "";
    static int SeqN = 0; // Sequence number.
    public static void ID_Definition(UdpClient client, IPEndPoint ep) // Nadanie ID.
    {
        ID = IdDraw(20);
        byte[] sendEndInfo = Encoding.ASCII.GetBytes("TI: " + DateTime.Now.ToString() + " SN: 0 ID: " + ID + " ST: IDRequest ");
        client.Send(sendEndInfo, sendEndInfo.Length);
        byte[] receiveByteArray = client.Receive(ref ep);
        string receivedData = Encoding.ASCII.GetString(receiveByteArray, 0, receiveByteArray.Length);
        sessionID = extensions.FindingInfo("ST", receivedData);
    }
    private static Random random = new Random((int)DateTime.Now.Ticks);

    public static string RandomString(int length)
    {
        const string chars = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz0123456789";
        return new string(Enumerable.Repeat(chars, length)
          .Select(s => s[random.Next(s.Length)]).ToArray());
    }
    static string IdDraw(int size)
    {
        // get 1st random string 
        string Rand1 = RandomString(size);
        string ID = Rand1;
        if (IDs.Contains(ID))
            return IdDraw(size);
        else
        {
            IDs.Add(ID);
            return ID;
        }
    }

    public static void Multiplication(UdpClient client)
    {
        SeqN = 2;
        string[] message = new string[3];
        message[0] = "TI: " + DateTime.Now.ToString() + " SN: " + SeqN + " ID: " + ID + " OP: Multiplication ";
        SeqN--;
        Console.Write("Write multiplicand & multiplier:");
        string multiplicand = Console.ReadLine();
        string multiplier = Console.ReadLine();
        message[1] = "TI: " + DateTime.Now.ToString() + " SN: " + SeqN + " ID: " + ID + " N1: " + multiplicand + " ";
        SeqN--;
        message[2] = "TI: " + DateTime.Now.ToString() + " SN: " + SeqN + " ID: " + ID + " N2: " + multiplier + " ";

        for (int i = 0; i <= 2; i++)
        {
            byte[] sendEndInfo = Encoding.ASCII.GetBytes(message[i]);
            client.Send(sendEndInfo, sendEndInfo.Length);
        }

    }
    public static void Division(UdpClient client)
    {
        SeqN = 2;
        string[] message = new string[3];
        message[0] = "TI: " + DateTime.Now.ToString() + " SN: " + SeqN + " ID: " + ID + " OP: Division ";
        SeqN--;
        Console.Write("Write dividend & divisor:");
        string dividend = Console.ReadLine();
        string divisor = Console.ReadLine();
        message[1] = "TI: " + DateTime.Now.ToString() + " SN: " + SeqN + " ID: " + ID + " N1: " + dividend + " ";
        SeqN--;
        message[2] = "TI: " + DateTime.Now.ToString() + " SN: " + SeqN + " ID: " + ID + " N2: " + divisor + " ";

        for (int i = 0; i <= 2; i++)
        {
            byte[] sendEndInfo = Encoding.ASCII.GetBytes(message[i]);
            client.Send(sendEndInfo, sendEndInfo.Length);
        }


    }
    public static void Addition(UdpClient client)
    {
        SeqN = 2;
        string[] message = new string[3];
        message[0] = "TI: " + DateTime.Now.ToString() + " SN: " + SeqN + " ID: " + ID + " OP: Addition ";
        SeqN--;
        Console.Write("Write two summands:");
        string summand1 = Console.ReadLine();
        string summand2 = Console.ReadLine();
        message[1] = "TI: " + DateTime.Now.ToString() + " SN: " + SeqN + " ID: " + ID + " N1: " + summand1 + " ";
        SeqN--;
        message[2] = "TI: " + DateTime.Now.ToString() + " SN: " + SeqN + " ID: " + ID + " N2: " + summand2 + " ";

        for (int i = 0; i <= 2; i++)
        {
            byte[] sendEndInfo = Encoding.ASCII.GetBytes(message[i]);
            client.Send(sendEndInfo, sendEndInfo.Length);
        }
    }
    public static void Subtraction(UdpClient client)
    {
        SeqN = 2;
        string[] message = new string[3];
        message[0] = "TI: " + DateTime.Now.ToString() + " SN: " + SeqN + " ID: " + ID + " OP: Subtraction ";
        SeqN--;
        Console.Write("Write minuend & subtrahend:");
        string minuend = Console.ReadLine();
        string subtrahend = Console.ReadLine();
        message[1] = "TI: " + DateTime.Now.ToString() + " SN: " + SeqN + " ID: " + ID + " N1: " + minuend + " ";
        SeqN--;
        message[2] = "TI: " + DateTime.Now.ToString() + " SN: " + SeqN + " ID: " + ID + " N2: " + subtrahend + " ";

        for (int i = 0; i <= 2; i++)
        {
            byte[] sendEndInfo = Encoding.ASCII.GetBytes(message[i]);
            client.Send(sendEndInfo, sendEndInfo.Length);
        }
    }
    public static void Factorial(UdpClient client)
    {
        SeqN = 1;
        string[] message = new string[2];
        message[0] = "TI: " + DateTime.Now.ToString() + " SN: " + SeqN + " ID: " + ID + " OP: Factorial ";
        SeqN--;
        Console.Write("Write number for factorial:");
        string argument = Console.ReadLine();
        message[1] = "TI: " + DateTime.Now.ToString() + " SN: " + SeqN + " ID: " + ID + " N1: " + argument + " ";
        SeqN--;

        for (int i = 0; i <= 1; i++)
        {
            byte[] sendEndInfo = Encoding.ASCII.GetBytes(message[i]);
            client.Send(sendEndInfo, sendEndInfo.Length);
        }

    }
    public static void History(UdpClient client)
    {
        string message;
        Console.Write("Write ID of operations you would like to search for (\"your session ID\" for all operations): ");
        string ID = Console.ReadLine();
        message = "TI: " + DateTime.Now.ToString() + " SN: 0 ID: " + ID + " OP: History ";
        byte[] sendEndInfo = Encoding.ASCII.GetBytes(message);
        client.Send(sendEndInfo, sendEndInfo.Length);
    }



    static void Main(string[] args)
    {
        Console.Write("Press any key to start");
        Console.ReadLine();
        Console.Clear();
        UdpClient client = new UdpClient();
        Console.WriteLine("Enter IP adrress you would like to send message: ");
        string ipAdrress = "127.0.0.1";//Console.ReadLine();
        Console.WriteLine("Enter port you would like to send message: ");
        int port = 9333;//Int32.Parse(Console.ReadLine());
        IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ipAdrress), port); // endpoint where server is listening
        client.Connect(ep);
        string receivedData;
        byte[] receiveByteArray;

        Boolean done = false;
        ID_Definition(client, ep); // Nadanie ID.
        Console.Clear();
        while (!done)
        {

            {
                Console.WriteLine("Your ID: " + sessionID);
                Console.WriteLine("\nIDs from your search history: ");
                for (int i=1;i<IDs.Count;i++)
                {
                    Console.WriteLine(IDs[i]);
                }
                Console.WriteLine("\nChoose which operation you would like to do:");
                Console.WriteLine("1 -> Multiplication");
                Console.WriteLine("2 -> Division");
                Console.WriteLine("3 -> Addition");
                Console.WriteLine("4 -> Subtraction");
                Console.WriteLine("5 -> Factorial");
                Console.WriteLine("6 -> History");
                Console.WriteLine("7 -> End of conversation");
            }
            int choice;
            try
            {
                choice = Convert.ToInt32(Console.ReadLine());
            }
            catch (FormatException letterInsteadOfNumber)
            {
                Console.WriteLine("Exception: wrong character instead of number. You must write number.");
                Console.WriteLine("Try once again. Press any key to continue.");
                Console.ReadLine();
                Console.Clear();
                continue;
            }

            switch (choice)
            {
                case 1:
                    ID = IdDraw(20);
                    Multiplication(client);
                    break;
                case 2:
                    ID = IdDraw(20);
                    Division(client);
                    break;
                case 3:
                    ID = IdDraw(20);
                    Addition(client);
                    break;
                case 4:
                    ID = IdDraw(20);
                    Subtraction(client);
                    break;
                case 5:
                    ID = IdDraw(20);
                    Factorial(client);
                    break;
                case 6:
                    History(client);
                    break;
                case 7:
                    done = true;
                    byte[] sendEndInfo = Encoding.ASCII.GetBytes("TI: " + DateTime.Now.ToString() + " SN: 0 ID: " + ID + " ST: Shutdowning server ");
                    client.Send(sendEndInfo, sendEndInfo.Length);
                    receiveByteArray = client.Receive(ref ep);
                    receivedData = Encoding.ASCII.GetString(receiveByteArray, 0, receiveByteArray.Length);
                    Console.WriteLine("Answer from server: " + receivedData);
                    return;
                default:
                    // sendEndInfo = Encoding.ASCII.GetBytes("TI: " + DateTime.Now.ToString() + " SN: 0 ID: " + ID + " OP: " );
                    //client.Send(sendEndInfo, sendEndInfo.Length);
                    Console.WriteLine("Try once again. Press any key to continue.");
                    Console.ReadLine();
                    Console.Clear();
                    continue;

            }
            receiveByteArray = client.Receive(ref ep);
            receivedData = Encoding.ASCII.GetString(receiveByteArray, 0, receiveByteArray.Length);
            int SyncN = Convert.ToInt32(extensions.FindingInfo("SN", receivedData));
            string[] syncMessages = new string[SyncN+1];
            Console.WriteLine("Answer from server: " + receivedData);
            if (SyncN != 0)
            {
                do
                {
                    receiveByteArray = client.Receive(ref ep);
                    receivedData = Encoding.ASCII.GetString(receiveByteArray, 0, receiveByteArray.Length);
                    SyncN = Convert.ToInt32(extensions.FindingInfo("SN", receivedData));
                    Console.WriteLine("Answer from server: " + receivedData);

                } while (SyncN > 0);
            }
            
                Console.WriteLine("Press any key to contiune...");

            Console.ReadLine();

            Console.Clear();
        } // end of while (!done)
    } // end of main()
} // end of class Program
