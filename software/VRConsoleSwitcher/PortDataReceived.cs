using System;
using System.IO.Ports;
using System.Diagnostics;
using System.IO;


class PortDataReceived
{
    private static int currentProcess = 0;
   // private static Process runningProcess;
    private static int currentProcessId;
    private static string[] files = new string[2];
    private static string[] names = new string[files.Length];

    private static bool ignoreInput = false;
    [STAThread]
    public static void Main()
    {

        using (StreamReader sr = new StreamReader(@".\filenames.txt"))
        {
            int i = 0;
            while (i < files.Length && sr.Peek() >= 0)
            {
               files[i] = sr.ReadLine();
                names[i] = Path.GetFileNameWithoutExtension(files[i]);
                Console.WriteLine(names[i]);
                i++;
            }
        }

        //runningProcess = new Process();
        SerialPort mySerialPort = new SerialPort();

        mySerialPort.PortName = SetPortName(mySerialPort.PortName);
        mySerialPort.BaudRate = 9600;
        mySerialPort.Parity = Parity.None;
        mySerialPort.StopBits = StopBits.One;
        mySerialPort.DataBits = 8;
        mySerialPort.Handshake = Handshake.None;
        mySerialPort.RtsEnable = true;

        mySerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

        mySerialPort.Open();

        Console.WriteLine("Press ESC to quit...");
        Console.WriteLine();
        while (Console.ReadKey().Key != ConsoleKey.Escape)
        { }
            mySerialPort.WriteLine("00");
            mySerialPort.Close();
        
       
    }

    private static void RunProcess(int processToRun, SerialPort sp)
    {
        if (processToRun == currentProcess)
            return;
        

        if (currentProcess != 0)
        {
            using (Process runningProcess = Process.GetProcessesByName(names[currentProcess-1])[0])
            {
                runningProcess.CloseMainWindow();
                runningProcess.WaitForExit(2000);
            }
        }

        /*if (runningProcess != null)
        {
            if (runningProcess.ProcessName == names[processToRun - 1])
            return;

            runningProcess.CloseMainWindow();
            runningProcess.WaitForExit(2000);
            runningProcess.Dispose();
           
        }*/

        currentProcess = processToRun;
        runSelectedProcess(sp);


    
    }

    // Handle Exited event and display process information.
    private static void myProcess_Exited(object sender, System.EventArgs e)
    {
        //runSelectedProcess();

        //waitingForExit = false;
    }

    private static void runSelectedProcess(SerialPort sp)
    {

        using (Process runningProcess = Process.Start(files[currentProcess - 1]))
        {
            runningProcess.WaitForInputIdle(2000);
        }

        string buttonLeds = "";
        for (int i = 0; i < files.Length; i++)
        {
            if (currentProcess - 1 == i)
            {
                buttonLeds += "1";
            }
            else
            {
                buttonLeds += "0";
            }

        }
        sp.WriteLine(buttonLeds);

        /*switch (currentProcess)
        {
            case 1:
                runningProcess = Process.Start("D:\\Blocked In Game Final Build\\Blocked In Game.exe");
                break;

            case 2:
                runningProcess = Process.Start("D:\\Taylor & Pauline\\Project Madison\\ProjectMadison.exe");
                break;
        }*/
    }

    private static void DataReceivedHandler(
                        object sender,
                        SerialDataReceivedEventArgs e)
    {
        SerialPort sp = (SerialPort)sender;
        string indata = sp.ReadExisting();
       // Console.WriteLine("Data Received:");
        //Console.Write(indata);

        //if (!ignoreInput)
        {
            try
            {
                RunProcess(Convert.ToInt32(indata), sp);
            }
            catch (Exception ex)
            {
              //  Console.Write(ex);
            }
        }
    }

    // Display Port values and prompt user to enter a port.
    public static string SetPortName(string defaultPortName)
    {
        string portName;

        Console.WriteLine("Available Ports:");
        foreach (string s in SerialPort.GetPortNames())
        {
            Console.WriteLine("   {0}", s);
        }

        Console.Write("Enter COM port value (Default: {0}): ", defaultPortName);
        portName = Console.ReadLine();

        if (portName == "" || !(portName.ToLower()).StartsWith("com"))
        {
            portName = defaultPortName;
        }
        return portName;
    }
}