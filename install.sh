#!/bin/sh

SCRIPT='crypty'
SCRIPTPATH="${HOME}/.local/bin/crypty"

# If file exists, remove it, if it doesn't, copy it over.
if [ -f "${SCRIPTPATH}" ]; then
	printf "Uninstalling crypty script\n"
	rm "${SCRIPTPATH}"
	if [ ! -f "${SCRIPTPATH}" ]; then
		printf "Script successfully uninstalled!\n"
	else
		printf "Error: Uninstallation failed\n"
	fi
elif [ ! -f "${SCRIPTPATH}" ]; then
	printf "Installing crypty script\n"
	cp "${SCRIPT}" "${SCRIPTPATH}"
	if [ -f "${SCRIPTPATH}" ]; then
		printf "Script successfully installed!\n"
	else
		printf "Error: installation failed\n"
	fi
fi
