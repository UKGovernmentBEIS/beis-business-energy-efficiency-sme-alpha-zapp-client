# BEIS Zapp

## Zero to Hero

### 0. Prerequisite

You will first need to set up the Zapp server prject for the client to work meaningfully.

See: https://github.com/UKGovernmentBEIS/beis-business-energy-efficiency-sme-alpha-zapp-server

### 1. Installs

#### Tools

* [Git](https://git-scm.com/)
* [Visual Studio Professional 2017](https://visualstudio.microsoft.com/vs/professional/)

#### Visual Studio extensions

* [SlowCheetah](https://marketplace.visualstudio.com/items?itemName=vscps.SlowCheetah-XMLTransforms) (required to build releases)
* [ReSharper](https://www.jetbrains.com/resharper/) (recommended)


### 2. Set up

* Check out the code from GitHub.
```
git clone git@github.com:UKGovernmentBEIS/beis-business-energy-efficiency-sme-alpha-zapp-client.git
```
* Open `Zapp.sln` in Visual Studio.
* The project should build without errors via **Build** > **Build solution (Ctrl+Shift+B)**.

### 3. Developing and debugging

* You will need to have a local version of the server running at `http://localhost:5000`, see the [Zapp server](https://github.com/UKGovernmentBEIS/beis-business-energy-efficiency-sme-alpha-zapp-server) project for set up details.
* Pressing **F5** or clicking **▶ Start** within Visual Studio will launch the application for debugging.

### 4. Release process

#### Overview

* The project uses [Squirrel](https://github.com/Squirrel/Squirrel.Windows) to create an app installer and package the app to allow background updates.
* The installer and update package are created as part of the **Release** build configurations and the artefacts are created in a `Releases/` folder at the project root.
* These artefacts are then checked in to the server project and deployed to Heroku.

#### Pre-build requirements

* You need to get a copy of the Softwire code signing certificate (`SoftwireCodeSigning2018.pfx`) and place it in the root folder of the codebase.
* Set a `ZappCodeSigningCertificatePassword` environment variable with the certificate password.

### Release
* Increment the `AssemblyVersion` and `AssemblyFileVersion` properties in `AssemblyInfo.cs`.
* Build the **Release** configuration of the project in Visual Studio.
* Copy the `Releases/` folder in to the server project as per the instructions on the [Zapp server](https://github.com/UKGovernmentBEIS/beis-business-energy-efficiency-sme-alpha-zapp-server)  Zero to Hero.
