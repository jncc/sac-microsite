# Setup a proper path, I call my virtualenv dir "venv" and
# I've got the virtualenv command installed in /usr/local/bin
PATH=$WORKSPACE/venv/bin:/usr/local/bin:$PATH
if [ ! -d "venv" ]; then
        virtualenv venv
fi
. venv/bin/activate
pip install -r $WORKSPACE/deployment/search-documents/requirements.txt --download-cache=/tmp/$JOB_NAME