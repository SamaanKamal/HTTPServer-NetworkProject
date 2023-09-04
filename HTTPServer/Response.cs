using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{

    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400,
        Redirect = 301
    }

    class Response
    {
        string responseString;
        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }
        StatusCode code;
        List<string> headerLines = new List<string>();
        public Response(StatusCode code, string contentType, string content, string redirectoinPath)
        {
            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
            this.code = code;

            string status_line = GetStatusLine(code);
            string content_type = "Content-Type: " + contentType + "\r\n";
            string content_length = "Content-Length: " + content.Length + "\r\n";
            string date = "Date: " + DateTime.Now + "\r\n";

            headerLines.Add(content_type);
            headerLines.Add(content_length);
            headerLines.Add(date);

            // TODO: Create the request string
            if (redirectoinPath != String.Empty) 
            {
                string location = "Location: " + redirectoinPath + "\r\n";
                headerLines.Add(location);

            }
            responseString = status_line;
            for (int i = 0; i < headerLines.Count; i++)
            {
                responseString += headerLines[i];
            }

            responseString += "\r\n";
            responseString += content;

        }

        private string GetStatusLine(StatusCode code)
        {

            // TODO: Create the response status line and return it

            string statusLine = string.Empty;
            switch ((int)code) {
                case 200:
                    statusLine = "OK";
                    break;
                case 500:
                    statusLine = "InternalServerError";
                    break;
                case 404:
                    statusLine = "NotFound";
                    break;
                case 400:
                    statusLine = "BadRequest";
                    break;
                case 301:
                    statusLine = "Redirect";
                    break;
            }

 
            statusLine = "HTTP / 1.1" + (int)code + "" + statusLine + "\r\n";

            return statusLine;
        }
    }
}
