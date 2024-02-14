# SAC Microsite

Special Areas of Conservation (SACs) are strictly protected sites designated under the EC Habitats Directive.

The SAC microsite https://sac.jncc.gov.uk is hosted on Github Pages. The website is a "static site" built from data held in the master Access database.

A data manager can clone the site on their local PC, and follow the instructions in this readme file to update the site.

# Dev and build container

The development environment for this solution is containerised. The current image can be pulled from AWS.

*BEFORE* you start visual studio code you need to log docker into aws in order to enable it to pull the image for the first time.

You only need to log in if:
* its the first time starting this solution
* if you want to use a newer image
* if you are changing the dev or build container - see instructions further down

Run the following commands from within the SAC solution folder
```
# log docker into to the aws cicd account (set the cicd admin profile name below)
aws ecr --profile <cicd admin profile> get-login-password --region eu-west-2 | docker login --username AWS --password-stdin 137043356624.dkr.ecr.eu-west-2.amazonaws.com

# start vs code
code .
```

*To run the dev env in an environment built locally from the docker file:*

* comment out the line starting *image*
* uncomment the codeblock for *build* that gives the relative path to the dockerfile

The continer can be built manually:

```
cd /deployment/docker-build-env
docker build -t jncc/sac_build_env .
```

To run the container manually:

```
docker run -u vscode -it -p 5000:5000 --rm -v <host path to /sac-microsite>:/sac-microsite jncc/sac_build_env
```

Ensure the dev container plugin is installed.

The container will be pulled from the AWS ECR container repo in the jncc-cicd account. Image details can be found in .devcontainer/devcontainer.json

This workspace should be mounted in /workspaces/sac-microsite by dev-container

## Updating the build container
This should be performed with sac in a buildable state so the container can be tested.

From within /deployment/docker-build-env:

Buid the docker file to the *buildenv* target:

```
docker build --target buildenv -t jncc/sac_build_env .
```

Check it's functionality by building the sac env.

```
docker run -it -p 5000:5000 --rm -v <local sac microsite folder>:/sac-microsite jncc/sac_build_env
cd /sac-microsite/
dotnet build
# wait for successful build
exit
```

Push the build image to the AWS repo

```
# log docker into to the aws cicd account
aws ecr --profile <cicd admin profile> get-login-password --region eu-west-2 | docker login --username AWS --password-stdin 137043356624.dkr.ecr.eu-west-2.amazonaws.com

# tag the image built in the previous step for a push, iterate the version.
docker tag jncc/sac_build_env 137043356624.dkr.ecr.eu-west-2.amazonaws.com/sac-build-container:1.0.0

# push the image to the repo
docker push 137043356624.dkr.ecr.eu-west-2.amazonaws.com/sac-build-container:1.0.0
```

## Updating the dev container

Follow the steps above to build and deploy the build container first.

From within /deployment/docker-build-env:

Buid the docker file:

```
docker build -t jncc/sac_dev_env .

# tag the image built in the previous step for a push, iterate the version.
docker tag jncc/sac_dev_env 137043356624.dkr.ecr.eu-west-2.amazonaws.com/sac-dev-container:1.0.0

# push the image to the repo
docker push 137043356624.dkr.ecr.eu-west-2.amazonaws.com/sac-dev-container:1.0.0

```

Edit .devcontainer/devcontainer.json in the solution

Locate the line beginning with "image" and change the tag on the end of the line to match the new version number.

# Working with dev containers



## Local development

Open a command terminal in the `sac-microsite` local respository folder.

Ensure you have the lastest commit of code from Github by using Github Desktop or `git pull` if you are comfortable with the Git command line. Then run

    dotnet restore
    dotnet build

Then open your text editor in the current directory. To open in VS Code, run

    code .    <-- (note the full stop!)

To build the site from the JSON data files, run

    dotnet run -- -g
  
To start a local web server, run

    dotnet run -- -v

You can edit the templates as required in the `views/` folder and re-run with

    dotnet run -- -g -v

## Data updates

If the data in the master Access database has changed, you need to requery the database.

This reads the master Access database and saves a JSON representation of the tables as .json files in the `output/data/` folder. This is a cache, so that you rarely need to connect to the database. When the data changes, you can see the changes in Git.

