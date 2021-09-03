In Windows, if you have trouble installing and executing the command line tools (kubectl, helm, tilt), here is what I prefer:
* download each executable and place them in C:\tools
* ensure the folder C:\tools is in path
    1) press windows key and type 'env'
    2) select "Edit the system environment variables"
    3) click the "Environment Variables.." button at the bottom
    4) select the row that has the value 'Path' in the 'Variable' column and click 'Edit..'
    5) click 'New' and type in 'C:\tools'
    6) click 'Ok' on all the opened windows
    7) close all terminal/command windows
    8) test by opening a new terminal/command windows and execute `tilt` and confirm you see the help command
