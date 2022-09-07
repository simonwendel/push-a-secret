# SPDX-FileCopyrightText: 2022 Simon Wendel
# SPDX-License-Identifier: GPL-3.0-or-later

FROM node:18.7-alpine3.15 AS build

LABEL \
  stage=intermediate

ARG ELM_VERSION="0.19.1"
ARG ELM_SHA256="e44af52bb27f725a973478e589d990a6428e115fe1bb14f03833134d6c0f155c"

ARG ELM_ENVIRONMENT="Development"
ARG ELM_CONFIG_API="http://localhost:5000"
ENV ELM_CONFIG_API=${ELM_CONFIG_API}

WORKDIR /work

RUN apk update \
  && apk upgrade \
  && apk add curl gettext

RUN curl -L -o elm.gz \
      https://github.com/elm/compiler/releases/download/${ELM_VERSION}/binary-for-linux-64-bit.gz \
  && echo "${ELM_SHA256} *elm.gz" | sha256sum -c - \
  && gunzip elm.gz \
  && chmod +x elm \
  && mv elm /usr/local/bin/ \
  && elm --help

COPY . .

RUN envsubst < templates/Configuration.elm.template > src/Configuration.elm

RUN npm install \
  && npx gulp

RUN cat dist/app.js | grep -i api_url

FROM nginx:1.23.1 AS final

WORKDIR /srv/www
COPY --from=build /work/dist .
RUN ls -l

RUN rm /etc/nginx/conf.d/*.conf
COPY nginx_static.conf /etc/nginx/conf.d/default.conf

LABEL \
  stage=final \
  name="push-a-secret-app" \
  description="Push-A-Secret Web App" \
  maintainer='Simon Wendel <mail@simonwendel.se>'
