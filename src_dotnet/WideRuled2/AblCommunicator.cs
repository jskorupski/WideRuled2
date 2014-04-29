using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.IO;
using System.ComponentModel;
using System.Net.Sockets;
using System.Net;



namespace WideRuled2
{
    public class AblCommunicator
    {
        public const String linesplit = "\t";
        public const String valuesplit = "=";
        public const String PRINT = "PRINT";
        public const String PRINTLINE = "PRINTLINE";
	    public const String END = "END";
    	
	    public const String ABORT = "ABORT";
	    public const String INTERACT = "INTERACT";
        public const String UNDO = "UNDO";
        public const String id = "Id";

        public static Int32 Port = 6000;

        public static void ExecuteStory()
        {
            Port = Port + 15;
            if(Port > 40000)
            {
                Port = 6000;
            }
            int retryConnection = 0;
            TcpClient client = null;
            TcpListener server = null;


            while (retryConnection < 10)
            {
                
                try
                {
                    // Set the TcpListener on port 5025

                    IPAddress localAddr = IPAddress.Parse("127.0.0.1");


                    string data = "";
                    server = new TcpListener(localAddr, Port+retryConnection);

                   // AblCommObject.Instance.StoryOutput.AppendLine("Server start: " + (Port + retryConnection).ToString());
                    server.Start();

                    Thread newAblLaunch = new Thread(AblCommunicator.launchAbl);
                    newAblLaunch.Start();

                   // AblCommObject.Instance.StoryOutput.AppendLine("server accept");

                    // Perform a blocking call to accept requests.
                    // You could also user server.AcceptSocket() here.
                    client = server.AcceptTcpClient();
                    //client.ReceiveTimeout = 500;

                    //AblCommObject.Instance.StoryOutput.AppendLine("after accept");
                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();
                    //stream.ReadTimeout = 4000;

                    StreamReader reader = new StreamReader(stream);
                    StreamWriter write = new StreamWriter(stream);
                    write.AutoFlush = true;


                    List<Object> netItems = new List<Object>();
                    netItems.Add(server);
                    netItems.Add(client);
                    netItems.Add(write);


                    Thread newThreadSend = new Thread(AblCommunicator.Send);
                    newThreadSend.Start(netItems);


                    while (true)
                    {

                        try
                        {
                            data = reader.ReadLine();
                        }
                        catch (Exception ex)
                        {
                            AblCommObject.Instance.StoryFinished = true;
                            client.Close();
                            server.Stop();
                            return;
                        }

                        string[] messages = data.Split(AblCommunicator.linesplit.ToCharArray());
                        if (messages.Length > 0)
                        {

                            if (messages[0].Equals(AblCommunicator.ABORT))
                            {

                                AblCommObject.Instance.StoryFinished = true;
                                client.Close();
                                server.Stop();
                                return;
                            }
                            else if (messages[0].Equals(AblCommunicator.PRINT))
                            {
                                string[] vals = messages[1].Split(AblCommunicator.valuesplit.ToCharArray());
                                string printString = vals[1];

                                AblCommObject.Instance.LatestText = printString + " ";
                            }
                            else if (messages[0].Equals(AblCommunicator.PRINTLINE))
                            {
                                string[] vals = messages[1].Split(AblCommunicator.valuesplit.ToCharArray());
                                string printString = vals[1];

                                AblCommObject.Instance.LatestText = "\n" + printString + "\n";
                            }
                           

                        }
                       


                    }


                }

                catch (SocketException e)
                {
                    AblCommObject.Instance.StoryOutput.Append("Generation error:" + e.Message);
                    retryConnection++;
                }
            }

            if (client != null)
            {
                client.Close();
            }
            if (server != null)
            {
                server.Stop();
            }

            AblCommObject.Instance.StoryFinished = true;

        }


        public static void Send(object items)
        {
            List<Object> netItems = (List<Object>)items;

            TcpListener server = (TcpListener)netItems[0];
            TcpClient client = (TcpClient)netItems[1];
            StreamWriter writer = (StreamWriter)netItems[2];
            while(true)
            {
                
                if (AblCommObject.Instance.StoryFinished)
                {
               
                    return;
                }
                else if (AblCommObject.Instance.AbortStory)
                {
                    //MESSAGETYPE<linesplit>PARAMNAME<valuesplit>PARAMVALUE<linesplit>PARAMNAME<valuesplit>PARAMVALUE.....
                    string message = AblCommunicator.ABORT + AblCommunicator.linesplit;
                    writer.WriteLine(message);
                    
                    return;
                }
                else if(AblCommObject.Instance.DoInteraction)
                {
                  
                    //MESSAGETYPE<linesplit>PARAMNAME<valuesplit>PARAMVALUE<linesplit>PARAMNAME<valuesplit>PARAMVALUE.....
                    string message = AblCommunicator.INTERACT + AblCommunicator.linesplit + AblCommunicator.id+AblCommunicator.valuesplit+AblCommObject.Instance.InteractionId.ToString();
                    writer.WriteLine(message);
                }
                else if (AblCommObject.Instance.Undo)
                {
                    //MESSAGETYPE<linesplit>PARAMNAME<valuesplit>PARAMVALUE<linesplit>PARAMNAME<valuesplit>PARAMVALUE.....
                    string message = AblCommunicator.UNDO + AblCommunicator.linesplit;
                    writer.WriteLine(message);

                }


                Thread.Sleep(40);
            }

  
            
        }

        private static void launchAbl()
        {
            
            //string output = "";
            string currWorkingDir = App.CurrentWorkingDirectory;
            System.Diagnostics.Process proc = new System.Diagnostics.Process();

            Thread.Sleep(100);
            //initiate server thread


            //run abl - must support proxybot


            proc.StartInfo.WorkingDirectory = currWorkingDir + "\\abl";
            proc.StartInfo.FileName = currWorkingDir + "\\abl\\jdk6\\jre\\bin\\java.exe";
            proc.StartInfo.Arguments = "-classpath \"abl.jar;hoj.jar;.\" javacode.WR_RunBot " + "localhost " + Port;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = false;
            proc.StartInfo.RedirectStandardError = false;
            proc.StartInfo.CreateNoWindow = true;
            proc.Start();
            


          
            /*try
            {
                AblCommObject.Instance.StoryOutput.Append(proc.StandardOutput.ReadToEnd());
                AblCommObject.Instance.StoryOutput.Append(proc.StandardError.ReadToEnd());
                output += proc.StandardError.ReadToEnd();
            }
            catch (System.Exception ex)
            {
                throw new Exception("Launch error: " + ex.Message);
            }
            */

            //proc.WaitForExit();

        }

     


        private static void startServer(WindowStoryOutputText outputWindow)
        {

            
        }


    }
}
