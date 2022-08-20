# Push-a-Secret

A zero-knowledge scheme for sharing secrets with a friend.

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

## Licensing

Code written by me is licensed under *GPL-3.0-or-later*, some configs under *CC0-1.0*.

To verify licensing, use the [REUSE] tool by issuing `reuse lint` in the repo root.

### Attribution

Some prior art exists:

* Favicon created using art from the awesome [Twemoji] project by Twitter.
* Bootstrap script adapted from an [article] and [code] by Philipp Schmieder

[Twemoji]: https://twemoji.twitter.com
[article]: https://pentacent.medium.com/nginx-and-lets-encrypt-with-docker-in-less-than-5-minutes-b4b8a60d3a71
[code]: https://github.com/wmnnd/nginx-certbot
[REUSE]: https://reuse.software/
