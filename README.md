# Lexicon LMS
A system to manage courses and students i Lexicon

## **Första gång körning:**

1. Right-click on the LMS.API project and chosoe `Manage user secrets`
2. Fill the file that opens with:

	```
	{
	  "password": "YourSecretPasswordHere",
	  "JwtSettings": {
		"secretkey": "randomStringhere"
	  }
	}
	```

3. Change `YourSecretPasswordHere` to a password that you choose.

4. Run this command in PowerShell:

	```Powershell
	 $b = New-Object Byte[] 32; [System.Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($b); [Convert]::ToBase64String($b)
	``` 

	You will get a random string with the correct length. Copy that.

4. Paste that string in the json file as the value to t´he `secretKey`

5. Run `dotnet ef database update`


## **Swagger!**

Click [here](https://localhost:7213/swagger/index.html) to open the Swagegr UI.