#!/usr/bin/env bash
PROJECT_ROOT=$(pwd)
export SOURCE_DATE_EPOCH=$(date +%s)

# CONFIG defaults
CONFIG="Debug"
IS_RELEASE=false

if [[ " $* " == *" release "* ]]; then
    CONFIG="Release"
    IS_RELEASE=true
fi

build_target() {
    local RID=$1
    echo "🚀 Building UKSFTA P3D Debinarizer ($CONFIG) for $RID..."
    git submodule update --init --recursive
    # Restore solution to ensure all project references are valid
    dotnet restore P3DDebinarizer.sln
    # Publish project with RID
    dotnet publish src/P3DDebinarizer.csproj -c "$CONFIG" -r "$RID" --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=false -o "./dist/$RID"
    return $?
}

# 1. Build Logic
build_target "linux-x64"
LINUX_STATUS=$?

build_target "win-x64"
WIN_STATUS=$?

if [ $LINUX_STATUS -eq 0 ] && [ $WIN_STATUS -eq 0 ]; then
    # 2. Manual Packaging for releases
    if [ "$IS_RELEASE" = true ]; then
        echo "📦 Packaging Release ZIPs..."
        mkdir -p releases
        
        VERSION="0.0.0"
        if [ -f "VERSION" ]; then
            VERSION=$(cat VERSION | tr -d '\n\r ')
        fi
        
        PROJECT_ID=$(basename "$PROJECT_ROOT")
        STAGING_DIR="dist/zip_staging"
        rm -rf "$STAGING_DIR"
        
        # Package for each RID
        for RID in "linux-x64" "win-x64"; do
            ZIP_NAME="uksf task force alpha - ${PROJECT_ID,,}_${VERSION}_${RID}.zip"
            mkdir -p "$STAGING_DIR/$PROJECT_ID"
            
            # Copy build artifacts
            cp -rp dist/$RID/* "$STAGING_DIR/$PROJECT_ID/"
            # Copy common files
            cp -rp README.md LICENSE docs/ "$STAGING_DIR/$PROJECT_ID/" 2>/dev/null || true
            
            (cd "$STAGING_DIR" && zip -q -1 -r "$PROJECT_ROOT/releases/$ZIP_NAME" "$PROJECT_ID")
            
            # Consolidate to Unit Hub
            CENTRAL_HUB=""
            if [ -d "../UKSFTA-Tools/all_releases" ]; then
                CENTRAL_HUB="../UKSFTA-Tools/all_releases"
            fi

            if [ -n "$CENTRAL_HUB" ]; then
                echo "  - Consolidating $RID release to Unit Hub..."
                cp "$PROJECT_ROOT/releases/$ZIP_NAME" "$CENTRAL_HUB/"
            fi
            
            echo "✨ $RID release packaged: releases/$ZIP_NAME"
            rm -rf "$STAGING_DIR/$PROJECT_ID"
        done
        
        rm -rf "$STAGING_DIR"
    fi
fi

if [ $LINUX_STATUS -eq 0 ] && [ $WIN_STATUS -eq 0 ]; then
    echo "✅ All builds complete!"
    exit 0
else
    echo "❌ Some builds failed!"
    exit 1
fi
