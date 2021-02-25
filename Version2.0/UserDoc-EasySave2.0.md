#Documentation User EasySave2.0

###THE USER INTERFACE

<img src="https://media.discordapp.net/attachments/626452497871011862/784897917877223484/Doc_User_-_EasySave2.0.png?width=1143&height=591" alt="">

###Log file
* The log file is saved here: `\EasySaveApp\Logs\`

###State file
* The state file is saved here: `\EasySaveApp\bin\Debug\netcoreapp3.1\State\state.json`

###BlackList file
* The blacklist file is located at: `\EasySaveApp\Ressources\BlackList.json`
<p>To fill this file there is a syntax to respect.
<p>It is necessary to separate the software to be banned by a ","
<pre><code>[ 
    { "blacklisted_items": "calculator,notepad" } 
]</code></pre>

###File of extensions to be encrypted
* The file of the extensions to be encrypted is located at: `\EasySaveApp\Ressources\CryptExtension.json`
<p>To fill this file there is a syntax to respect.
<p>It is necessary to separate the software to be banned by a ","
<pre><code>[
  { "extension_to_crypt": ".txt,.csv" }
]</code></pre>