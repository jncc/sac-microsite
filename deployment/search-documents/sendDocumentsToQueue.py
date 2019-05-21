import argparse
import logging
import os
import boto3
import json
from botocore.exceptions import ClientError


def send_sqs_message(sqs_queue_url, msg_body):
    """

    :param sqs_queue_url: String URL of existing SQS queue
    :param msg_body: String message body
    :return: Dictionary containing information about the sent message. If
        error, returns None.
    """

    # Send the SQS message
    sqs_client = boto3.client('sqs')
    try:
        msg = sqs_client.send_message(QueueUrl=sqs_queue_url,
                                      MessageBody=msg_body)
    except ClientError as e:
        logging.error(e)
        return None
    return msg


def getFileList(path, excludeSubDirectories):
    fileList = []

    if excludeSubDirectories:
        for document in os.listdir(path):
            if document.endswith(".json"):
                fileList.append(os.path.join(path, document))
    else:
        for root, dirs, files in os.walk(path):
            for document in files:
                if document.endswith(".json"):
                    fileList.append(os.path.join(root, document))
    
    return fileList


def getJSONStr(path):
    with open(path) as f:
        searchDocument = json.load(f)
        return json.dumps(searchDocument)


def main(path, excludeSubDirectories, queueUrl):
    # Set up logging
    logging.basicConfig(level=logging.INFO,
                        format='%(levelname)s: %(asctime)s: %(message)s')

    # Send some SQS messages
    fileList = getFileList(path, excludeSubDirectories)
    for document in fileList:
        msg_body = f'{getJSONStr(document)}'
        msg = send_sqs_message(queueUrl, msg_body)
        if msg is not None:
            logging.debug(f'Sent SQS message ID: {msg["MessageId"]}')
        else:
            logging.error(f'Could not send message')
            exit(1)
    logging.info(f"Pushed {len(fileList)} Search Documents")


if __name__ == '__main__':
    # Setup command line switches
    parser = argparse.ArgumentParser(description='Upload a set of json documents to an SQS queue.')
    parser.add_argument('-p' , '--path', type=str, required=True, help='The base path to search JSON documents from')
    parser.add_argument('-e' , '--exclude', action='store_true', required=False, default=False, help='Optionally exclude subdirectories for JSON documents')
    parser.add_argument('-q', '--queue', type=str, required=True, help='The URL to the SQS queue to submit to')
    
    args = parser.parse_args()

    # Run main proc
    main(args.path, args.exclude, args.queue)