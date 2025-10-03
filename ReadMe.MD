This project is a .Net 8 console application created with Visual Studio 2022 

To build and run the application:

1. Windows:

   Open the project in Visual Studio which supports .Net 8, build it and run.

2. Linux:

   2.1 publish the project for Linux. Download the project to a windows machine, then open a command console window (use "Run as administrator) and navigate to the solution folder which contains the "MBAssignment" project:
      and run:
           C:.../MBAssignment> dotnet publish -c Release -r linux-x64 --self-contained true        
       This command publishes your application to the bin/Release/netX.0/linux-x64/publish directory.
   2.2 Transfer to Linux: Copy the published output directory to your Linux machine.
   2.3 Run on Linux:
       - Navigate to the published directory on your Linux machine.
       - Make the executable file runnable:
           % chmod +x your_application_name
       - Ensure the appropriate .NET runtime is installed on your Linux system if you chose a framework-dependent deployment.

3. MacOS:

   3.1 Install Visual Studio Code, and have the C# extension by Microsoft installed in VS Code.
   3.2 Verify that the .NET Core CLI tools are installed and accessible from your terminal. 
   3.3 Open the project, build and run it.


4. Instructions for running the Application:

When the application starts, a prompt ">" will show up in a console terminal for command input.

Commands:

>> run     --> start the file processing task

>status  --> display numbers of total processed files processed, valid files, invalid file, and last N jobs, etc 

>set <arg1> <arg2>   --> change configurations in state, such as input, output diretories, maximum file size, allowed file extensions. "set help" will provide the <arg> list.

>config     --> display current figurations

>exit       --> wait for all current running job to complete and terminate the application.

<Ctr + C> key pressing  --> function the same as command "exit". 

5. Output: 
   if a file is valid, two files will be put into the output foler: 1. its compressed .gz file and its SHA256 checksum file.

6. Configuration:

   4.1 A configuration file, called "appSettings.json", is created under "Configuration" directory. When the application starts, it will be loaded before any command input.
   4.2 If the above seting file dose not exist, an instance of "AppSettings" will be created with the default values.
   4.3 a configuration can be changed through "set" command while application is running.
   4.2 When the application terminates, its currently used configurations will be saved to the file of "appSettings.json" for future use.

7. Logs:

   There is a "logs" folder for applicaton file processing history and  error records: "err.txt".

8. Unit Tests:

   Four xUnit testing cases are created to test SHA256 checksum generation and file validation methods.



Notes:

  The methods about SHA_256 CheckSum generation and gz compressing are created based on online source.

