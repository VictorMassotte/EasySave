#Documentation User

Start the application
=================
Once you have installed our program with the help of our Getting Started solution you will have multiple choices.

###Add a backup job
> To perform a backup, you must first create a backup job.
* First, you need to choose whether your backup is a full or a differential. (A full backup will copy every files / folder while a differential one compares itself to a full and copy only modified files).
* To get started, you need to give your backup a name.
* Then you must add the path of your folder to save.
* Then put the path to your destination folder.
* For differential backup, you need to put the path to your folder where a full backup was made.

###Load backup job and execute
* To start a backup job, you must enter the name of the backup you want to do.
* After filling in the name of the backup, the execution will start. If the backup works fine, you will get a validation message.

###Log file
* The log file is saved here: `\EasySaveApp\bin`

###State file
* The state file is saved here: `\EasySaveApp\bin\Debug\netcoreapp3.1\State\state.json`