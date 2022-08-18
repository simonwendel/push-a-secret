#!/bin/bash
set -e
while true; do sleep 6h; nginx -s reload; done &
