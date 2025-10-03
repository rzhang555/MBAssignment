using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Runtime.InteropServices.ComTypes;

namespace MBAssignment.Configurations
{
    public class AppSettings
    {
        public string Input_dir { set; get; } = @"c:\tmp\mb_assign\input";
        public string Output_dir { set; get; } = @"c:\tmp\mb_assign\output";
        public string Failed_dir { set; get; } = @"c:\tmp\mb_assign\failed";
        public string Archive_dir { set; get; } = @"c:\tmp\mb_assign\archive";
        public string Logfile_dir { set; get; } = @"c:\tmp\mb_assign\logs";
        public string logfile_name { set; get; } = "file_process_history.txt";
        public string logErrfile_name { set; get; } = "err.txt";
        public float MaxFileSizeMB { set; get; } = (float) 1.0;
        public List<string> AllowedExtensions { set; get; } = ["txt", "doc"];
        public int Num_of_last_jobs { set; get; } = 10;
        public AppSettings() { }
    }
}
