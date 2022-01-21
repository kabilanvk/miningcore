#!/bin/bash
set -x
set -e

#Param(s)
SRC_DIR=$1
OUT_DIR=$2
BUILD_CONFIG=$3

echo 'Host: $(PERSISTENCE_POSTGRES_HOST)'
echo 'Host: $PERSISTENCE_POSTGRES_HOST'

#Dependencies
sudo apt-get update -y && sudo apt-get install -y --no-install-recommends jq 

#Update config
cd $SRC_DIR
jq '.persistence.postgres.host="$(PERSISTENCE_POSTGRES_HOST)"|.persistence.postgres.user="$(persistence-postgres-user)"|.persistence.postgres.password="$(persistence-postgres-password)"|.persistence.cosmos.endpointUrl="$(persistence-cosmos-endpointUrl)"|.persistence.cosmos.authorizationKey="$(persistence-cosmos-authorizationKey)"|.persistence.cosmos.databaseId="$(persistence-cosmos-databaseId)"' $SRC_DIR/src/Miningcore.Integration.Tests/config_test.json > tmp.$$.json && mv tmp.$$.json $SRC_DIR/src/Miningcore.Integration.Tests/config_test.json
