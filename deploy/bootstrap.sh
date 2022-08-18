#!/bin/bash
set -e

# Adapted from https://github.com/wmnnd/nginx-certbot
# Script file: https://raw.githubusercontent.com/wmnnd/nginx-certbot/9fdb9461e77e40459b4a616bf11ce2bb1cdca75a/init-letsencrypt.sh
# Original Copyright (c) 2018 Philipp Schmieder

function help() {
   echo "Usage: ${0##*/} -d \"example.com www.example.com\" -e \"certmanager@example.com\" [-s] [-h]"
   echo
   echo "Initialize Let's Encrypt certificates for specified domains and start services."
   echo
   echo "Options:"
   echo "n     Deployment name for Docker Compose. (Required)"
   echo "d     Whitespace separated list of domains to request certificates for. (Required)"
   echo "e     Email address to associate with Let's Encrypt certificates issued. (Required)"
   echo "s     If provided, Let's Encrypt requests will be issued against the staging servers."
   echo "      (Use this when debugging, default is off.)"
   echo
   echo "h     Print this help message."
   echo
}

rsa_key_size=4096
staging=0
domains=()
email=""
name=""

while getopts n:d:e:sh flag; do
  case "${flag}" in
  n)
    name=("${OPTARG}")
    ;;
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
    echo "${0##*/}: Illegal argument(s)." >&2
    help
    exit 1
    ;;
  esac
done

if [ "$name" == "" ] || [ ${#domains[@]} -eq 0 ] || [ "$email" == "" ]; then
  echo "${0##*/}: Missing argument(s)." >&2
  help
  exit 1
fi

function main() {
  if runOnCertbot "test -f /etc/letsencrypt/.bootstrap"; then
    echo ">>> Already bootstrapped, starting services ..."
    start "proxy app api certbot"
    exit 0
  fi

  echo ">>> Downloading TLS parameters ..."
SCRIPT=$(cat << END
    set -e
    mkdir -p /etc/letsencrypt/conf
    apk add curl
    curl -o- https://raw.githubusercontent.com/certbot/certbot/master/certbot-nginx/certbot_nginx/_internal/tls_configs/options-ssl-nginx.conf | tee /etc/letsencrypt/options-ssl-nginx.conf
    curl -o- https://raw.githubusercontent.com/certbot/certbot/master/certbot/certbot/ssl-dhparams.pem | tee /etc/letsencrypt/ssl-dhparams.pem
END
)
  runOnCertbot "$SCRIPT"

  echo ">>> Creating dummy certificate(s) for ${domains[*]} ..."
SCRIPT=$(cat << END
    set -e
    for d in $domains
    do
      echo \$d
      mkdir -p /etc/letsencrypt/live/\$d
      openssl req -x509 -nodes -newkey rsa:$rsa_key_size -days 1 \
        -keyout /etc/letsencrypt/live/\$d/privkey.pem \
        -out /etc/letsencrypt/live/\$d/fullchain.pem \
        -subj /CN=localhost
    done
END
)
  runOnCertbot "$SCRIPT"
  echo

  echo ">>> Starting proxy ..."
  docker compose --project-name "$name" up --force-recreate -d proxy
  echo

  echo ">>> Deleting dummy certificate(s) for ${domains[*]} ..."
SCRIPT=$(cat << END
    set -e
    for d in $domains
    do
      echo \$d
      rm -Rf /etc/letsencrypt/live/\$d
      rm -Rf /etc/letsencrypt/archive/\$d
      rm -Rf /etc/letsencrypt/renewal/\$d.conf
    done
END
)
  runOnCertbot "$SCRIPT"
  echo

  echo ">>> Requesting Let's Encrypt certificate for ${domains[*]} ..."
  if [ $staging != "0" ]; then
    staging_arg="--staging"
    echo "Using Let's Encrypt staging servers."
  else
    echo "Using Let's Encrypt production servers."
  fi

SCRIPT=$(cat << END
    set -e
    for d in $domains
    do
      echo \$d
      certbot certonly -v --webroot -w /var/www/certbot \
        $staging_arg \
        -d \$d \
        --email $email \
        --rsa-key-size $rsa_key_size \
        --no-eff-email \
        --agree-tos \
        --force-renewal
    done
END
)
  runOnCertbot "$SCRIPT"
  echo

  echo ">>> Reloading proxy ..."
  docker compose --project-name "$name" exec proxy nginx -s reload

  echo ">>> Marking environment as bootstrapped ..."
  runOnCertbot "touch /etc/letsencrypt/.bootstrap"

  echo ">>> Bootstrapped, starting services ..."
  start "proxy app api certbot"
}

function runOnCertbot() {
  script="$1"
  docker compose --project-name "$name" run -it --rm --entrypoint "/bin/ash -c '$script'" certbot
}

function start() {
  services="$1"
  docker compose --project-name "$name" up -d $services
}

main
