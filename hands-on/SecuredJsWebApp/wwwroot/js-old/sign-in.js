const createState = function () {
    // TODO 먼가 real 한 key 생성 루틴이 필요.
    return "SessionValueMakeItABitLongerasdfhjsadoighasdifjdsalkhrfakwelyrosdpiufghasidkgewr";
};

const createNonce = function () {
    // TODO 먼가 real 한 key 생성 루틴이 필요.
    return "NonceValuedsafliudsayatroiewewryie123";
};

const signIn = () => {
    const redirectUri = "https://localhost:60011/signin"
    const client_id = "mirero.secured.web.app"
    const responseType = "id_token token";
    const scope = "openid scope.mirero.api.type.secret";
    const authUrl =
        "/connect/authorize/callback" +
        "?client_id=" + client_id +
        "&redirect_uri=" + encodeURIComponent(redirectUri) +
        "&response_type=" + encodeURIComponent(responseType) +
        "&scope=" + encodeURIComponent(scope) +
        "&nonce=" + createNonce() +
        "&state=" + createState();

    const returnUrl = encodeURIComponent(authUrl);
    //console.log(returnUrl);

    const identityServerUri = "https://localhost:50001";
    window.location.href = `${identityServerUri}/Auth/Login?ReturnUrl=${returnUrl}`;
}

