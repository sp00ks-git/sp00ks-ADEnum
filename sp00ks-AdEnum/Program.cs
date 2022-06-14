using System;
using System.Diagnostics;
using System.Security.Principal;
using System.Threading;
using System.Net.NetworkInformation;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Sockets;

namespace sp00ks_AdEnum

{
    class Program
    {
        static void Main(string[] args)
        {
            bool showMenu = true;
            while (showMenu)
            {
                showMenu = MainMenu();
            }
        }
        private static bool MainMenu()
        {
            Console.Clear();
            Console.Write("Current User -> " + WindowsIdentity.GetCurrent().Name);
            Console.Write("\r\n");
            Console.WriteLine("\r\nChoose an option:");
            Console.WriteLine("1) Reverse String");
            Console.WriteLine("2) Remove Whitespace");
            Console.WriteLine("3) Display AD User Details (Net User)");
            Console.WriteLine("4) Display AD User Details (LDAP) OPSEC Safe");
            Console.WriteLine("5) Display Local Info");
            Console.WriteLine("6) SPN Scanning Info");
            Console.WriteLine("x) Exit");
            Console.Write("\r\nSelect an option: ");

            switch (Console.ReadLine())
            {
                case "1":
                    ReverseString();
                    return true;
                case "2":
                    RemoveWhitespace();
                    return true;
                case "3":
                    ADUserDetails_netuser();
                    return true;
                case "4":
                    ADUserDetails_LDAP();
                    return true;
                case "5":
                    Display_Local_Info();
                    return true;
                case "6":
                    SPN_Scanning_Info();
                    return true;
                case "x":
                    return false;
                default:
                    return true;
            }
        }

        private static string CaptureInput()
        {
            Console.Write("Enter the string you want to modify: ");
            return Console.ReadLine();
        }

        private static void ReverseString()
        {
            Console.Clear();
            Console.WriteLine("Reverse String");
            char[] charArray = CaptureInput().ToCharArray();
            Array.Reverse(charArray);
            DisplayResult(String.Concat(charArray));
        }

        private static void RemoveWhitespace()
        {
            Console.Clear();
            Console.WriteLine("Remove Whitespace");
            DisplayResult(CaptureInput().Replace(" ", ""));
        }

        private static void ADUserDetails_netuser()
        {
            Console.Clear();
            Console.WriteLine("AD User Details Menu!");
            Console.WriteLine("Enter the user to find their details - Method - Net User");
            string answer1 = Console.ReadLine();
            System.Diagnostics.Process.Start("CMD.exe", "/c net user " + answer1 + " /domain");
            Thread.Sleep(1000);
            Console.Write("\r\nPress Enter to return to the Main Menu");
            Console.ReadLine(); //used to pause
        }


        //private static void ADUserDetails_ldap()
        //{
        //    Console.Clear();
        //    Console.WriteLine("AD User Details Menu!");
        //    Console.WriteLine("Enter the user to find their details - Method - Net User");
        //    string answer1 = Console.ReadLine();
        //    InitialSessionState iss = InitialSessionState.CreateDefault();
        //    Runspace rs = RunspaceFactory.CreateRunspace(iss);
        //    rs.Open();
        //    PowerShell ps = PowerShell.Create();
        //    ps.Runspace = rs;
        //    ps.AddCommand("Get-Command");
        //    ps.Invoke();
        //    Thread.Sleep(1000);
        //    Console.Write("\r\nPress Enter to return to the Main Menu");
        //    Console.ReadLine(); //used to pause
        //}



        private static void DisplayResult(string message)
        {
            Console.WriteLine($"\r\nYour modified string is: {message}");
            Console.Write("\r\nPress Enter to return to Main Menu");
            Console.ReadLine();
        }

