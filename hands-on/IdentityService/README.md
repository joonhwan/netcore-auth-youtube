# openssl 을 사용해서 self certification file 만들기

왜 필요한지 아직 잘모르겠지만 

- HTTPS 
- IdentityServer

와 관련이 있는것은 확실하다. 쿨럭


- https://docs.microsoft.com/en-us/dotnet/core/additional-tools/self-signed-certificates-guide#with-openssl [1]

상기 문서를 보고, 

```
> PARENT=mirero.co.kr
> echo $PARENT
mirero.co.kr
```

처럼 환경변수 `PARENT` 를 설정한 다음, 

```
> openssl req \
-x509 \
-newkey rsa:4096 \
-sha256 \
-days 365 \
-nodes \
-keyout $PARENT.key \
-out $PARENT.crt \
-subj "/CN=${PARENT}" \
-extensions v3_ca \
-extensions v3_req \
-config <( \
  echo '[req]'; \
  echo 'default_bits= 4096'; \
  echo 'distinguished_name=req'; \
  echo 'x509_extension = v3_ca'; \
  echo 'req_extensions = v3_req'; \
  echo '[v3_req]'; \
  echo 'basicConstraints = CA:FALSE'; \
  echo 'keyUsage = nonRepudiation, digitalSignature, keyEncipherment'; \
  echo 'subjectAltName = @alt_names'; \
  echo '[ alt_names ]'; \
  echo "DNS.1 = www.${PARENT}"; \
  echo "DNS.2 = ${PARENT}"; \
  echo '[ v3_ca ]'; \
  echo 'subjectKeyIdentifier=hash'; \
  echo 'authorityKeyIdentifier=keyid:always,issuer'; \
  echo 'basicConstraints = critical, CA:TRUE, pathlen:0'; \
  echo 'keyUsage = critical, cRLSign, keyCertSign'; \
  echo 'extendedKeyUsage = serverAuth, clientAuth')
 ```
의 명령어와 

```
> openssl x509 -noout -text -in $PARENT.crt
```
를 차례대로 입력하면, 

- `mirero.co.kr.key` (=`$PARENT.key`)
- `mirero.co.kr.crt` (=`$PARENT.crt`) 

파일 2개가 생성된다. 

그런다음 위 2개파일과 어떤 패스워드를 정하여 `mirero.co.kr.pfx`(=`$PARENT.pfx`) 파일을 만든다.
(**주의** : 위 [1] 문서에서는 아래 명령에서 `-password pass:mirero` 부분이 없다. 
이게 없으면 패스워드를 터미널에서 입력시 자꾸 에러가 발생한다  --> [Stackoverflow 관련질문](https://stackoverflow.com/a/22328260/884268) 참고)


```
> openssl pkcs12 -export -out $PARENT.pfx -inkey $PARENT.key -in $PARENT.crt -password pass:mirero
```

위 처럼 진행하면 

- `mirero.co.kr.pfx` (=`$PARENT.pfx`) 

파일이 추가로 생긴다(총 3개의 파일이 생겼다). 

마지막에 생긴 `*.pfx` 파일로 IdentityServer4 의 SigningCredential 을 설정할 수 있다. 

```cs

        private X509Certificate2 Certification
        {
            get
            {
                var filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "mirero.co.kr.pfx");
                return new X509Certificate2(filePath, "mirero");
            }
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // ...
            
            services
                .AddIdentityServer()
                // ... 
                .AddSigningCredential(Certification)
                // ...
                ;

```

