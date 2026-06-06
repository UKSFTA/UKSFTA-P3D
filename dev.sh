#!/bin/bash
PROJECT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$PROJECT_DIR"

command=$1

# Force Linux development
export DOTNET_CLI_TELEMETRY_OPTOUT=1

case $command in
    "test")
        echo "🧪 Running Fast Unit Tests..."
        dotnet test --no-restore -c Debug
        ;;
    "lint")
        echo "🧹 Linting & Formatting..."
        dotnet format
        ;;
    "run")
        shift
        # Fast run using the dev DLL directly
        dotnet P3DDebinarizer/bin/Debug/net10.0/debinarizer.dll "$@"
        ;;
    "watch")
        echo "👀 Watching for changes..."
        dotnet watch test --c Debug
        ;;
    *)
        echo "Usage: ./dev.sh {test|lint|run|watch}"
        ;;
esac
