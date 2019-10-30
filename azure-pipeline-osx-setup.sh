#!/bin/bash

MONO_VERSION=5_18_1
XCODE_VERSION=10.3

echo "Switch to the latest Xamarin SDK"
sudo $AGENT_HOMEDIRECTORY/scripts/select-xamarin-sdk.sh $MONO_VERSION
echo "Switch to the latest Xcode"
echo '##vso[task.setvariable variable=MD_APPLE_SDK_ROOT;]'/Applications/Xcode_$XCODE_VERSION.app;sudo xcode-select --switch /Applications/Xcode_$XCODE_VERSION.app/Contents/Developer
MONOPREFIX=/Library/Frameworks/Mono.framework/Versions/$MONO_VERSION
echo "##vso[task.setvariable variable=DYLD_FALLBACK_LIBRARY_PATH;]$MONOPREFIX/lib:/lib:/usr/lib:$DYLD_LIBRARY_FALLBACK_PATH"
echo "##vso[task.setvariable variable=PKG_CONFIG_PATH;]$MONOPREFIX/lib/pkgconfig:$MONOPREFIX/share/pkgconfig:$PKG_CONFIG_PATH"
echo "##vso[task.setvariable variable=PATH;]$MONOPREFIX/Commands/bin:$PATH"
echo "Set Mono"
