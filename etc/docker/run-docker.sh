#!/bin/bash

if [[ ! -d certs ]]
then
    mkdir certs
    cd certs/
    if [[ ! -f localhost.pfx ]]
    then
        dotnet dev-certs https -v -ep localhost.pfx -p c9add8fa-dfa7-4ede-a0f1-149c34370b06 -t
    fi
    cd ../
fi

docker-compose up -d
