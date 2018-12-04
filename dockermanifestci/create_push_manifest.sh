#! /bin/bash
docker manifest create fbeltrao/dockermanifestci:1.0 \
  fbeltrao/dockermanifestci:1.0-amd64 \
  fbeltrao/dockermanifestci:1.0-arm32v7
  
docker manifest push fbeltrao/dockermanifestci:1.0