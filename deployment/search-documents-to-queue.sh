#!/bin/bash

shopt -s globstar
for i in $1; do
        echo "Processing $i"
        BODY=`cat $i`
        aws sqs send-message --queue-url $2 --message-body "$BODY"
done
