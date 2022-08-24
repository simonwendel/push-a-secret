#!/bin/bash

# SPDX-FileCopyrightText: 2022 Simon Wendel
# SPDX-License-Identifier: GPL-3.0-or-later

env -C deploy docker compose --project-name "push-a-secret" down
