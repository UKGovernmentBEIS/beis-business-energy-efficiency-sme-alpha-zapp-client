# BEIS Zapp

## Zero to Hero

* Install [Visual Studio Professional 2017](https://visualstudio.microsoft.com/vs/professional/)
* Install [Resharper](https://www.jetbrains.com/resharper/)


## Release process

* The project uses [Squirrel](https://github.com/Squirrel/Squirrel.Windows) to create an app installer and package the app to allow background updates.
* The installer and update package are created as part of the Test and Release build configurations and the artefacts are created in a Releases folder under the root Zapp folder.
* These artefacts are then published to an S3 bucket (s3://beis-sme-alpha/Releases/) using the AWS CLI.

### Requirements:
* [AWS CLI](https://aws.amazon.com/cli/) installed
* AWS credentials with write permissions for the `beis-sme-alpha` S3 bucket
* Softwire code signing certificate present in the root folder of the codebase
* Environment variable `ZappCodeSigningCertificatePassword` added with the relevant password

### Release steps:
* Increment the `AssemblyVersion` and `AssemblyFileVersion` properties (in AssemblyInfo.cs)
* Clean and build the Release configuration
* Execute the following from the root folder to publish the artefacts to S3: `aws s3 sync Releases s3://beis-sme-alpha/Releases/`


## Troubleshooting

* If you encounter an exception of `"Update.exe not found, not a Squirrel-installed app?"` while attempting to debug the application in Visual Studio, you can resolve this by placing a copy of Squirrel's Update.exe in your bin directory (see [here](https://github.com/Squirrel/Squirrel.Windows/blob/master/docs/using/debugging-updates.md) for details).
