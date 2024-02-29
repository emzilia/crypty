#!/bin/bash

SCRIPT='crypty.py'
SCRIPTPATH="$HOME/.local/bin/crypty"

# If file exists, remove it, if it doesn't, copy it over.
if [ -f "$SCRIPTPATH" ]; then
	echo "Uninstalling crypty script"
	rm "$SCRIPTPATH"
	if [ ! -f "$SCRIPTPATH" ]; then
		echo "Script successfully uninstalled!"
	else
		echo "Error: Uninstallation failed"
	fi
elif [ ! -f "$SCRIPTPATH" ]; then
	echo "Installing crypty script"
	cp "$SCRIPT" "$SCRIPTPATH"
	if [ -f "$SCRIPTPATH" ]; then
		echo "Script successfully installed!"
	else
		echo "Error: installation failed"
	fi
fi
