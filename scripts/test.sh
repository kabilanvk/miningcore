#!/bin/bash
set -x
set -e

#Param(s)
SRC_DIR=$1
OUT_DIR=$2
BUILD_CONFIG=$3

#Dependencies
sudo apt-get update -y && sudo apt-get install -y --no-install-recommends jq 

#Update config
cd $SRC_DIR
jq '.persistence.postgres.host="'+$PERSISTENCE_POSTGRES_HOST+'"|.persistence.postgres.user="'+$PERSISTENCE_POSTGRES_USER+'"|.persistence.postgres.password="'+$PERSISTENCE_POSTGRES_PASSWORD+'"|.persistence.cosmos.endpointUrl="'+$PERSISTENCE_COSMOS_ENDPOINTURL+'"|.persistence.cosmos.authorizationKey="'+$PERSISTENCE_COSMOS_AUTHORIZATIONKEY+'"|.persistence.cosmos.databaseId="'+$PERSISTENCE_COSMOS_DATABASEID+'"' $SRC_DIR/src/Miningcore.Integration.Tests/config_test.json > tmp.$$.json && mv tmp.$$.json $SRC_DIR/src/Miningcore.Integration.Tests/config_test.json
