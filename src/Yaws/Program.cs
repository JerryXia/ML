using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Yaws
{
    class Program
    {
        // web server settings
        static string _appPath;
        static string _portString;
        static string _virtRoot;

        // the web server
        static Server _server;

        static void Main(string[] args)
        {
            _portString = "8080";
            _virtRoot = "/";
            _appPath = string.Empty;

            try
            {
                if (args.Length >= 1) _appPath = args[0].Trim('"');
                if (args.Length >= 2) _portString = args[1];
                if (args.Length >= 3) _virtRoot = args[2];
            }
            catch
            {
            }

            if (string.IsNullOrEmpty(_appPath))
            {
                return;
            }

            Console.WriteLine(_appPath);
            Console.WriteLine(_portString);
            Console.WriteLine(_virtRoot);

            Start();
        }

        static void Start()
        {
            if (_appPath.Length == 0 || !Directory.Exists(_appPath))
            {
                ShowError("Invalid Application Directory");
                return;
            }

            int portNumber = -1;
            try
            {
                portNumber = Int32.Parse(_portString);
            }
            catch
            {
            }
            if (portNumber <= 0)
            {
                ShowError("Invalid Port");
                return;
            }

            if (_virtRoot.Length == 0 || !_virtRoot.StartsWith("/"))
            {
                ShowError("Invalid Virtual Root");
                return;
            }

            try
            {
                _server = new Server(portNumber, _virtRoot, _appPath);
                _server.Start();
            }
            catch
            {
                ShowError(
                    "Yet Another Web Server failed to start listening on port " + portNumber + ".\r\n" +
                    "Possible conflict with another Web Server on the same port.");

                return;
            }

            Console.ReadKey();
            Stop();
        }

        static void Stop()
        {
            try
            {
                if (_server != null)
                    _server.Stop();
            }
            catch
            {
            }
            finally
            {
                _server = null;
            }
        }

        static void ShowError(String err)
        {
            Console.WriteLine("ErrMsg: " + err);
        }
    }
}
