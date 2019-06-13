#!/usr/bin/env bash
find Assets/_Core -iname "*.cs" -not -path "Assets/_Core/PluginsSetting/GoogleSheetImport/GeneratedScripts/*" | xargs -n 1 -I {} bash -c "astyle --options=.astylerc \"{}\""