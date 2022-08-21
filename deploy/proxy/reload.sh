# SPDX-FileCopyrightText: 2022 Simon Wendel
# SPDX-License-Identifier: GPL-3.0-or-later

#!/bin/bash
set -e
while true; do sleep 6h; nginx -s reload; done &
