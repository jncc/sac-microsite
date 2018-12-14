# sac-microsite
Special Areas of Conservation (SACs) are strictly protected sites designated under the EC Habitats Directive.

Readme (to implement!)
----------------------

Plan is to make a static site. Hosted on Github Pages (or similar).

The data manager can `git clone` clone the site on their local PC, and use the readme.md file to follow the instructions.

The PC will only need Node and Git installed; both are available as Windows MSI installers. A text editor such as VS Code is recommended. Github Desktop is very helpful too. 

Local development
-----------------

Open a command terminal in the `sac-microsite` folder. 

Ensure you have the lastest commit of code from Github by using Github Desktop or `git pull` if you are comfortable with the Git command line. Then run

    yarn
    
to ensure you have the latest libraries installed. Then open your text editor in the current directory. To open in VS Code, run

    code .    <-- (note the full stop!)

Ensure that you have created or edited the `.env` file in the project root to contain the connection string to the Access database. This file isn't committed to Github. For example:

    DB_CONNECTION=Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Z:\thefolder\sac-db.accdb;Persist Security Info=False;

To get the latest data dump from the database, run 

    yarn data 

This reads the master Access database and saves a JSON representation of the tables as .json files in the `data/` folder. (This is a cache so that your local dev experience is good.) If the data has changed since this was last done, you can see the changes in Github Desktop, or with `git status` and `git diff`.

The next command is optional, as it's called automatically by the later commands. It builds the static site and the search engine index entries from the JSON data files.

    yarn build
  
To pop open a local web server and open your browser at the built microsite.

    yarn dev 
    
You can edit the markdown files in the `content/` folder to update the small amount of editable content in the microsite. 

To deploy the microsite,

- Push to Github
- Press a button on Jenkins to deploy the latest version.

Notes for devs
--------------

The build/dev commands might be a provided by the static site generator. 

The Jenkins build command will probably run:

    yarn deploy
    
This 

1. builds the site
2. upserts (all) the index entries in ElasticSearch
3. pushes to Github Pages
