# SPDX-FileCopyrightText: 2022 Simon Wendel
# SPDX-License-Identifier: GPL-3.0-or-later

#!/bin/bash
env -C deploy docker compose --project-name "push-a-secret" down -v
