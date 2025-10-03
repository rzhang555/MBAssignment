using System.Linq.Expressions;
using MBAssignment.Configurations;
using Microsoft.Extensions.Logging;

namespace MBAssignment.Services
{
    public class LoggingService
    {
        private static readonly object _lockObject = new object();
        public int Total_file_processed { set; get; } = 0;
        private int _total_valid_file = 0;
        private int _total_failed_file = 0;
        public int File_in_processing { set; get; } = 0;

        public Queue<string> historyQueue = new Queue<string>();

        AppSettings settings;

        static string logFilePath = "";
        static string errFilePath = "";
        public LoggingService(AppSettings settings) 
        {
            this.settings = settings;      
            logFilePath = Path.Combine(this.settings.Logfile_dir, this.settings.logfile_name);
            errFilePath = Path.Combine(this.settings.Logfile_dir, this.settings.logErrfile_name);
        }

        public void AddRecord(string msg)
        {
            lock (_lockObject)
            {
                if (historyQueue.Count >= settings.Num_of_last_jobs)
                    historyQueue.Dequeue();

                historyQueue.Enqueue(msg);

                WriteMsgToFile(msg);
            }
        }

        
        void WriteMsgToFile(string msg)
        {
            try {
                 File.AppendAllText(logFilePath, msg + Environment.NewLine);
             }
            catch(Exception ex) { 
                 Console.WriteLine(ex.ToString());
            }
        }
        
        public static void WriteErr(string errMsg) 
        {
            try {
                Console.WriteLine(errMsg);
                File.AppendAllText(errFilePath, errMsg + Environment.NewLine);
            }
            catch (Exception ex) {
                Console.WriteLine(ex.ToString());
            }
        }

        public void IncrementValidCount()
        {
            lock (_lockObject)
            {
                _total_valid_file++;
            }
        }

        public int GetValidFileCount()
        {
            return _total_valid_file;
        }

        public void IncremenFailedFileCount()
        {
            lock (_lockObject)
            {
                _total_failed_file++;
            }
        }

        public int GetFailedFileCount()
        {
            return _total_failed_file;
        }

        public void ResizeHistoryQueue(int num)
        {
            while (historyQueue.Count > num)
            {
                historyQueue.Dequeue();
            }
        }
    }
}
