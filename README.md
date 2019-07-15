# UnofficialGenconMobile
This is the Unofficial Gen Con Mobile application - Xamarin version. This is the **current production version** of the app.

**WE NEED HELP!** Please see "Code Contributions" and "Other Contributions" below!

## How to build
You may install the Xamarin toolset and Visual Studio 2019, and open the SLN file like any other project.

## Special Build Note
Sometimes when opening on Visual Studio 2019 for the first time, the build will fail with a cannot find DLL error. To fix this, right-click on each project (ConventionMobile, ConventionMobile.Android, etc) and choose "clean" then "rebuild". It should fix the problem and you should be able to run again.
*Make sure to set your startup project as the platform project you're running on! e.g. if you're on Android, right-click "ConventionMobile.Android" and choose "set as startup project"*

## Contributing
All accepted contributions will be merged into the main branch and then deployed to the production app stores (Play Store, App Store) in release waves preceding the convention.

There are two ways to contribute:
1. If you can code - Submit a pull request! I'll work with you to get it integrated.
2. If you can submit maps or other guide data (see "Other Contributions" below).

### Code Contributions
Please make sure to document/comment and follow reasonable coding practices. Please also make sure that commits are concise and limited in scope to the thing they are meant to address. If you want to introduce a new feature, limit the pull request to that feature. If you want to make appearance changes, please limit the pull request to those changes. Do not combine the two requests into one.

### Other Contributions
The maps for 2019 must be completely made from scratch. We cannot use screenshots from the website. The contents (including names, room numbers, and other details) may be present on the maps, but they cannot be directly screen-shotted from the website. If you are interested in redrawing the maps, please do so!! You may submit a pull-request here, or if you don't understand github, please send an email to SUPPORT @ VECTORLIT . NET and we will help you get what you need started.

### Web services and foundation support
There are several underlying REST web services which provide app state management, file downloads, user event list synchronization, and event downloads. The code for these services is not yet open source, but the services will remain available for use with this app. The endpoints are described in the code and are generally self-explanatory. If you have any issues or need changes for an update, please open an issue.

### A note about versions
This is the XAMARIN version of the app, which has been in production for visitors from 2015-Present. A newer attempt to recreate this app in Flutter has been started at https://github.com/vectorlit/unofficial_gencon_mobile . If you want to try to advance the state of the newer app (and potentially replace this one), please visit that repository instead.

### Licensing
This code and resources are being distributed under the GPL-3.0 license. IF YOU USE THIS CODE, YOU HAVE A LEGAL REQUIREMENT TO ABIDE BY THE RULES SPECIFIED IN THE LICENSE.
