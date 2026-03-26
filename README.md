# Lexicon LMS
En system för att hantera kurser och elever i Lexicon

## **Första gång körning:**

1. Höger clicka LMS.API projekt och välja `Manage user secrets`
2. I filen som öpnas, skriv:

	```
	{
	  "password": "YourSecretPasswordHere",
	  "JwtSettings": {
		"secretkey": "randomStringhere"
	  }
	}
	```

3. Bytta `YourSecretPasswordHere` till till valda lösenord.

4. Kör kommanden nedan i Porshell:

	```Powershell
	 $b = New-Object Byte[] 32; [System.Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($b); [Convert]::ToBase64String($b)
	``` 

	Du ska få en random string med rätta längden. Kopiera den.

4. Klistra in strängen i json filen som värde till `secretKey`

5. Kör `dotnet ef database update`


## **Swagger!**

Clicka [här](https://localhost:7213/swagger/index.html) för att öopna Swagger UI