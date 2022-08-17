#!/bin/bash

# Adapted from https://github.com/wmnnd/nginx-certbot
# Script file: https://raw.githubusercontent.com/wmnnd/nginx-certbot/9fdb9461e77e40459b4a616bf11ce2bb1cdca75a/init-letsencrypt.sh
# Original Copyright (c) 2018 Philipp Schmieder

function help() {
   echo "Usage: ${0##*/} -d \"example.com www.example.com\" -e \"certmanager@example.com\" [-s] [-h]"
   echo
   echo "Initialize Let's Encrypt certificates for specified domains and start services."
   echo
   echo "Options:"
   echo "d     Whitespace separated list of domains to request certificates for. (Required)"
   echo "e     Email address to associate with Let's Encrypt certificates issued. (Required)"
   echo "s     If provided, Let's Encrypt requests will be issued against the staging servers."
   echo "      (Use this when debugging, default is off.)"
   echo
   echo "h     Print this help message."
   echo
}

if docker compose run -it --entrypoint "test -f \"/etc/letsencrypt/.bootstrap\"" certbot; then
  echo ">>> Already bootstrapped, starting services ..."
  docker compose up -d proxy app api certbot
  exit 0
fi

rsa_key_size=4096
staging=0
domains=()
email=""

while getopts d:e:sh flag; do
  case "${flag}" in
  d)
    domains=("${OPTARG}")
    ;;
  e)
    email=${OPTARG}
    ;;
  s)
    staging=1
    ;;
  h)
    help
    ;;
  *)
    echo "Illegal argument(s)." >&2
    help
    exit 1
    ;;
  esac
done

if [ ${#domains[@]} -eq 0 ] || [ "$email" == "" ]; then
  echo "Missing argument(s)." >&2
  help
  exit 1
fi

echo ">>> Downloading TLS parameters ..."
docker compose run -it --entrypoint "\
  mkdir -p \"/etc/letsencrypt/conf\" \
  curl -s https://raw.githubusercontent.com/certbot/certbot/master/certbot-nginx/certbot_nginx/_internal/tls_configs/options-ssl-nginx.conf >\"/etc/letsencrypt/options-ssl-nginx.conf\" \
  curl -s https://raw.githubusercontent.com/certbot/certbot/master/certbot/certbot/ssl-dhparams.pem >\"/etc/letsencrypt/ssl-dhparams.pem\"" certbot

echo ">>> Creating dummy certificate for ${domains[*]} ..."
paths="/etc/letsencrypt/live/$domains"
docker compose run --rm --entrypoint "\
  mkdir -p "/etc/letsencrypt/live/$domains" \
  openssl req -x509 -nodes -newkey rsa:$rsa_key_size -days 1\
    -keyout '$paths/privkey.pem' \
    -out '$paths/fullchain.pem' \
    -subj '/CN=localhost'" certbot
echo

echo ">>> Starting proxy ..."
docker compose up --force-recreate -d proxy
echo

echo ">>> Deleting dummy certificate for ${domains[*]} ..."
docker compose run --rm --entrypoint "\
  rm -Rf /etc/letsencrypt/live/$domains && \
  rm -Rf /etc/letsencrypt/archive/$domains && \
  rm -Rf /etc/letsencrypt/renewal/$domains.conf" certbot
echo

echo ">>> Requesting Let's Encrypt certificate for ${domains[*]} ..."
domain_args=""
for domain in "${domains[@]}"; do
  domain_args="$domain_args -d $domain"
done

if [ $staging != "0" ]; then staging_arg="--staging"; fi
docker compose run --rm --entrypoint "\
  certbot certonly --webroot -w /var/www/certbot \
    $staging_arg \
    $domain_args \
    --email $email \
    --rsa-key-size $rsa_key_size \
    --agree-tos \
    --force-renewal" certbot
echo

echo ">>> Reloading proxy ..."
docker compose exec proxy nginx -s reload

echo ">>> Marking environment as bootstrapped ..."
docker compose run -it --entrypoint "touch \"/etc/letsencrypt/.bootstrap\"" certbot

echo ">>> Bootstrapped, starting services ..."
docker compose up -d app api certbot
