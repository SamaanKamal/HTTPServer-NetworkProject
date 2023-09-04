using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        string[] requestLines;
        RequestMethod method;
        public string relativeURI;
        Dictionary<string, string> headerLines;

        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        HTTPVersion httpVersion;
        string requestString;
        string[] contentLines;

        public Request(string requestString)
        {
            this.requestString = requestString;
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest()
        {
            bool result = true;
            //TODO: parse the receivedRequest using the \r\n delimeter   

            string[] splitchars = { "\r\n" };

            contentLines = requestString.Split(splitchars, StringSplitOptions.None);
            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            if (contentLines.Length < 3)
            {
                return false;
            }

            // Parse Request line
            requestLines = contentLines[0].Split(' ');
        
            // Validate blank line exists
            // Load header lines into HeaderLines dictionary
         
            return result & LoadHeaderLines() & ValidateBlankLine() & ParseRequestLine();
        }//End of function

        private bool ParseRequestLine()
        {

            if (requestLines.Length < 2)
            {
                return false;
            }

            if (requestLines.Length == 2)
            {
                httpVersion = HTTPVersion.HTTP09;
            }
            else
            {

                if (requestLines[2] == "HTTP/1.0")
                {
                    httpVersion = HTTPVersion.HTTP10;
                }
                else if (requestLines[2] == "HTTP/1.1") { httpVersion = HTTPVersion.HTTP11; }
                else { return false; }

            }

            /*
            switch (requestLines[0].ToUpper())
            {
                case "GET":
                    method = RequestMethod.GET;
                    break;
                case "POST":
                    method = RequestMethod.POST;
                    break;
                case "HEAD":
                    method = RequestMethod.HEAD;
                    break;
                default:
                    return false;
            }*/
            method = RequestMethod.GET;

            relativeURI = requestLines[1];
            return ValidateIsURI(relativeURI);
        }



        private bool ValidateBlankLine()
        {
            if (contentLines[(contentLines.Length - 2)] == "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool LoadHeaderLines()
        {
            bool result = true;
            headerLines = new Dictionary<string, string>();
            for (int i = 1; i < contentLines.Length - 2; i++)
            {
                if (contentLines[i].Contains(":"))
                {
                    string[] splitch = { ": " };
                    string[] request1 = contentLines[i].Split(splitch, StringSplitOptions.None);
                    headerLines.Add(request1[0], request1[1]);

                }
                else result = false;
            }
            return result;
        }

        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }



    }
}