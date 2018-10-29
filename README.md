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
* ASP.NET Core web application to demonstrate MFA registration and login using TOTP as the possession factor
* .NET Core console application to demonstrate a TOTP token provider
* Unit tests coverage over most TOTP MFA functionality
* .NET Standard class library contains most TOTP functionality

There is no identity database, all credentials and keys are stored in memory purely for demonstration purposes. Classes in the __OwaspDemo.Identity__ namespace were designed and commented to closely mimic equivalent classes in the __Microsoft.AspNetCore.Identity__ namespace.

## Installation and Getting Started
Clone the source code to a local environment, compile, and run. You can use the __SoftwareTokenProvider__ console application to provide tokens, or you can use a mobile application such as Authy to scan the QR code.

1. Run the OWASP Demo web application
2. Register
3. Enter _User Name_ and _Password_ (minimum 8 characters, no other restrictions)
4. Tick _Enable Two-Factor Authentication_
5. If using mobile application, scan the QR code. If using the console application, click _Manual Entry_ link, copy the entire _otpauth_ URL, and paste it into the console application at the prompt.
6. Enter the generated token to complete registration with MFA enabled.
