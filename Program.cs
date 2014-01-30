using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace UploadSm
{
    class Program
    {
        static void Main(string[] args)
        {
            String user;
            String password;
            String incident;
            String url;
            String pathFilename;
            String filename;
            // The Length property is used to obtain the length of the array. 
            // Notice that Length is a read-only property:
            Console.WriteLine("Number of command line parameters = {0}",args.Length);
            if (args.Length == 6)
            {
                user = args[0];
                password = args[1];
                incident = args[2];
                url = args[3];
                pathFilename = args[4];
                filename = args[5];
            }
            else
            {
                Console.WriteLine("Argument error!");
                Console.WriteLine("Usage : UploadSm user password incident_jump url_jump_webservice fullpath_file filename");
                return;
            }
            for (int i = 0; i < args.Length; i++)
            {
                Console.WriteLine("Arg[{0}] = [{1}]", i, args[i]);
            }

            SGIncident_USVD srv;
            srv = new SGIncident_USVD(url) { Credentials = new NetworkCredential(user, password) };
            // get the exact file name from the path
            String strFile = System.IO.Path.GetFileName(@pathFilename);
            FileStream fStream = new FileStream(@pathFilename,
            FileMode.Open, FileAccess.Read);

            // convert the file to a byte array
            byte[] data = System.IO.File.ReadAllBytes(@pathFilename);

            // pass the byte array (file) and file name to the web service

            //Set attachment
            AttachmentType attachment = new AttachmentType
            {
                action = "add",
                name = filename,
                attachmentType = "file",
                len = data.Length,
                lenSpecified = true,
                contentType = "application/octet-stream",
                contentId = "attachment",
                Value = data,
                href = "&lt;" + filename + ">",
            };
            //Set instance 
            SGIncident_USVDInstanceType instance = new SGIncident_USVDInstanceType { query = "number = \"" + incident + "\"" };
            instance.attachments = new AttachmentType[1];
            instance.attachments[0] = attachment;

            //Set journal update
            SGIncident_USVDInstanceTypeJournalUpdates journalUpdates = new SGIncident_USVDInstanceTypeJournalUpdates();
            journalUpdates.JournalUpdates = new StringType[1];
            StringType journalUpdate = new StringType { Value = filename };
            journalUpdates.JournalUpdates[0] = journalUpdate;

            instance.JournalUpdates = journalUpdates;


            // Set model
            SGIncident_USVDModelType model = new SGIncident_USVDModelType();
            model.instance = instance;

            //Set keys
            SGIncident_USVDKeysType keys = new SGIncident_USVDKeysType();
            
            //Set request
            UpdateSGIncident_USVDRequest request = new UpdateSGIncident_USVDRequest();
            request.attachmentData = true;
            request.attachmentInfo = true;
            //request.ignoreEmptyElements = false;

            //Set keys in model
            model.keys = keys;

            //Set model in request
            request.model = model;
            try
            {
                UpdateSGIncident_USVDResponse response;
                response = srv.UpdateSGIncident_USVD(request);
                Console.WriteLine(response.message);
                Console.WriteLine(response.returnCode);


            }
            catch (Exception e)
            {
                Console.WriteLine("Error : " + e.Message);
            }
            

            fStream.Close();
            fStream.Dispose();
            Console.WriteLine("Upload Done");
        }
    }
}