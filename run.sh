#!/bin/bash

# SPDX-FileCopyrightText: 2022 Simon Wendel
# SPDX-License-Identifier: GPL-3.0-or-later

set -e

function help() {
  echo "Usage: ${0##*/} [-p|h]"
  echo
  echo "Start Push-a-Secret with pre-defined domain names and administrative info."
  echo
  echo "Options:"
  echo "p     If provided, Let's Encrypt requests will be issued against production servers."
  echo "      (Use this when you want official certificates instead of test certs, ie, when releasing.)"
  echo
  echo "h     Print this help message."
  echo
}

function configsNeeded() {
  echo "All configuration variables are required, for example:"
  echo
  echo "APP_DOMAIN=www.example.com"
  echo "API_DOMAIN=api.example.com"
  echo "CERT_EMAIL=certmanager@example.com"
  echo
}

function missingEnv() {
  echo "Could not find the .env file with application configs."
  echo "Create a .env file next to ${0##*/}"
  echo
  configsNeeded
}

function reminder() {
  echo "The .env file seems to be missing proper configuration:"
  echo
  cat .env
  echo
  configs
}

production=0
while getopts ph flag; do
  case "${flag}" in
  p)
    production=1
    ;;
  h)
    help
    exit 0
    ;;
  *)
    echo "${0##*/}: Illegal argument(s)." >&2
    echo
    help
    exit 1
    ;;
  esac
done

if [ ! -f .env ]; then
  missingEnv
  exit 1
fi

# shellcheck disable=SC2046
export $(cat .env | sed 's/#.*//g' | envsubst | xargs)
echo "Read the following configuration from .env file: "
echo
echo "APP_DOMAIN=$APP_DOMAIN"
echo "API_DOMAIN=$API_DOMAIN"
echo "CERT_EMAIL=$CERT_EMAIL"
echo

if [ "$API_DOMAIN" == "" ] || [ "$APP_DOMAIN" == "" ] || [ "$CERT_EMAIL" == "" ]; then
  reminder
  exit 1
fi

staging_arg="-s"
if [ $production -eq 1 ]; then
  staging_arg=""
fi

env -C deploy ./bootstrap.sh -n "push-a-secret" -d "$APP_DOMAIN $API_DOMAIN" -e "$CERT_EMAIL" $staging_arg
