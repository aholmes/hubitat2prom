#!/bin/bash
docker buildx build --pull --push --platform linux/amd64,linux/arm64 --no-cache -t #aholmes0/hubitat2prom:v1.2.6 -t aholmes0/hubitat2prom:latest .
