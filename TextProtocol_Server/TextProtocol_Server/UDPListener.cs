using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Linq;

// Klasa zawierająca 3 pomocnicze metody potrzebne do realizacji zadania.
public static class Extensions
{
    // Zwracanie wartości indeksu gdzie rozpoczyna się zadana fraza.
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

    // Znajac indeks rozpoczęcia się danej frazy zwraca poszukiwany tekst, który znajduje się po zadanej frazie.
    //Służy do szukania wartości pól w komunikacie.
    public static string FindingInfo(string type, string message)

    {
        string information = "";
        int index;
        string firstChar = "";
        int i = 1;
        int questionTypeSize = 4;
        index = IndexOfNth(message, type + ": ", 1);
        firstChar = message.Substring(index + questionTypeSize, 1);
        while (firstChar != " ")
        {
            information += firstChar;
            firstChar = message.Substring(index + questionTypeSize + i, 1);
            i++;
        }
        return information;

    }

    // Metoda sprawdza czy string zawiera tylko cyfry.
    public static bool IsDigitsOnly(string str)
    {
        foreach (char c in str)
        {
            if (c < '0' || c > '9')
                return false;
        }

        return true;
    }
} // End of class extensions

public class UDPListener
{

    static UdpClient listener = new UdpClient(listenPort); // Definicja nowego obiektu, którym jest serwer.
    static IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, listenPort);  // Definicja struktury odpowiedzialnej za port i adres IP serwera.
    static List<string> IDs = new List<string>(); // Lista identyfikatorów.
    private const int listenPort = 9333; // Ustawienei portu.
    static string ID = "0";
    static double number1 = 0;
    static double number2 = 0;
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

    // Listy zapiusjące kolejne pola z każdego komunikatu.
    public static List<string> OP = new List<string>();
    public static List<string> N1 = new List<string>();
    public static List<string> N2 = new List<string>();
    public static List<string> R = new List<string>();


    public static void History(string receivedData)
    {
        string message; // Komunikat do wysłania.
        int SyncN; // Zmienna służąca do ustawiania numeru senkwencyjnego.
        byte[] sendEndInfo; // Komunikat do wysłania w zmienne byte. 
        string HID = Extensions.FindingInfo("ID", receivedData); // ID konkretnej operacji lub sesji.
        bool IDExist = false; // Zmienna slużąca do sprawdzania czy dane ID istnieje.
        int whichOperation = 0; // Miejsce w tablicy ID gdzie występuje ID silni.
        foreach (string x in IDs) // Szukanie ID.
        {
            if (HID == x)
            {
                IDExist = true;
                whichOperation = IDs.IndexOf(x);
            }
        }
        bool fac = false; // Zmienna slużąca do sprawdzenia czy w historii mamy silnie, jesli tak ulegnie zmianie numer sekswencyjny i przesyłane dane.
        if (whichOperation != 0) if (OP[whichOperation - 2] == "Factorial") fac = true;
        string information = "";
        if (HID == sessionID) // Wyświetlenie całej historii.
        {
            SyncN = OP.Count * 4; // Każda operacja to 4 komunikaty do przesłania: operacja, liczba 1, liczba 2, wynik.
            for (int i = 0; i < OP.Count; i++) if (OP[i] == "Factorial") SyncN--; // Silnia posiada jedna liczbę, więc każde wystąpienie silnii to jeden komunikat (numer sekwencyjny) mniej.

            message = "TI: " + DateTime.Now.ToString() + " NS: " + SyncN-- + " ID: " + ID + " OP: history"; // Komunikat informujący o wyświetleniu historii.
            sendEndInfo = Encoding.ASCII.GetBytes(message);
            listener.Send(sendEndInfo, sendEndInfo.Length, remoteEP); // Wysłanie komunikatu.

            for (int i = 0; i < OP.Count; i++)
            {
                if (OP[i] == "Factorial") // Oddzielny przypadek dla silni - bo jedna liczba.
                {
                    for (int j = 0; j < 3; j++)
                    {
                        information = "";
                        if (j == 0) information += " HOP: " + OP[i] + " ";
                        else if (j == 1) information += " HN1: " + N1[i] + " ";
                        else if (j == 2) information += " HR: " + R[i] + " ";
                        message = "TI: " + DateTime.Now.ToString() + " NS: " + SyncN-- + " ID: " + ID + information;
                        sendEndInfo = Encoding.ASCII.GetBytes(message);
                        listener.Send(sendEndInfo, sendEndInfo.Length, remoteEP); // Wysłanie komunikatu.
                    }
                }
                else
                {
                    for (int j = 0; j < 4; j++)
                    {
                        information = "";
                        if (j == 0) information += " HOP: " + OP[i] + " ";
                        else if (j == 1) information += " HN1: " + N1[i] + " ";
                        else if (j == 2) information += " HN2: " + N2[i] + " ";
                        else if (j == 3) information += " HR: " + R[i] + " ";
                        message = "TI: " + DateTime.Now.ToString() + " NS: " + SyncN-- + " ID: " + ID + information;
                        sendEndInfo = Encoding.ASCII.GetBytes(message);
                        listener.Send(sendEndInfo, sendEndInfo.Length, remoteEP); // Wysłanie komunikatu.
                    }
                }
            }
        }
        else if (IDExist) // Wyświetlenie pojedynczej operacji.
        {
            if (fac) // Oddzielny przypadek dla silni - bo jedna liczba.
            {
                SyncN = 3;
                message = "TI: " + DateTime.Now.ToString() + " NS: " + SyncN-- + " ID: " + ID + " OP: history";
                sendEndInfo = Encoding.ASCII.GetBytes(message);
                listener.Send(sendEndInfo, sendEndInfo.Length, remoteEP); // Wysłanie komunikatu.

                for (int j = 0; j < 3; j++)
                {
                    information = "";
                    if (j == 0) information += " HOP: " + OP[whichOperation - 2] + " ";
                    else if (j == 1) information += " HN1: " + N1[whichOperation - 2] + " ";
                    else if (j == 2) information += " HR: " + R[whichOperation - 2] + " ";
                    message = "TI: " + DateTime.Now.ToString() + " NS: " + SyncN-- + " ID: " + ID + information;
                    Console.WriteLine(message);
                    sendEndInfo = Encoding.ASCII.GetBytes(message);
                    listener.Send(sendEndInfo, sendEndInfo.Length, remoteEP); // Wysłanie komunikatu.
                }
            }
            else // Reszta operacji jest na dwóch liczbach.
            {
                SyncN = 4;
                message = "TI: " + DateTime.Now.ToString() + " NS: " + SyncN-- + " ID: " + ID + " OP: history";
                sendEndInfo = Encoding.ASCII.GetBytes(message);
                listener.Send(sendEndInfo, sendEndInfo.Length, remoteEP); // Wysłanie komunikatu.

                for (int j = 0; j < 4; j++)
                {
                    information = "";
                    if (j == 0) information += " HOP: " + OP[whichOperation - 2] + " ";
                    else if (j == 1) information += " HN1: " + N1[whichOperation - 2] + " ";
                    else if (j == 2) information += " HN2: " + N2[whichOperation - 2] + " ";
                    else if (j == 3) information += " HR: " + R[whichOperation - 2] + " ";
                    message = "TI: " + DateTime.Now.ToString() + " NS: " + SyncN-- + " ID: " + ID + information;
                    Console.WriteLine(message);
                    sendEndInfo = Encoding.ASCII.GetBytes(message);
                    listener.Send(sendEndInfo, sendEndInfo.Length, remoteEP); // Wysłanie komunikatu.
                }
            }

        }
        else // Przypadek dla podania złego ID.
        {
            message = "TI: " + DateTime.Now.ToString() + " NS: 0" + " ID: " + ID + " ST: Wrong ID ";
            sendEndInfo = Encoding.ASCII.GetBytes(message);
            listener.Send(sendEndInfo, sendEndInfo.Length, remoteEP); // Wysłanie komunikatu.
        }
    }

    public static int Main()
    {
        Console.Write("Waiting for broadcast:");
        receiveByteArray = listener.Receive(ref remoteEP); // Nasłuchiwanie komunikatu.
        receivedData = Encoding.ASCII.GetString(receiveByteArray, 0, receiveByteArray.Length);

        if (receivedData.Contains("ST: IDRequest"))
        {
            Console.Clear();

            Console.WriteLine("Received: " + receivedData);
            sessionID = IdDraw(); // Losowanie ID sesji.
            ID = Extensions.FindingInfo("ID", receivedData);
            IDs.Add(ID);
            answer = "TI: " + DateTime.Now.ToString() + " NS: 0 ID: " + ID + " ST: " + sessionID + " ";
            send_buffer = Encoding.ASCII.GetBytes(answer);
            listener.Send(send_buffer, send_buffer.Length, remoteEP); // Wysłanie komunikatu
            Console.WriteLine("Response: " + answer);
        }
        while (!done)
        {

            receiveByteArray = listener.Receive(ref remoteEP); // Nasłuchiwanie komunikatu.
            receivedData = Encoding.ASCII.GetString(receiveByteArray, 0, receiveByteArray.Length);
            Console.WriteLine("Received: " + receivedData);
            SyncN = Convert.ToInt32(Extensions.FindingInfo("NS", receivedData));
            if (receivedData.Contains("ST: Shutdowning server")) // Zamykanie serwera.
            {

                answer = "TI: " + DateTime.Now.ToString() + " NS: 0 ID: " + ID + " ST: Server is shutdown. Goodbye!;\n";
                send_buffer = Encoding.ASCII.GetBytes(answer);
                listener.Send(send_buffer, send_buffer.Length, remoteEP);
                Console.WriteLine("Bye!");
                listener.Close(); // Zamknięcię serwera.
                break;
            }
            syncMessages[0] = receivedData; // Przypisanie do tablicy zawierającej kolejne komunikaty z jednego ciągu wiadomości.
            if (SyncN != 0) // Jeżeli numer sekwencyjny jest większy od zera to do wj. tablicy będą dodawane kolejne komunikaty z niższymi numer sekw. aż zostanie osiągniete zero.
            {

                int i = 1;
                do
                {
                    receiveByteArray = listener.Receive(ref remoteEP); // Nasłuchiwanie komunikatu.
                    receivedData = Encoding.ASCII.GetString(receiveByteArray, 0, receiveByteArray.Length);
                    Console.WriteLine("Received: " + receivedData);
                    SyncN = Convert.ToInt32(Extensions.FindingInfo("NS", receivedData));
                    syncMessages[i] = receivedData;
                    i++;
                } while (SyncN > 0);
            }
            ID = Extensions.FindingInfo("ID", receivedData); // Ustalanie ID operacji.
            operation = Extensions.FindingInfo("OP", syncMessages[0]); // Ustalanie jaka operacja ma być wykonana.
            double result = 0; // Definicja zmiennej przechowującej wynik.
            try
            {   // Switch case wykonujący polecenia w zależności od typu operacji.
                // Każda opcja zawiera dodanie ID do tablicy, konwersję liczb ze stringa na typ double
                // Wyszukiwanie błędów takich jak
                // - pierwsza liczba jest za duża, druga liczba jest za duża, obie liczby są za duże
                // - rezult przekracza maks. lub min.
                // Dodatkowo w przypadku dzielenia:
                // - dzielenie przez zero.
                // W przypadku silni:
                // - sprawdzenie czy argument nie jest liczbą nienaturalną.
                switch (operation)
                {
                    case "Multiplication":
                        {

                            IDs.Add(ID);
                            number1 = Convert.ToDouble(Extensions.FindingInfo("N1", syncMessages[1]));
                            number2 = Convert.ToDouble(Extensions.FindingInfo("N2", syncMessages[2]));
                            if (Double.IsPositiveInfinity(number2) && Double.IsPositiveInfinity(number1)) throw new System.Exception("Both numbers are too big ");
                            else if (Double.IsPositiveInfinity(number1)) throw new System.Exception("First number is too big ");
                            else if (Double.IsPositiveInfinity(number2)) throw new System.Exception("Second number is too big ");
                            result = number1 * number2;
                            if (Double.IsPositiveInfinity(result)) throw new System.Exception("Result is out of range (positive infinity) ");
                            if (Double.IsNegativeInfinity(result)) throw new System.Exception("Result is out of range (negative infinity) ");
                            answer = "TI: " + DateTime.Now.ToString() + " NS: 0 ID: " + ID + " RE: " + result.ToString() + "\n";
                            send_buffer = Encoding.ASCII.GetBytes(answer);
                            listener.Send(send_buffer, send_buffer.Length, remoteEP); // Wysłanie komunikatu
                            Console.WriteLine("Response: " + answer);

                            break;
                        }
                    case "Division":
                        {
                            IDs.Add(ID);
                            number1 = Convert.ToDouble(Extensions.FindingInfo("N1", syncMessages[1]));
                            number2 = Convert.ToDouble(Extensions.FindingInfo("N2", syncMessages[2]));
                            if (number2 == 0) throw new System.Exception("Division by zero");
                            if (Double.IsPositiveInfinity(number2) && Double.IsPositiveInfinity(number1)) throw new System.Exception("Both numbers are too big ");
                            else if (Double.IsPositiveInfinity(number1)) throw new System.Exception("First number is too big ");
                            else if (Double.IsPositiveInfinity(number2)) throw new System.Exception("Second number is too big ");
                            result = number1 / number2;
                            if (Double.IsPositiveInfinity(result)) throw new System.Exception("Result is out of range (positive infinity) ");
                            if (Double.IsNegativeInfinity(result)) throw new System.Exception("Result is out of range (negative infinity) ");
                            answer = "TI: " + DateTime.Now.ToString() + " NS: 0 ID: " + ID + " RE: " + result.ToString() + "\n";
                            send_buffer = Encoding.ASCII.GetBytes(answer);
                            listener.Send(send_buffer, send_buffer.Length, remoteEP);  // Wysłanie komunikatu
                            Console.WriteLine("Response: " + answer);
                            break;
                        }
                    case "Addition":
                        {
                            IDs.Add(ID);
                            number1 = Convert.ToDouble(Extensions.FindingInfo("N1", syncMessages[1]));
                            number2 = Convert.ToDouble(Extensions.FindingInfo("N2", syncMessages[2]));
                            if (Double.IsPositiveInfinity(number2) && Double.IsPositiveInfinity(number1)) throw new System.Exception("Both numbers are too big ");
                            else if (Double.IsPositiveInfinity(number1)) throw new System.Exception("First number is too big ");
                            else if (Double.IsPositiveInfinity(number2)) throw new System.Exception("Second number is too big ");
                            result = number1 + number2;
                            if (Double.IsPositiveInfinity(result)) throw new System.Exception("Result is out of range (positive infinity) ");
                            if (Double.IsNegativeInfinity(result)) throw new System.Exception("Result is out of range (negative infinity) ");
                            answer = "TI: " + DateTime.Now.ToString() + " NS: 0 ID: " + ID + " RE: " + result.ToString() + "\n";
                            send_buffer = Encoding.ASCII.GetBytes(answer);
                            listener.Send(send_buffer, send_buffer.Length, remoteEP); // Wysłanie komunikatu
                            Console.WriteLine("Response: " + answer);
                            break;
                        }
                    case "Subtraction":
                        {
                            IDs.Add(ID);
                            number1 = Convert.ToDouble(Extensions.FindingInfo("N1", syncMessages[1]));
                            number2 = Convert.ToDouble(Extensions.FindingInfo("N2", syncMessages[2]));
                            if (Double.IsPositiveInfinity(number2) && Double.IsPositiveInfinity(number1)) throw new System.Exception("Both numbers are too big ");
                            else if (Double.IsPositiveInfinity(number1)) throw new System.Exception("First number is too big ");
                            else if (Double.IsPositiveInfinity(number2)) throw new System.Exception("Second number is too big ");
                            result = number1 - number2;
                            if (Double.IsPositiveInfinity(result)) throw new System.Exception("Result is out of range (positive infinity) ");
                            if (Double.IsNegativeInfinity(result)) throw new System.Exception("Result is out of range (negative infinity) ");

                            answer = "TI: " + DateTime.Now.ToString() + " NS: 0 ID: " + ID + " RE: " + result.ToString() + "\n";
                            send_buffer = Encoding.ASCII.GetBytes(answer);
                            listener.Send(send_buffer, send_buffer.Length, remoteEP); // Wysłanie komunikatu
                            Console.WriteLine("Response: " + answer);
                            break;
                        }
                    case "Factorial":
                        {
                            IDs.Add(ID);
                            number1 = Convert.ToDouble(Extensions.FindingInfo("N1", syncMessages[1]));
                            if (number1 < 0) throw new System.Exception("Number is negative");
                            if (Double.IsPositiveInfinity(number1)) throw new System.Exception("Number is too big ");
                            string factTest = number1.ToString();
                            if (!Extensions.IsDigitsOnly(factTest)) throw new System.Exception("Number isn't natural");
                            result = Factorial(number1);
                            if (Double.IsPositiveInfinity(result)) throw new System.Exception("Result is too out of range (positive infinity) ");
                            answer = "TI: " + DateTime.Now.ToString() + " NS: 0 ID: " + ID + " RE: " + result.ToString() + "\n";
                            send_buffer = Encoding.ASCII.GetBytes(answer);
                            listener.Send(send_buffer, send_buffer.Length, remoteEP); // Wysłanie komunikatu
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
                            break;
                        }
                }
                if (operation != "History") // Zapytania o historię do historii nie są dodawane.
                {
                    OP.Add(operation);
                    N1.Add(number1.ToString());
                    N2.Add(number2.ToString());
                    R.Add(result.ToString());
                }


            }
            catch (FormatException) // Łapanie błędu wpisania złego znaku.
            {
                OP.Add(operation);
                N1.Add(number1.ToString());
                N2.Add(number2.ToString());
                R.Add("Wrong character instead of number");
                answer = "TI: " + DateTime.Now.ToString() + " NS: 0 ID: " + ID + " ST: Wrong character instead of number ";
                send_buffer = Encoding.ASCII.GetBytes(answer);
                listener.Send(send_buffer, send_buffer.Length, remoteEP); // Wysłanie komunikatu
                Console.WriteLine("Response: " + answer);
            }
            catch (Exception ex) // Obsługa pozostałych błędów.
            {
                OP.Add(operation);
                N1.Add(number1.ToString());
                N2.Add(number2.ToString());
                R.Add(ex.Message);
                answer = "TI: " + DateTime.Now.ToString() + " NS: 0 ID: " + ID + " ST: " + ex.Message + " ";
                send_buffer = Encoding.ASCII.GetBytes(answer);
                listener.Send(send_buffer, send_buffer.Length, remoteEP); // Wysłanie komunikatu
                Console.WriteLine("Response: " + answer);

            };



        }

        listener.Close();
        return 0;
    }
} // End of class UDPListener