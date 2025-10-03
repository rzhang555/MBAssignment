
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MBAssignment.Configurations;
using MBAssignment.Services;
using Microsoft.Extensions.Logging;


namespace MultibeamAssign
{
    class Program
    {
        static bool _appRun = true;

        static async Task<int> Main(string[] args)
        {
            Task? task = null;
            try
            {
                Console.WriteLine("Welcome To Multibeam File Managing System...");
                Console.WriteLine();
                string settingsFilePath = @"..\..\..\Configurations\appSettings.json";
               
                AppSettings settings = LoadAppSettings(settingsFilePath);
                LoggingService logger = new LoggingService(settings);
           
                while (_appRun )
                 {
                    Console.Write(">");
                    string? cmd_input = Console.ReadLine();
                    if (cmd_input == null || cmd_input == string.Empty)
                        continue;

                    string[] cmd = cmd_input.ToLower().Split(" ", StringSplitOptions.RemoveEmptyEntries );


                    switch (cmd[0])
                    {
                        case "run":
                            task = RunFileHandlingAsync(settings, logger);
                            Console.WriteLine(" File processing job started.");
                            Console.WriteLine();
                            break;
                        case "status":
                            ReportStatus(logger);
                            break;
                        case "set":
                            SetAppSettings(cmd, settings, logger);
                            break;
                        case "config":
                            displayCurrentConfiguration(settings);
                            break;
                        case "exit":
                            _appRun = false;
                            break;
                        default:
                            Console.WriteLine("invalid command");
                            break;
                    }

                    await Task.Delay(100); // Simulate a long-running operation
                }

                //press ctl + C event hanler
                Console.CancelKeyPress += async (sender, eventArgs) =>
                {
                    SaveAppSettingsToFile(settingsFilePath, settings);
                    _appRun = false;
                    if (task != null)
                        await task;
                    Environment.Exit(0);
                };

                SaveAppSettingsToFile(settingsFilePath, settings);
                return 0;
            }
            catch (Exception ex)
            {
                LoggingService.WriteErr(ex.Message);
                return 1;
            }
            finally
            {
                //wait for all tasks to complete before exit
                if (task != null)
                    await task;
            }
        }

        static async Task RunFileHandlingAsync(AppSettings settings, LoggingService logger) 
        {
            try
            {
                FileService.CheckDirectories(settings);
                while (_appRun)
                {
                    string[] filePaths = Directory.GetFiles(settings.Input_dir);

                    if (filePaths.Length != 0)
                    {
                        List<Task> processingTasks = new List<Task>();

                        foreach (string filePath in filePaths)
                        {
                            // Create a task for each file operation
                            processingTasks.Add(ProcessFileAsync(filePath, settings, logger));
                        }
                        logger.File_in_processing = processingTasks.Count;

                        // Wait for all file processing tasks to complete
                        await Task.WhenAll(processingTasks);
                        logger.File_in_processing = 0;

                        logger.Total_file_processed += processingTasks.Count;

                    }

                    await Task.Delay(1000);
                }
            }
            catch (Exception ex)
            {
                LoggingService.WriteErr(ex.Message); 
            }
        }

        static async Task ProcessFileAsync(string filePath, AppSettings settings, LoggingService logger)
        {
            await Task.Run(() =>
            {
                try
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    string fileName = Path.GetFileName(filePath);

                    //validate file size and name extension
                    string errMsg = FileService.ValidFile(filePath, settings.MaxFileSizeMB, settings.AllowedExtensions);
                    if (errMsg != string.Empty)
                    {
                        logger.IncremenFailedFileCount();
                        string path = Path.Combine(settings.Failed_dir, fileName);
                        FileService.CopyFile(filePath, path);
                        FileService.DeleteFile(filePath);
                        
                        logger.AddRecord($"{fileName}: invalid file, reason: {errMsg}");
                        return;
                    }

                    //On pass validation: place its checksum file to Output folder
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                    string targetFilePath = Path.Combine(settings.Output_dir, fileNameWithoutExtension + ".checksum");

                    //On pass validation: place compressed .gz file to Output folder
                    FileService.WriteSHA256ChecksumToFile(filePath, targetFilePath);                  
                    targetFilePath = Path.Combine(settings.Output_dir, fileNameWithoutExtension + ".gz");
                    FileService.CompressFile(filePath, targetFilePath);

                    //move original to Archive folder
                    targetFilePath = Path.Combine(settings.Archive_dir, fileName);
                    FileService.CopyFile(filePath, targetFilePath);
                    logger.IncrementValidCount();
                    logger.AddRecord($"{fileName}: valid file");
                  
                    //remove file from input folder
                    FileService.DeleteFile(filePath);
                }
                catch (Exception ex)
                {
                    LoggingService.WriteErr(ex.Message);
                    return;
                }
            }
            );
        }

