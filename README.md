# Push-a-Secret

## Pre-requisites

Dependencies required on the hosting server:

* Git (duh)
* Docker
* Docker Compose (separate CLI plugin, or bundled)

## Running the app

1. Clone Git repo.
2. Create .env file in repo root with domains and email address for Let's Encrypt certificates, for example:
    ```
    APP_DOMAIN=www.example.com
    API_DOMAIN=api.example.com
    CERT_EMAIL=certmanager@example.com
    ```
3. Deploy the app using the `run.sh` script
    a. Test the setup by supplying no flags to the script.
    b. When bootstrapped with staging certs, run `remove.sh` to completely remove the app.
    c. Go live by invoking the script adding `-p` to create production certificates.
4. If the services are stopped, re-run `run.sh` to start them, once bootstrapped certs will only by renewed, never replaced.

## Attribution

* Favicon created using art from the awesome Twemoji project by Twitter ([twemoji website]).

[twemoji website]: https://twemoji.twitter.com/