        private static void ADUserDetails_LDAP()
        {

            Console.Clear();
            Console.WriteLine("AD User Details Menu!");
            Console.WriteLine("Enter the user to find their details - Method - Net User");
            string answer1 = Console.ReadLine();
            //Open up PowerShell with no window
            Process ps = new Process();
            ProcessStartInfo psinfo = new ProcessStartInfo();
            psinfo.FileName = "powershell.exe";
            psinfo.WindowStyle = ProcessWindowStyle.Hidden;
            psinfo.UseShellExecute = false;
            psinfo.RedirectStandardInput = true;
            psinfo.RedirectStandardOutput = true;
            ps.StartInfo = psinfo;
            ps.Start();
            //Done with that.

            //Run the command.
            ps.StandardInput.WriteLine("Get-Process");
            ps.StandardInput.Flush();
            ps.StandardInput.Close();
            ps.WaitForExit();
            //Done running it.

            //Write it to the console.
            Console.WriteLine(ps.StandardOutput.ReadToEnd());
            //Done with everything.

            //Wait for the user to press any key.
            Console.ReadKey(true);
        }

        private static void Display_Local_Info()
        {
            Console.Clear();
            Console.Write("");
            Console.WriteLine("A Bunch of  Local User / Machine Info");
            Console.Write("");
            Console.WriteLine("Current Name -> " + WindowsIdentity.GetCurrent().Name);
            Console.WriteLine("Current User(SID) -> " + WindowsIdentity.GetCurrent().User);
            Console.WriteLine("Current Groups -> " + WindowsIdentity.GetCurrent().Groups);
            Console.WriteLine("Current User Impersonation Level -> " + WindowsIdentity.GetCurrent().ImpersonationLevel);
            Console.WriteLine("Current User Is Authenticated? -> " + WindowsIdentity.GetCurrent().IsAuthenticated);
            Console.WriteLine("Current User Is Guest? -> " + WindowsIdentity.GetCurrent().IsGuest);
            Console.WriteLine("Current User Is System? -> " + WindowsIdentity.GetCurrent().IsSystem);
            Console.WriteLine("Current User SID / Owner -> " + WindowsIdentity.GetCurrent().Owner);
            Console.WriteLine("Current User's Token -> " + WindowsIdentity.GetCurrent().Token);
            
            //Code Block to Show the Local IP Address of the machine

            {
                string IPAddress = "";
                IPHostEntry Host = default(IPHostEntry);
                string Hostname = null;
                Hostname = System.Environment.MachineName;
                Host = Dns.GetHostEntry(Hostname);
                foreach (IPAddress IP in Host.AddressList)
                {
                    if (IP.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        IPAddress = Convert.ToString(IP);
                    }
                }

                Console.WriteLine("The Local IP Address is: {0}", IPAddress);
            }
            Console.Write("");
            {
                string url = "http://checkip.dyndns.org";
                System.Net.WebRequest req = System.Net.WebRequest.Create(url);
                System.Net.WebResponse resp = req.GetResponse();
                System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
                string response = sr.ReadToEnd().Trim();
                string[] ipAddressWithText = response.Split(':');
                string ipAddressWithHTMLEnd = ipAddressWithText[1].Substring(1);
                string[] ipAddress = ipAddressWithHTMLEnd.Split('<');
                string mainIP = ipAddress[0];
                Console.WriteLine("The Public Facing IP is: {0} ", mainIP);
            }
            //Wait for the user to press any key.
            Console.ReadKey(true);
        }


        private static void SPN_Scanning_Info()
        {

            Console.Clear();
            Console.WriteLine("Scan the domain for Potential SPN's!");
            //Open up PowerShell with no window
            Process ps = new Process();
            ProcessStartInfo psinfo = new ProcessStartInfo();
            psinfo.FileName = "powershell.exe";
            psinfo.WindowStyle = ProcessWindowStyle.Hidden;
            psinfo.UseShellExecute = false;
            psinfo.RedirectStandardInput = true;
            psinfo.RedirectStandardOutput = true;
            ps.StartInfo = psinfo;
            ps.Start();
            //Done with that.

            //Run the command.
            ps.StandardInput.WriteLine("([adsisearcher]”(&(objectClass=User)(primarygroupid=513)(servicePrincipalName=*))”).FindAll() | ForEach-Object { “Name:$($_.properties.name)””SPN:$($_.properties.serviceprincipalname)””Path:$($_.Path)”””}");
            ps.StandardInput.Flush();
            ps.StandardInput.Close();
            ps.WaitForExit();
            //Done running it.

            //Write it to the console.
            Console.WriteLine(ps.StandardOutput.ReadToEnd());
            //Done with everything.

            //Wait for the user to press any key.
            Console.ReadKey(true);
        }

    }
}