- Updating the data from the Access database will currently **ONLY work on Windows**.
- You will also need to have an **admin account**, because unfortunately `dotnet build` requires admin.

The Windows PC will need Dotnet SDK/Runtime and Git installed; both are available as Windows MSI installers.

You will need the x64 Microsoft Access drivers from https://www.microsoft.com/en-gb/download/details.aspx?id=13255. Choose `AccessDatabaseEngine_X64.exe`.

(If you already have a 32-bit install of Microsoft office you will need to run the install in passive mode, open an admin console (powershell or command prompt), navigate to the folder and run `AccessDatabaseEngine_X64.exe /quiet`)

Full list of steps to update the data:

    git clone https://github.com/jncc/sac-microsite.git
    cd sac-microsite
    git checkout develop
    dotnet restore
    dotnet build
    dotnet run -- -u "path/to/new/natura2000.mdb"
    dotnet run -- -g -v     # rebuild the pages and run locally to check.
    git commit -a -m "Updated data files"
    git push

After which the updated files on the `develop` branch which will be automatically redeployed to the internal beta site at http://internal-beta/

## Publishing to the live site

Merge the changes from the develop branch onto master:

        git checkout master
        git merge develop
        git push

Run  the `sac-microsite-deploy-to-live` Jenkins job.

## Jenkins pipeline

We have created an Jenkins pipeline [deployment/jenkins/deploy-to-live-pipeline](deployment/jenkins/deploy-to-live-pipeline) that just needs to be fed the correct variables, i.e.;

    AWS_DEFAULT_REGION = '${AWS_REGION}' 
    AWS_SQS_QUEUE_HOST = '${AWS_SQS_QUEUE_HOST}'
    AWS_ELASTICSEARCH_HOST = '${AWS_ELASTICSEARCH_HOST}'
    AWS_ELASTICSEARCH_INDEX = '${AWS_ELASTICSEARCH_INDEX}'
    GOOGLE_TAG_MANAGER_ID = google tag manager ID - ie GTM-XXX00XX

More info about the elasticsearch ingestion process can be found at jncc/elasticsearch-lambda-ingester.

The Jenkins pipeline will run the following

    dotnet run -- -g -s $AWS_ELASTICSEARCH_INDEX

This creates a set of json search documents in `output/search` that can be pushed onto the queue to ingest into the central search ingester lambda function, it will also create the static pages in `output/html` (in the .gitignore) which can be combined with the other static elements in the [docs](docs) folder which will then be pushed out onto the `gh-pages` branch (the live SAC microsite).

### Manual Deployment to live

To manually deploy a new version we just need to run the following;

    github clone https://github.com/jncc/sac-microsite.git
    cd sac-microsite
    dotnet restore
    dotnet build
    dotnet run -- -g -s $AWS_ELASTICSEARCH_INDEX -cookiecontrol -t $GOOGLE_TAG_MANAGER_ID'
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

## Updates with an unpriviledged network account

You need to be an admin on your machine to use the dotnet tools to build the site. An executable is built and packaged by jenkins to get around this issue for unpriviledged users.

Checkout the SAC project from git. The folder containing the project is your root folder.

Download the latest JNCC.Microsite.SAC_{version}.zip
file [from the releases section in git](https://github.com/jncc/sac-microsite/releases) and extract it to a suitable location outside of the sac project.

### Update the data

Get a copy of the Natura database and run the executable JNCC.Microsite.SAC.exe from the extracted release zip file.

For example, given that:

- sac root path = c:\development\sac-microsite
- ASP natura database path = c:\development\ASP NATURA DATABASE.mdb
- JNCC.Microsite.SAC.exe is in the curent folder

Run the following to update the data

    JNCC.Microsite.SAC.exe -r c:\development\sac-microsite -u "c:\development\ASP NATURA DATABASE.mdb"

This process will update the json files in c:\development\sac-microsite\output\json

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

# Notification Panel

The site has a simple notification panel that is displayed above the menu. The contents of the panel is in Views/Shared/_Navbar.cshtml

The bar itself can be enabled by commenting it out in line 71 of Views/Shared/_Layout.cshtml 

