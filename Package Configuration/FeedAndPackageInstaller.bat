@echo off

:check_Permissions
    echo Administrative permissions required. Detecting permissions...

    net session >nul 2>&1
    if %errorLevel% == 0 (
        echo Success: Administrative permissions confirmed.
		goto install
    ) else (
        echo Failure: Current permissions inadequate. Re-run this script as an administrator to continue.
		goto end
    )
 
	

:install
	cd C:\Program Files\National Instruments\NI Package Manager\
	echo Adding feed...
	nipkg feed-add -n=rfmx-signal-creator https://raw.githubusercontent.com/NISystemsEngineering/package-repo/master/rfmx-signal-creator/Packages
	echo Installing packages...
	nipkg update rfmx-signal-creator
	nipkg install --accept-eulas -y rfmx-signal-creator rfmx-signal-creator-plugin-nr rfmx-signal-creator-plugin-wlan
	
:end
	pause >nul