# sac-microsite
Special Areas of Conservation (SACs) are strictly protected sites designated under the EC Habitats Directive.

## Readme

The sac microsite is hosted on github pages and is published from the gh-pages branch

The data manager can `git clone` clone the site on their local PC, and use the readme.md file to follow the instructions.

The PC will only need Dotnet SDK/Runtime, Access x64 drivers and Git installed; both are available as Windows MSI installers. A text editor such as VS Code is recommended. Github Desktop is very helpful too. 

## Requirements

The data extractor part of this project requires the x64 Microsoft Access drivers to be installed for this to work, the drivers can be found [here](https://www.microsoft.com/en-gb/download/details.aspx?id=13255). Choosing `AccessDatabaseEngine_X64.exe` when promted. This however means updating the data files will **ONLY** work on Windows.

If you already have a 32-bit install of Microsoft office you will need to run the install in passive mode, open an admin console (powershell or command prompt), navigate to the folder and run;

```
AccessDatabaseEngine_X64.exe /quiet
```
You will need to have an admin account if you do the updates or development on a windows machine and wish to use the dotnet sdk. Otherwise you will have to follow the instructions for [Updates with an unprivaliged login](#updates-with-an-unprivaliged-login)

## Local development

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

## Updating the site

The site is automatically redeployed to an internal beta site at http://beta-sac available internally on commits to the devleop branch, an update just requires the json files in [output/data](output/data) to be updated using the following commands;

    github clone https://github.com/jncc/sac-microsite.git
    cd sac-microsite
    dotnet restore
    dotnet build
    dotnet run -- -u "path/to/new/natura2000.mdb"
    git commit -a -m "Updated data files YYYY-MM-DD"
    git push

After which the newly committed json files on the master branch which will be automatically redeployed to an internal beta site at http://beta-sac.

## Deploying the site to live

We have created an Jenkins pipeline to deploy the site to live which is triggered currently on commits to master, that pipeline is commited alongside the code at [deployment/jenkins/deploy-to-live-pipeline](deployment/jenkins/deploy-to-live-pipeline) where it just needs to be fed the correct variables, i.e.;

    AWS_DEFAULT_REGION = '${AWS_REGION}' 
    AWS_SQS_QUEUE_HOST = '${AWS_SQS_QUEUE_HOST}'
    AWS_ELASTICSEARCH_HOST = '${AWS_ELASTICSEARCH_HOST}'
    AWS_ELASTICSEARCH_INDEX = '${AWS_ELASTICSEARCH_INDEX}'
    GOOGLE_ANALYTICS_ID = google analytics ID - ie UA-00000000-0
    GOOGLE_TAG_MANAGER_ID = google tag manager ID - ie GTM-XXX00XX

These variables determine the AWS region we are running in, the name of an AWS SQS queue endpoint to push the search documents into for ingestion, the AWS Elasticsearch endpoint and the index to post results into. More info about the elasticsearch ingestion process can be found at jncc/elasticsearch-lambda-ingester.

The Jenkins pipeline will run the following

    dotnet run -- -g -s $AWS_ELASTICSEARCH_INDEX

This creates a set of json search documents in `output/search` that can be pushed onto the queue to ingest into the central search ingester lambda function, it will also create the static pages in `output/html` (in the .gitignore) which can be combined with the other static elements in the [docs](docs) folder which will then be pushed out onto the `gh-pages` branch (the live SAC microsite).

### Manual Deployment to live

To manually deploy a new version we just need to run the following;

    github clone https://github.com/jncc/sac-microsite.git
    cd sac-microsite
    dotnet restore
    dotnet build
    dotnet run -- -g -s $AWS_ELASTICSEARCH_INDEX -a $GOOGLE_ANALYTICS_ID -t $GOOGLE_TAG_MANAGER_ID'
    cd ..

This builds the new pages and the updated search documents as above, then we will need to run the following;

    mkdir gh-pages
    cd gh-pages 
    git clone https://github.com/jncc/sac-microsite.git . --branch=gh-pages --depth 1
    cp -r ../sac-microsite/output/html/* ./ 
    git add --all
    git commit -a -m "Manual Build YYYY-MM-DD HH:MM:SS" 
    git push --force

This checks out the `gh-pages` branch merges the output from the current data files and commits it to the branch and becomes the new live site.

#### Search Index Update [Requires Python 3 and Virtualenv]

If you update the site manually you should also update the search index, a delete by query would need to be run on the `search-index` index (where that index is the elasticsearch index currently in use), this could be achieved in multiple ways, but the basic building block is to run an elasticsearch delete by query;

```
POST search-index/_delete_by_query
{
  "query": { 
    "match": {
      "site": "sac"
    }
  }
}
```

We have a basic helper scripts to carry out this function, located in [deployment/search-documents](deployment/search-documents). They require a python virtualenv and a set of requirements from `requirements.txt`. The two python commands should be run sequentially, the first `clearExistingIndexContents.py` deletes all contents of a given index / site combination and `sendDocumentsToQueue.py` which sends all JSON documents matching a given GLOB pattern to the given SQS endpoint;

    cd ./deployment/search-documents
    virtualenv venv
    source ./venv/bin/activate
    pip install -r requirements.txt
    python clearExistingIndexContents.py -s sac -i $AWS_ELASTICSEARCH_INDEX --host  $AWS_ELASTICSEARCH_HOST
    python sendDocumentsToQueue.py -p "../../output/search/**/*.json" -q $AWS_SQS_QUEUE_HOST

## Updates with an unprivaliged network account

You need to be an admin on your machine to use the dotnet tools to build the site. An executable is built and packeged by jenkins to get around this issue for unprivaliged users.

Checkout the SAC project from git. The folder containing the project is your root folder.

Download the latest JNCC.Microsite.SAC_{version}.zip
file [from the releases section in git](https://github.com/jncc/sac-microsite/releases) and extract it to a suitable location outside of the sac project.

### Update the data
Get a copy of the Natura database and run the executable JNCC.Microsite.SAC.exe from the extracted release zip file.

For example, given that: 

* sac root path = c:\development\sac-microsite
* ASP natura database path = c:\development\ASP NATURA DATABASE.mdb
* JNCC.Microsite.SAC.exe is in the curent folder

Run the following to update the data

    JNCC.Microsite.SAC.exe -r c:\development\sac-microsite -u "c:\development\ASP NATURA DATABASE.mdb"

This proces will update the json files in c:\development\sac-microsite\output\json

### Update the website

The following instruciotns will update the web pages using the data extracted in the previous step.

    JNCC.Microsite.SAC.exe -r c:\development\sac-microsite -g -v

The -v switch intitiates a web server once the pages have been generated.  You can browse the site at http://localhost:5000/

### Updating page templates

Updating pages and regenerating the site is more convoluted in an unprivaliged environment because the changes have to be incorporated into the executable.

Check out the solution and make the changes to the page templates in Views.

Commit the changes to the master branch. This will trigger jenkins to build a new executable and put it into the [github releases folder.](https://github.com/jncc/sac-microsite/releases)

Any build failures will be visible in the build server logs.

Download the new executable and follow the instructions for [updating the data](#update-the-data) and [updating the website](#update-the-website).

## Updating Images in /doc/images

If you need to update the images for the maps, make sure that you follow the existing naming convention exactly, if you need to regenerate thumbnails you can use the ImageMagick Mogrify tool to create them i.e.;

    magick mogrify -resize 300x388 -quality 100 -path ./docs/mages/maps/features/uk/thumbnails ./docs/images/maps/features/uk/*.gif

You can then bulk rename them in Windows;

    ren *.gif ?????_thumb.*
    i.e. S1365.gif > S1365_thumb.gif

OR

    ren *.gif ????????_thumb.*
    i.e. uk_S1365.gif > uk_S1365_thumb.gif

Or under linux using rename or something similar;

    rename 's/\.gif$/_thumb\.gif/g' *.gif