        static void ReportStatus(LoggingService logger)
        {
            try
            {
                Console.WriteLine();
                Console.WriteLine($" Total processed file: {logger.Total_file_processed}");
                Console.WriteLine($" Total valid file: {logger.GetValidFileCount()}");
                Console.WriteLine($" Total failed file: {logger.GetFailedFileCount()}");
                Console.WriteLine($" Total file in processing: {logger.File_in_processing}");
                Console.WriteLine();
                Console.WriteLine($" Recent processed files:");
                Console.WriteLine($" -----------------------");
                Console.WriteLine();

                foreach (string item in logger.historyQueue)
                {
                    Console.WriteLine(" " + item);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static AppSettings LoadAppSettings(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Application configuration file not found!");
                return new AppSettings();   //use default settings
            }
            else {
                //get settings from file, if file doen not exist then create oen with default values
                string jsonString = File.ReadAllText(filePath);
                AppSettings? settings = JsonSerializer.Deserialize<AppSettings>(jsonString);
                return settings ?? new AppSettings();
            }
        }

        static void SaveAppSettingsToFile(string filePath, AppSettings settings)
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, jsonString);
            }
            catch (Exception ex)
            {
                LoggingService.WriteErr(ex.Message);
            }
        }

        static void SetAppSettings(string[] cmd, AppSettings settings, LoggingService logger)
        { 
            if (cmd.Length == 1)
            {
                Console.WriteLine(" Invalid command, run 'set help' for reference.");
                return;
            }
            if (cmd[1]!.ToLower() == "help")
            {
                ShowSetCmdOptions();
                return;
            }
            if (cmd.Length < 3)
            {
                Console.WriteLine(" Invalid command, run 'set help' for reference.");
                return;
            }
            try
            {
                switch (cmd[1].ToLower())
                {
                    case "input":
                        settings.Input_dir = cmd[2];
                        break;
                    case "output":
                        settings.Output_dir = cmd[2];
                        break;
                    case "failed":
                        settings.Output_dir = cmd[2];
                        break;
                    case "archive":
                        settings.Output_dir = cmd[2];
                        break;
                    case "addext":
                        settings.AllowedExtensions.Add(cmd[2]);
                        break;
                    case "delext":
                        settings.AllowedExtensions.Remove(cmd[2]);
                        break;
                    case "job_num":
                        int newSize = int.Parse(cmd[2]);
                        if (newSize < settings.Num_of_last_jobs)
                            logger.ResizeHistoryQueue(newSize);
                        settings.Num_of_last_jobs = newSize;
                        break;
                    case "mb":
                        settings.MaxFileSizeMB = float.Parse(cmd[2]);
                        break;
                    case "logfile":
                        settings.logfile_name = cmd[2];
                        break;
                    default:
                        Console.WriteLine("invalid settings command");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Configuration command execution error: " + ex.ToString());
            }

        }

        static void ShowSetCmdOptions()
        {
            Console.WriteLine();
            Console.WriteLine("- input <folder name> :  set input file folder");
            Console.WriteLine("- output <folder name> :  set output file folder");
            Console.WriteLine("- archive <folder name> :  set archive file folder");
            Console.WriteLine("- failed <folder name> :  set failed file folder");
            Console.WriteLine("- addExt <ext> :  add a file extention to allowed file extensions");
            Console.WriteLine("- delExt <ext> :  remove a file extention from allowed file extensions");
            Console.WriteLine("- job_num <number> :  the number of last processed jobs for status report");
            Console.WriteLine("- mb <floatt> :  max file size in MB");
            Console.WriteLine("- logfile <file name> : the name file for recording file processing history");
            Console.WriteLine();
        }

        static void displayCurrentConfiguration(AppSettings settings)
        {
            Console.WriteLine();
            Console.WriteLine($" Input dir:  {settings.Input_dir}");
            Console.WriteLine($" Output dir:  {settings.Output_dir}");
            Console.WriteLine($" Archive dir:  {settings.Archive_dir}");
            Console.WriteLine($" Faile dir:  {settings.Failed_dir}");
            Console.WriteLine($" Max file size allowd:  {settings.MaxFileSizeMB} MB");
            Console.WriteLine($" Allowed file name extensions: [{string.Join(", ", settings.AllowedExtensions)}]");
            Console.WriteLine($" Number of last jobs for status report: {settings.Num_of_last_jobs}");
            Console.WriteLine();
        }
    }
}


