using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Linq;

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
    public static bool IsDigitsOnly(string str)
    {
        foreach (char c in str)
        {
            if (c < '0' || c > '9')
                return false;
        }

        return true;
    }
}

public class UDPListener
{

    static UdpClient listener = new UdpClient(listenPort);
    static IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, listenPort);
    static List<string> IDs = new List<string>();
    private const int listenPort = 9333;
    static string ID = "0";
    static double number1 = 0;
    static double number2 = 0;
    static bool error = false;
    static byte[] send_buffer;
    static bool done = false;


    static string receivedData;
    static string answer;
    static string operation;
    static string[] syncMessages = new string[3];
    static int SyncN = 0;
    static byte[] receiveByteArray;
    static string sessionID = "0";
    private static Random random = new Random((int)DateTime.Now.Ticks);
    public static string RandomString(int length)
    {
        const string chars = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz0123456789";
        return new string(Enumerable.Repeat(chars, length)
          .Select(s => s[random.Next(s.Length)]).ToArray());
    }
    static string IdDraw()
    {
        // get 1st random string 
        string Rand1 = RandomString(10);
        string ID = Rand1;
        if (IDs.Contains(ID))
            return IdDraw();
        else
        {
            IDs.Add(ID);
            return ID;
        }
    }
    public static double Factorial(double firstNumber)
    {
        if (firstNumber == 0 || firstNumber == 1) return 1;
        else return firstNumber * Factorial(firstNumber - 1);
    }

    public static List<string> OP = new List<string>();
    public static List<string> N1 = new List<string>();
    public static List<string> N2 = new List<string>();
    public static List<string> R = new List<string>();


    public static void History(string receivedData)
    {
        string message;
        int SyncN;
        byte[] sendEndInfo;
        string HID = extensions.FindingInfo("ID", receivedData);
        bool IDExist = false; ;
        int whichOperation = 0;
        foreach (string x in IDs)
        {
            if (HID == x)
            {
                IDExist = true;
                whichOperation = IDs.IndexOf(x);
            }
        }
        bool fac = false;
        if (whichOperation != 0) if (OP[whichOperation - 2] == "Factorial") fac = true;
        string info = "";
        if (HID == sessionID)
        {
            Console.WriteLine("in");
            SyncN = OP.Count * 4;
            for (int i = 0; i < OP.Count; i++) if (OP[i] == "Factorial") SyncN--;

            message = "TI: " + DateTime.Now.ToString() + " SN: " + SyncN-- + " ID: " + ID + " OP: history";
            sendEndInfo = Encoding.ASCII.GetBytes(message);
            listener.Send(sendEndInfo, sendEndInfo.Length, remoteEP);

            for (int i = 0; i < OP.Count; i++)
            {
                if (OP[i] == "Factorial")
                {
                    for (int j = 0; j < 3; j++)
                    {
                        info = "";
                        if (j == 0) info += " HOP: " + OP[i] + " ";
                        else if (j == 1) info += " HN1: " + N1[i] + " ";
                        else if (j == 2) info += " HR: " + R[i] + " ";
                        message = "TI: " + DateTime.Now.ToString() + " SN: " + SyncN-- + " ID: " + ID + info;
                        sendEndInfo = Encoding.ASCII.GetBytes(message);
                        listener.Send(sendEndInfo, sendEndInfo.Length, remoteEP);
                    }
                }
                else
                {
                    for (int j = 0; j < 4; j++)
                    {
                        info = "";
                        if (j == 0) info += " HOP: " + OP[i] + " ";
                        else if (j == 1) info += " HN1: " + N1[i] + " ";
                        else if (j == 2) info += " HN2: " + N2[i] + " ";
                        else if (j == 3) info += " HR: " + R[i] + " ";
                        message = "TI: " + DateTime.Now.ToString() + " SN: " + SyncN-- + " ID: " + ID + info;
                        sendEndInfo = Encoding.ASCII.GetBytes(message);
                        listener.Send(sendEndInfo, sendEndInfo.Length, remoteEP);
                    }
                }
            }
        }
        else if (IDExist)
        {
            if (fac)
            {
                SyncN = 3;
                message = "TI: " + DateTime.Now.ToString() + " SN: " + SyncN-- + " ID: " + ID + " OP: history";
                sendEndInfo = Encoding.ASCII.GetBytes(message);
                listener.Send(sendEndInfo, sendEndInfo.Length, remoteEP);

                for (int j = 0; j < 3; j++)
                {
                    info = "";
                    if (j == 0) info += " HOP: " + OP[whichOperation - 2] + " ";
                    else if (j == 1) info += " HN1: " + N1[whichOperation - 2] + " ";
                    else if (j == 2) info += " HR: " + R[whichOperation - 2] + " ";
                    message = "TI: " + DateTime.Now.ToString() + " SN: " + SyncN-- + " ID: " + ID + info;
                    Console.WriteLine(message);
                    sendEndInfo = Encoding.ASCII.GetBytes(message);
                    listener.Send(sendEndInfo, sendEndInfo.Length, remoteEP);
                }
            }
            else
            {
                SyncN = 4;
                message = "TI: " + DateTime.Now.ToString() + " SN: " + SyncN-- + " ID: " + ID + " OP: history";
                sendEndInfo = Encoding.ASCII.GetBytes(message);
                listener.Send(sendEndInfo, sendEndInfo.Length, remoteEP);

                for (int j = 0; j < 4; j++)
                {
                    info = "";
                    if (j == 0) info += " HOP: " + OP[whichOperation - 2] + " ";
                    else if (j == 1) info += " HN1: " + N1[whichOperation - 2] + " ";
                    else if (j == 2) info += " HN2: " + N2[whichOperation - 2] + " ";
                    else if (j == 3) info += " HR: " + R[whichOperation - 2] + " ";
                    message = "TI: " + DateTime.Now.ToString() + " SN: " + SyncN-- + " ID: " + ID + info;
                    Console.WriteLine(message);
                    sendEndInfo = Encoding.ASCII.GetBytes(message);
                    listener.Send(sendEndInfo, sendEndInfo.Length, remoteEP);
                }
            }

        }
        else
        {
            message = "TI: " + DateTime.Now.ToString() + " SN: 0" + " ID: " + ID + " ST: Wrong ID ";
            sendEndInfo = Encoding.ASCII.GetBytes(message);
            listener.Send(sendEndInfo, sendEndInfo.Length, remoteEP);
        }
    }

    public static int Main()
    {
        try
        {
            Console.Write("Waiting for broadcast:");
            receiveByteArray = listener.Receive(ref remoteEP);
            receivedData = Encoding.ASCII.GetString(receiveByteArray, 0, receiveByteArray.Length);

            if (receivedData.Contains("ST: IDRequest"))
            {
                Console.Clear();

                Console.WriteLine("Received: " + receivedData);
                sessionID = IdDraw();
                ID = extensions.FindingInfo("ID", receivedData);
                IDs.Add(ID);
                answer = "TI: " + DateTime.Now.ToString() + " SN: 0 ID: " + ID + " ST: " + sessionID + " ";
                send_buffer = Encoding.ASCII.GetBytes(answer);
                listener.Send(send_buffer, send_buffer.Length, remoteEP);
                Console.WriteLine("Response: " + answer);
            }
            while (!done)
            {

                receiveByteArray = listener.Receive(ref remoteEP);
                receivedData = Encoding.ASCII.GetString(receiveByteArray, 0, receiveByteArray.Length);
                Console.WriteLine("Received: " + receivedData);
                SyncN = Convert.ToInt32(extensions.FindingInfo("SN", receivedData));
                if (receivedData.Contains("ST: Shutdowning server"))
                {

                    answer = "TI: " + DateTime.Now.ToString() + " SN: 0 ID: " + ID + " ST: Server is shutdown. Goodbye!;\n";
                    send_buffer = Encoding.ASCII.GetBytes(answer);
                    listener.Send(send_buffer, send_buffer.Length, remoteEP);
                    Console.WriteLine("Server is shutdown. Goodbye!");
                    listener.Close();
                    break;
                }
                syncMessages[0] = receivedData;
                if (SyncN != 0)
                {

                    int i = 1;
                    do
                    {
                        receiveByteArray = listener.Receive(ref remoteEP);
                        receivedData = Encoding.ASCII.GetString(receiveByteArray, 0, receiveByteArray.Length);
                        Console.WriteLine("Received: " + receivedData);
                        SyncN = Convert.ToInt32(extensions.FindingInfo("SN", receivedData));
                        syncMessages[i] = receivedData;
                        i++;
                    } while (SyncN > 0);
                }
                ID = extensions.FindingInfo("ID", receivedData);

                operation = extensions.FindingInfo("OP", syncMessages[0]);
                // Console.WriteLine(operation);
                double result = 0;
                try
                {
                    switch (operation)
                    {
                        case "Multiplication":
                            {

                                IDs.Add(ID);
                                number1 = Convert.ToDouble(extensions.FindingInfo("N1", syncMessages[1]));
                                if (Double.IsPositiveInfinity(number1)) throw new System.Exception("First number is too big ");
                                    number2 = Convert.ToDouble(extensions.FindingInfo("N2", syncMessages[2]));
                                if (Double.IsPositiveInfinity(number2)) throw new System.Exception("Second number is too big ");
                                if (Double.IsPositiveInfinity(number2)&& Double.IsPositiveInfinity(number1)) throw new System.Exception("Both numbers are too big ");
                                result = number1 * number2;
                                if (Double.IsPositiveInfinity(result)) throw new System.Exception("Result is out of range (positive infinity) ");
                                if (Double.IsNegativeInfinity(result)) throw new System.Exception("Result is out of range (negative infinity) ");
                                answer = "TI: " + DateTime.Now.ToString() + " SN: 0 ID: " + ID + " RE: " + result.ToString() + "\n";
                                send_buffer = Encoding.ASCII.GetBytes(answer);
                                listener.Send(send_buffer, send_buffer.Length, remoteEP);
                                Console.WriteLine("Response: " + answer);

                                break;
                            }
                        case "Division":
                            {
                                IDs.Add(ID);
                                number1 = Convert.ToDouble(extensions.FindingInfo("N1", syncMessages[1]));
                                if (Double.IsPositiveInfinity(number1)) throw new System.Exception("First number is too big ");
                                number2 = Convert.ToDouble(extensions.FindingInfo("N2", syncMessages[2]));
                                if (number2 == 0) throw new System.Exception("Division by zero");
                                if (Double.IsPositiveInfinity(number2)) throw new System.Exception("Second number is too big ");
                                if (Double.IsPositiveInfinity(number2) && Double.IsPositiveInfinity(number1)) throw new System.Exception("Both numbers are too big ");
                                result = number1 / number2;
                                if (Double.IsPositiveInfinity(result)) throw new System.Exception("Result is out of range (positive infinity) ");
                                if (Double.IsNegativeInfinity(result)) throw new System.Exception("Result is out of range (negative infinity) ");
                                


                                answer = "TI: " + DateTime.Now.ToString() + " SN: 0 ID: " + ID + " RE: " + result.ToString() + "\n";
                                send_buffer = Encoding.ASCII.GetBytes(answer);
                                listener.Send(send_buffer, send_buffer.Length, remoteEP);
                                Console.WriteLine("Response: " + answer);
                                break;
                            }
                        case "Addition":
                            {
                                IDs.Add(ID);
                                number1 = Convert.ToDouble(extensions.FindingInfo("N1", syncMessages[1]));
                                if (Double.IsPositiveInfinity(number1)) throw new System.Exception("First number is too big ");
                                number2 = Convert.ToDouble(extensions.FindingInfo("N2", syncMessages[2]));
                                if (Double.IsPositiveInfinity(number2)) throw new System.Exception("Second number is too big ");
                                if (Double.IsPositiveInfinity(number2) && Double.IsPositiveInfinity(number1)) throw new System.Exception("Both numbers are too big ");
                                result = number1 + number2;
                                if (Double.IsPositiveInfinity(result)) throw new System.Exception("Result is out of range (positive infinity) ");
                                if (Double.IsNegativeInfinity(result)) throw new System.Exception("Result is out of range (negative infinity) ");

                                answer = "TI: " + DateTime.Now.ToString() + " SN: 0 ID: " + ID + " RE: " + result.ToString() + "\n";
                                send_buffer = Encoding.ASCII.GetBytes(answer);
                                listener.Send(send_buffer, send_buffer.Length, remoteEP);
                                Console.WriteLine("Response: " + answer);
                                break;
                            }
                        case "Subtraction":
                            {
                                IDs.Add(ID);
                                number1 = Convert.ToDouble(extensions.FindingInfo("N1", syncMessages[1]));
                                if (Double.IsPositiveInfinity(number1)) throw new System.Exception("First number is too big ");
                                number2 = Convert.ToDouble(extensions.FindingInfo("N2", syncMessages[2]));
                                if (Double.IsPositiveInfinity(number2)) throw new System.Exception("Second number is too big ");
                                if (Double.IsPositiveInfinity(number2) && Double.IsPositiveInfinity(number1)) throw new System.Exception("Both numbers are too big ");
                                result = number1 - number2;
                                if (Double.IsPositiveInfinity(result)) throw new System.Exception("Result is out of range (positive infinity) ");
                                if (Double.IsNegativeInfinity(result)) throw new System.Exception("Result is out of range (negative infinity) ");

                                answer = "TI: " + DateTime.Now.ToString() + " SN: 0 ID: " + ID + " RE: " + result.ToString() + "\n";
                                send_buffer = Encoding.ASCII.GetBytes(answer);
                                listener.Send(send_buffer, send_buffer.Length, remoteEP);
                                Console.WriteLine("Response: " + answer);
                                break;
                            }
                        case "Factorial":
                            {
                                IDs.Add(ID);
                                number1 = Convert.ToDouble(extensions.FindingInfo("N1", syncMessages[1]));
                                if(number1<0) throw new System.Exception("Number is negative");
                                if (Double.IsPositiveInfinity(number1)) throw new System.Exception("Number is too big ");
                                string factTest = number1.ToString();
                                if(!extensions.IsDigitsOnly(factTest)) throw new System.Exception("Number isn't natural");
                                result = Factorial(number1);
                                if (Double.IsPositiveInfinity(result)) throw new System.Exception("Result is too out of range (positive infinity) ");
                                answer = "TI: " + DateTime.Now.ToString() + " SN: 0 ID: " + ID + " RE: " + result.ToString() + "\n";
                                send_buffer = Encoding.ASCII.GetBytes(answer);
                                listener.Send(send_buffer, send_buffer.Length, remoteEP);
                                Console.WriteLine("Response: " + answer);
                                break;
                            }
                        case "History":
                            {
                                History(receivedData);
                                break;
                            }
                        default:
                            {

                                answer = "TI: " + DateTime.Now.ToString() + " SN: 0 ID: " + ID + " ST:  Wrong type of operation. Try again later.\n";
                                send_buffer = Encoding.ASCII.GetBytes(answer);
                                listener.Send(send_buffer, send_buffer.Length, remoteEP);
                                Console.WriteLine("Response: " + answer);
                                break;
                            }
                    }
                    if (operation != "History")
                    {
                        OP.Add(operation);
                        N1.Add(number1.ToString());
                        N2.Add(number2.ToString());
                        R.Add(result.ToString());
                    }


                }
                catch (FormatException letterInsteadOfNumber)
                {
                    OP.Add(operation);
                    N1.Add(number1.ToString());
                    N2.Add(number2.ToString());
                    R.Add("Wrong character instead of number");
                    answer = "TI: " + DateTime.Now.ToString() + " SN: 0 ID: " + ID + " ST: Wrong character instead of number ";
                    send_buffer = Encoding.ASCII.GetBytes(answer);
                    listener.Send(send_buffer, send_buffer.Length, remoteEP);
                    Console.WriteLine("Response: " + answer);
                }
                catch (Exception ex)
                {
                    OP.Add(operation);
                    N1.Add(number1.ToString());
                    N2.Add(number2.ToString());
                    R.Add(ex.Message);
                    answer = "TI: " + DateTime.Now.ToString() + " SN: 0 ID: " + ID + " ST: " + ex.Message + " ";
                    send_buffer = Encoding.ASCII.GetBytes(answer);
                    listener.Send(send_buffer, send_buffer.Length, remoteEP);
                    Console.WriteLine("Response: " + answer);

                };



            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
        listener.Close();
        return 0;
    }
} // end of class UDPListener