# OWASP Wellington MFA Demo
Demo application to accompany OWASP Wellington Meetup on multi-factor authentication

[Make the Cyber Safer with Multi-factor Authentication](https://www.meetup.com/OWASP-Wellington/events/255158934/) October 29, 2018

## Objectives
The applications contained herein are intended to demonstrate some key concepts around multi-factor authentication using TOTP:
* basic security concepts such as compartmentalisation, hashing, and encryption
* TOTP token calculations with explanatory comments and references
* unit tests verifying compliance with RFC 6238 specifications
* alternate implementation using existing OtpSharp library and configurable via _appsettings.json_

## Key Components
* ASP.NET Core 2.1 web application to demonstrate MFA registration and login using TOTP as the possession factor
* .NET Core 2.1 console application to demonstrate a TOTP token provider
* .NET Standard class library contains most TOTP functionality
* .NET Framework unit test coverage over most TOTP MFA functionality

There is no identity database, all credentials and keys are stored in memory purely for demonstration purposes. Classes in the __OwaspDemo.Identity__ namespace were designed and commented to closely mimic equivalent classes in the __Microsoft.AspNetCore.Identity__ namespace.

## Installation and Getting Started
Clone the source code to a local environment, compile, and run. You can use the __SoftwareTokenProvider__ console application to provide tokens, or you can use a mobile application such as Authy to scan the QR code.

1. Run the OWASP Demo web application
2. Register
3. Enter _User Name_ and _Password_ (minimum 8 characters, no other restrictions)
4. Tick _Enable Two-Factor Authentication_
5. If using mobile application, scan the QR code. If using the console application, click _Manual Entry_ link, copy the entire _otpauth_ URL, and paste it into the console application at the prompt.
6. Enter the generated token to complete registration with MFA enabled.

## Other Notes
* If you want to create an ASP.NET Core 2.1 web app with MFA, you can start with [these Identity instructions](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-2.1&tabs=visual-studio) to set up basic Identity, and then [these QR code instructions](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity-enable-qrcodes?view=aspnetcore-2.1) to add TOTP two-factor authentication.
* I've had a few issues with the unit test project references. First I encountered [this issue](https://github.com/dotnet/standard/issues/481) which is the result of referencing a .NET Core (or .NET Standard) project into a .NET Framework project. I changed the unit test project to use `PackageReference` as recommended. I still encountered inconsistent reference issues in the unit test project, so I added a `<RestoreProjectStyle>PackageReference</RestoreProjectStyle>` line within the first `<PropertyGroup>` as recommended by [this Scott Hanselman blog post](https://www.hanselman.com/blog/ReferencingNETStandardAssembliesFromBothNETCoreAndNETFramework.aspx). The unit tests seem to compile and run consistently now, both in a VS test runner and in NCrunch, although they will fail to compile if either the web app or console application are running because of resource locks. 
