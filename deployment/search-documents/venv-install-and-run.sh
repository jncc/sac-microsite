#!/bin/bash
# Setup a proper path, I call my virtualenv dir "venv" and
# I've got the virtualenv command installed in /usr/local/bin
PATH=$WORKSPACE/venv/bin:/usr/local/bin:$PATH
if [ ! -d "venv" ]; then
        virtualenv venv
fi
. venv/bin/activate
pip install -r $WORKSPACE/master/deployment/search-documents/requirements.txt
python $WORKSPACE/master/deployment/search-documents/clearExistingIndexContents.py -s "sac" -i "$AWS_ELASTICSEARCH_INDEX" --host "$AWS_ELASTICSEARCH_URL"
python $WORKSPACE/master/deployment/search-documents/sendDocumentsToQueue.py -p "$WORKSPACE/master/output/search" -q "$AWS_SQS_QUEUE_URL"