keytool -exportcert -alias allcashgames -keystore allcashgames.keystore | openssl sha1 -binary | openssl base64
pause