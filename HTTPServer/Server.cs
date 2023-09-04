using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {
        Socket serverSocket;
        IPEndPoint endPoint;
        int backLog = 1000;

        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            //TODO: initialize this.serverSocket

            this.LoadRedirectionRules(redirectionMatrixPath);
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            endPoint = new IPEndPoint(IPAddress.Any, portNumber);
            serverSocket.Bind(endPoint);
        }

        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.
            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"

            serverSocket.Listen(backLog);

            while (true)
            {
                //TODO: accept connections and start thread for each accepted connection.
                Socket clientSocket = serverSocket.Accept();
                Console.WriteLine("New client accepted : {0}", clientSocket.RemoteEndPoint);
                Thread newThread = new Thread(new ParameterizedThreadStart(HandleConnection));
                newThread.Start(clientSocket);
            }
        }

        public void HandleConnection(object obj)
        {
            // TODO: Create client socket 
            Socket clientSocket = (Socket)obj;

            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            clientSocket.ReceiveTimeout = 0;

            // TODO: receive requests in while true until remote client closes the socket.
            Console.WriteLine("Client: " + clientSocket.RemoteEndPoint + " started the connection");
            while (true)
            {
                try
                {
                    // TODO: Receive request
                    byte[] data = new byte[1024 * 1024];//ngrb nshlha
                    int receivedLength = clientSocket.Receive(data);
                    // TODO: break the while loop if receivedLen==0
                    if (receivedLength == 0)
                    {
                        Console.WriteLine("Client: " + clientSocket.RemoteEndPoint + " ended the connection");
                        break;
                    }
                    // TODO: Create a Request object using received request string
                    Request request = new Request(Encoding.ASCII.GetString(data, 0, receivedLength));

                    // TODO: Call HandleRequest Method that returns the response
                    Response response = this.HandleRequest(request);
                    // TODO: Send Response back to client
                    clientSocket.Send(Encoding.ASCII.GetBytes(response.ResponseString));

                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                    //Environment.Exit(1);
                }
            }

            // TODO: close client socket
            clientSocket.Close();
        }


        Response HandleRequest(Request request)
        {
            string content; 
            try
            { 
                //TODO: check for bad request 
                if(!request.ParseRequest())
                {
                    content = LoadDefaultPage(Configuration.BadRequestDefaultPageName);
                    return new Response(StatusCode.BadRequest, "html", content, "");
                }
                string physicalPath = Configuration.RootPath+ "\\" + request.relativeURI;

                //TODO: map the relativeURI in request to get the physical path of the resource.
                string redirectionPath = GetRedirectionPagePathIFExist(request.relativeURI);

                //TODO: check for redirect
                if (!String.IsNullOrEmpty(redirectionPath))
                {
                    content = LoadDefaultPage(Configuration.RedirectionDefaultPageName);
                    physicalPath = Configuration.RootPath + "\\" + redirectionPath;
                    content = File.ReadAllText(physicalPath);
                    return new Response(StatusCode.Redirect, "html", content, redirectionPath);
                }
                //TODO: check file exists
                if (!File.Exists(physicalPath))
                {
                    content = LoadDefaultPage(Configuration.NotFoundDefaultPageName);
                    return new Response(StatusCode.NotFound, "html", content, "");
                }


                //TODO: read the physical file
                content = File.ReadAllText(physicalPath);


                // Create OK response
                return new Response(StatusCode.OK, "html", content, "");

            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                // TODO: in case of exception, return Internal Server Error. 
                content = LoadDefaultPage(Configuration.InternalErrorDefaultPageName);
                return new Response(StatusCode.InternalServerError, "html", content, "");

            }
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty

            string RedirectionPath = string.Empty;
            if (relativePath[0] == '/')
            {
                relativePath = relativePath.Substring(1);
            }
            if (Configuration.RedirectionRules.ContainsKey(relativePath))
            {
                RedirectionPath = Configuration.RedirectionRules[relativePath];
                return RedirectionPath;
            }
            else
                return string.Empty;
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Configuration.RootPath + '\\' + defaultPageName; 
            try
            {
                // TODO: check if filepath not exist log exception using Logger class and return empty string
                if (!File.Exists(filePath))
                {
                    Logger.LogException(new FileNotFoundException("cannot find the file", filePath));  
                }
      
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return string.Empty;
            }
            return File.ReadAllText(filePath);
        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file 
                string[] RulesArr = File.ReadAllLines(filePath);

                Configuration.RedirectionRules = new Dictionary<string, string>();
                // then fill Configuration.RedirectionRules dictionary 
                for (int i = 0 ; i < RulesArr.Length ; i++)
                {
                    string[] rule = RulesArr[i].Split(',');
                    Configuration.RedirectionRules.Add(rule[0], rule[1]);

                }
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                Environment.Exit(1);
            }
        }
    }
}
