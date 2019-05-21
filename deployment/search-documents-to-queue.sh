#!/bin/bash

shopt -s globstar
for i in $1; do
        echo "Processing $i"
        BODY=`cat $i`        
        retVal=$(aws sqs send-message --queue-url $2 --message-body "$BODY")
        if [ $retVal -ne 0 ]; then
            echo "An error occured"
            exit $retVal
        fi
done
