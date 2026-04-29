#!/bin/bash

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"

# get variables from script args
PORT=$1
INSTANCES=$2

# validation of variables
if [ -z "$PORT" ]; then
    echo "The starting port was not specified. Defaulting to 8000"
    PORT=8000
fi

if [ -z "$INSTANCES" ]; then
    echo "The number of instances was not specified. Defaulting to 2"
    INSTANCES=2
fi

# export variables for all processes
export APP_PORT=$PORT
export APP_REPLICAS=$INSTANCES

echo "Starting system on port: $APP_PORT with $APP_REPLICAS instances"

# clean old containers and build new one
docker compose -f "$SCRIPT_DIR/Infrastructure/docker-compose.yml" up --build --scale rest-api=$APP_REPLICAS --remove-orphans