# sac-microsite
Special Areas of Conservation (SACs) are strictly protected sites designated under the EC Habitats Directive.

Readme (to implement!)
----------------------

Plan is to make a static site. Hosted on Github Pages (or similar).

The data manager can `git clone` clone the site on their local PC, and use the readme.md file to follow the instructions.

The PC will only need Dotnet SDK/Runtime, Access x64 drivers and Git installed; both are available as Windows MSI installers. A text editor such as VS Code is recommended. Github Desktop is very helpful too. 

Requirements
------------

The data extractor part of this project requires the x64 Microsoft Access drivers to be installed for this to work, the drivers can be found [here](https://www.microsoft.com/en-gb/download/details.aspx?id=13255). Choosing `AccessDatabaseEngine_X64.exe` when promted. This however means updating the data files will **ONLY** work on Windows.

If you already have a 32-bit install of Microsoft office you will need to run the install in passive mode, open an admin console (powershell or command prompt), navigate to the folder and run;

```
AccessDatabaseEngine_X64.exe /quiet
```

Local development
-----------------

Open a command terminal in the `sac-microsite` folder. 

Ensure you have the lastest commit of code from Github by using Github Desktop or `git pull` if you are comfortable with the Git command line. Then run

    dotnet restore
    dotnet build
    
to ensure you have the latest libraries installed. Then open your text editor in the current directory. To open in VS Code, run

    code .    <-- (note the full stop!)

To get the latest data dump from the database, run 

    dotnet run -- -u path/to/access.mdb

This reads the master Access database and saves a JSON representation of the tables as .json files in the `output/data/` folder. (This is a cache so that your local dev experience is good.) If the data has changed since this was last done, you can see the changes in Github Desktop, or with `git status` and `git diff`.

The next command, builds the static site from the JSON data files.

    dotnet run -- -g
  
To start a local web server with the built static microsite run;

    dotnet run -- -v

You can combine these runs into a single command;

    dotnet run -- -u path/to/access.mdb -g -v
    
You can edit the templates as required in the `views/` folder, but most of the editable data is directly extracted from the source Access MDB, you can try minor edits through the fields in the `output/json` files. 

To deploy the microsite,

- Push to Github
- Press a button on Jenkins to deploy the latest version.

Notes for devs (Internal)
--------------

Deployment
--------------

The site is automatically redeployed to an internal beta site at http://beta-sac available internally on every commit, if you need to deploy the main site run the relevant Jenkins job on the internal development server. This will run the equivalent of the following;

    dotnet run -- -g -s search-index-name -v

Where `search-index-name` is the name of the search index that this site will add into, this creates a set of json search documents in `output/search` that can be pushed onto the queue to ingest into the central search system, this will be hanlded by the central jenkins deploy to live job for this microsite. The job creates the output and joins it with the static content in /docs (css, js and images) and force pushes it on the `gh-pages` branch which is pointed to by the new SAC url.

