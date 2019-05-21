from elasticsearch import Elasticsearch, RequestsHttpConnection
from requests_aws4auth import AWS4Auth
import argparse
import logging
import os
import boto3

def main(host, index, site):
    # Set up logging
    logging.basicConfig(level=logging.INFO,
                        format='%(levelname)s: %(asctime)s: %(message)s')

    region = os.environ['AWS_DEFAULT_REGION']
    service = 'es'

    awsauth = AWS4Auth(os.environ['AWS_ACCESS_KEY_ID'], os.environ['AWS_SECRET_ACCESS_KEY'], region, service)

    es = Elasticsearch(
        hosts = [{'host': host, 'port': 443}],
        http_auth = awsauth,
        use_ssl = True,
        verify_certs = True,
        connection_class = RequestsHttpConnection
    )

    logging.info(f'Deleting all documents where site={site}')

    es.delete_by_query(index=index, body={"query": {"match": {"site": site}}})


if __name__ == '__main__':
    # Setup command line switches
    parser = argparse.ArgumentParser(description='Clear an existing elasticsearch index by its `site` value.')
    parser.add_argument('-s' , '--site', type=str, required=True, help='The site to clear from the given index')
    parser.add_argument('-i' , '--index', type=str, required=True, help='The index to clear from')
    parser.add_argument('--host', type=str, required=True, help='The url of the existing elasticsearch service')
    
    args = parser.parse_args()

    # Run main proc
    main(args.host, args.index, args.site)