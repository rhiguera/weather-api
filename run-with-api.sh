#!/bin/bash
# Script to run WeatherApp with OpenWeatherMap API key
# Usage: ./run-with-api.sh YOUR_API_KEY_HERE

if [ -z "$1" ]; then
    echo "Usage: $0 YOUR_API_KEY"
    echo "Example: $0 abc123def456"
    exit 1
fi

export OPENWEATHER_API_KEY="$1"
dotnet build -q && dotnet run --project WeatherApp.UI
