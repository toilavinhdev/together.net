#!/bin/sh
# https://github.com/dotnet/dotnet-docker/blob/main/samples/run-aspnetcore-https-development.md
mkdir -p -m 700 ${HOME}/.aspnet/https &&
dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p p@ssword